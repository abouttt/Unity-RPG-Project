using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour
{
    public event Action HPChanged;

    [field: SerializeField]
    public MonsterData Data { get; private set; }

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
    [field: SerializeField]
    public float RotationSmoothTime { get; private set; }
    [field: SerializeField]
    public float AttackDelayTime { get; private set; }
    [field: SerializeField]
    public Transform AttackOffset { get; private set; }
    [field: SerializeField]
    public float AttackRadius { get; private set; }

    public Collider Collider { get; private set; }
    public IReadOnlyList<Collider> LockOnTargetColliders => _lockOnTargetColliders;
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

    protected readonly Collider[] PlayerCollider = new Collider[1];

    private UI_MonsterHPBar _hpBar;
    private bool _isLockOnTarget;
    private readonly Dictionary<BasicMonsterState, int> _stateAnimID = new();
    private readonly List<Collider> _lockOnTargetColliders = new();

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Monster");
        Collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();

        int lockOnTargetLayer = LayerMask.NameToLayer("LockOnTarget");
        foreach (Transform child in transform)
        {
            if (child.gameObject.layer == lockOnTargetLayer)
            {
                _lockOnTargetColliders.Add(child.GetComponent<Collider>());
            }
        }

        _stateAnimID.Add(BasicMonsterState.Idle, Animator.StringToHash("Idle"));
        _stateAnimID.Add(BasicMonsterState.Tracking, Animator.StringToHash("Tracking"));
        _stateAnimID.Add(BasicMonsterState.Restore, Animator.StringToHash("Restore"));
        _stateAnimID.Add(BasicMonsterState.Attack, Animator.StringToHash("Attack"));
        _stateAnimID.Add(BasicMonsterState.Stunned, Animator.StringToHash("Stunned"));
        _stateAnimID.Add(BasicMonsterState.Damaged, -1);
        _stateAnimID.Add(BasicMonsterState.Dead, -1);

        if (Managers.Resource.HasResources)
        {
            Managers.Resource.Instantiate("MinimapIcon", transform).GetComponent<MinimapIcon>().Setup("MonsterMinimapIcon", Data.MonsterName);
        }
    }

    private void OnEnable()
    {
        CurrentHP = Data.MaxHP;
        OriginalPosition = transform.position;
        Collider.isTrigger = false;
        foreach (var collider in _lockOnTargetColliders)
        {
            collider.enabled = true;
        }
    }

    public void Transition(BasicMonsterState state)
    {
        if (_stateAnimID[state] == -1)
        {
            Animator.Play(state.ToString(), -1, 0f);
        }
        else
        {
            Animator.SetTrigger(_stateAnimID[state]);
        }
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHP <= 0)
        {
            return;
        }

        ShowHPBar();
        CurrentDamage = Mathf.Clamp(damage - Data.Defense, 0, damage);
        CurrentHP -= CurrentDamage;
        HPChanged?.Invoke();
        CurrentDamage = 0;

        if (CurrentHP <= 0)
        {
            Transition(BasicMonsterState.Dead);
        }
        else
        {
            Transition(BasicMonsterState.Damaged);
        }
    }

    public void Stunned()
    {
        if (CurrentHP <= 0)
        {
            return;
        }

        Transition(BasicMonsterState.Stunned);
    }

    public bool PlayerDetect()
    {
        var monsterCenterPos = Collider.bounds.center;
        int cnt = Physics.OverlapSphereNonAlloc(monsterCenterPos, DetectionRadius, PlayerCollider, TargetMask);
        if (cnt != 0)
        {
            var playerCenterPos = PlayerCollider[0].bounds.center;
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
