using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public event Action HPChanged;

    [field: SerializeField]
    public MonsterStat Stat { get; private set; }

    [field: SerializeField, Header("플레이어 탐지")]
    public float DetectionRadius { get; private set; }
    [field: SerializeField, Range(0, 360)]
    public float DetectionAngle { get; private set; }
    [field: SerializeField]
    public Transform Eyes { get; private set; }
    [field: SerializeField]
    public LayerMask TargetMask { get; private set; }
    [field: SerializeField]
    public LayerMask ObstacleMask { get; private set; }


    [field: SerializeField, Header("플레이어 추적")]
    public float TrackingDistance { get; private set; }

    [field: SerializeField, Header("전투")]
    public float AttackDistance { get; private set; }

    public Collider Collider { get; private set; }
    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }

    public int CurrentHP { get; set; }
    public int CurrentDamage { get; private set; }
    public Vector3 OriginalPosition { get; private set; }

    public bool IsLockOnTarget
    {
        get => _isLockOnTarget;
        set
        {
            _isLockOnTarget = value;
            if (_isLockOnTarget)
            {
                ShowHPBar();
            }
        }
    }

    private UI_MonsterHPBar _hpBar;
    private bool _isLockOnTarget;
    private readonly Collider[] _playerCollider = new Collider[1];
    private Dictionary<BasicMonsterState, int> _stateAnimID = new();

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Monster");
        Collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();

        _stateAnimID.Add(BasicMonsterState.Idle, Animator.StringToHash("Idle"));
        _stateAnimID.Add(BasicMonsterState.Tracking, Animator.StringToHash("Tracking"));
        _stateAnimID.Add(BasicMonsterState.Restore, Animator.StringToHash("Restore"));
    }

    private void OnEnable()
    {
        CurrentHP = Stat.MaxHP;
        OriginalPosition = transform.position;
        Collider.enabled = true;
    }

    public void Transition(BasicMonsterState state)
    {
        Animator.SetTrigger(_stateAnimID[state]);
    }

    public void TakeDamage(int damage)
    {
        ShowHPBar();
        CurrentDamage = Mathf.Clamp(damage - Stat.Defense, 0, damage);
        CurrentHP -= CurrentDamage;
        HPChanged?.Invoke();
        CurrentDamage = 0;

        if (CurrentHP <= 0)
        {
            // DIE
        }
        else
        {
            // DAMAGED
        }
    }

    public bool PlayerDetect()
    {
        var monsterCenterPos = Collider.bounds.center;
        int cnt = Physics.OverlapSphereNonAlloc(monsterCenterPos, DetectionRadius, _playerCollider, TargetMask);
        if (cnt != 0)
        {
            var playerCenterPos = _playerCollider[0].bounds.center;
            var centerDir = (playerCenterPos - monsterCenterPos).normalized;
            if (Vector3.Angle(transform.forward, centerDir) < DetectionAngle * 0.5f)
            {
                var monsterEyesPos = Eyes.position;
                var eyesDir = playerCenterPos - monsterEyesPos;
                var EyesToPlayerCenterDist = Vector3.Distance(monsterEyesPos, playerCenterPos);
                if (!Physics.Raycast(monsterEyesPos, eyesDir.normalized, EyesToPlayerCenterDist, ObstacleMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsThePlayerInAttackRange()
    {
        return Vector3.Distance(Player.GameObject.transform.position, transform.position) <= AttackDistance;
    }

    public bool IsArrivedCurrentDestination()
    {
        return !NavMeshAgent.pathPending && NavMeshAgent.remainingDistance <= 0f;
    }

    public void ResetAllTriggers()
    {
        foreach (var param in Animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                Animator.ResetTrigger(param.name);
            }
        }
    }

    public void NaveMeshAgentUpdateToggle(bool toggle)
    {
        NavMeshAgent.isStopped = !toggle;
        NavMeshAgent.updatePosition = toggle;
        NavMeshAgent.updateRotation = toggle;
        if (!toggle)
        {
            NavMeshAgent.velocity = Vector3.zero;
        }
    }

    private void ShowHPBar()
    {
        if (_hpBar == null)
        {
            _hpBar = Managers.Resource.Instantiate("UI_MonsterHPBar",
                Managers.UI.Get<UI_AutoCanvas>().transform, true).GetComponent<UI_MonsterHPBar>();
            _hpBar.SetTarget(this);
        }
        else
        {
            HPChanged?.Invoke();
        }
    }
}
