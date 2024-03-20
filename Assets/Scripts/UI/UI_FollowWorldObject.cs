using UnityEngine;

public class UI_FollowWorldObject : MonoBehaviour
{
    public Transform Target { get; private set; }

    [SerializeField]
    private bool _ignoreBoundsCenter;
    private Vector3 _offest;
    private RectTransform _rt;
    private Collider _targetCollider;
    private Camera _mainCamera;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        var targetPos = Target.position;

        if (!_ignoreBoundsCenter)
        {
            targetPos.y = _targetCollider.bounds.center.y;
        }

        _rt.position = _mainCamera.WorldToScreenPoint(new Vector3(targetPos.x, targetPos.y, targetPos.z) + _offest);
    }

    public void SetTarget(Transform target, Vector3? offest = null)
    {
        Target = target;
        _offest = offest ?? Vector3.zero;

        if (target != null && !_ignoreBoundsCenter)
        {
            _targetCollider = Target.GetComponent<Collider>();
        }

        gameObject.SetActive(target != null);
    }
}
