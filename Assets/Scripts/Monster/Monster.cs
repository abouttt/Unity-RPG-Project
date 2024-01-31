using System;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public event Action HPChanged;

    [field: SerializeField]
    public MonsterStat Stat { get; private set; }

    [field: SerializeField, Header("�÷��̾� Ž��")]
    public float DetectionRadius { get; private set; }
    [field: SerializeField, Range(0, 360)]
    public float DetectionAngle { get; private set; }
    [field: SerializeField]
    public Transform Eyes { get; private set; }
    [field: SerializeField]
    public LayerMask TargetMask { get; private set; }
    [field: SerializeField]
    public LayerMask ObstacleMask { get; private set; }


    [field: SerializeField, Header("�÷��̾� ����")]
    public float TrackingDistance { get; private set; }

    [field: SerializeField, Header("����")]
    public float AttackDistance {  get; private set; }

    public Collider Collider { get; private set; }
    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }

    public int CurrentHP { get; set; }
    public int CurrentDamage { get; private set; }

    public readonly int AnimIDIdle = Animator.StringToHash("Idle");
    public readonly int AnimIDTracking = Animator.StringToHash("Tracking");

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

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Monster");
        Collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        CurrentHP = Stat.MaxHP;
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

    public bool IsThePlayerInAttackRange()
    {
        return Vector3.Distance(Player.GameObject.transform.position, transform.position) <= AttackDistance;
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
