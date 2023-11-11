using UnityEngine;

public class UI_FollowTarget : UI_Base
{
    public Transform Target { get; private set; }

    [SerializeField]
    private bool _ignoreBoundsCenter;
    private Vector3 _deltaPos;
    private RectTransform _rt;
    private Collider _targetCollider;

    protected override void Init()
    {
        _rt = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        var targetPos = Target.transform.position;
        if (!_ignoreBoundsCenter)
        {
            targetPos.y = _targetCollider.bounds.center.y;
        }

        _rt.position = Camera.main.WorldToScreenPoint(new Vector3(targetPos.x, targetPos.y, targetPos.z) + _deltaPos);
    }

    public void SetTarget(Transform target, Vector3? deltaPos = null)
    {
        Target = target;
        _deltaPos = deltaPos ?? Vector3.zero;

        if (target != null && !_ignoreBoundsCenter)
        {
            _targetCollider = Target.GetComponent<Collider>();
        }

        gameObject.SetActive(target != null);
    }
}
