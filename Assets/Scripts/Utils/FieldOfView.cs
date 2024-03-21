using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public bool Detected { get; private set; }
    public Transform Target => _collider[0].transform;

    [field: SerializeField]
    public float Radius { get; private set; }

    [field: SerializeField, Range(0, 360)]
    public float Angle { get; private set; }

    [SerializeField]
    private Transform _startRaycastTransform;

    [SerializeField]
    private bool _rayToTargetCenter;

    [SerializeField]
    private LayerMask _targetMask;

    [SerializeField]
    private LayerMask _obstacleMask;

    private readonly Collider[] _collider = new Collider[1];

    private void Start()
    {
        if (_startRaycastTransform == null)
        {
            _startRaycastTransform = transform;
        }
    }

    public void CheckFieldOfView()
    {
        int cnt = Physics.OverlapSphereNonAlloc(transform.position, Radius, _collider, _targetMask);
        if (cnt != 0)
        {
            var dirToTarget = (_collider[0].transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < Angle * 0.5f)
            {
                var targetPos = _rayToTargetCenter ? _collider[0].bounds.center : _collider[0].transform.position;
                var rayDirToTarget = (targetPos - _startRaycastTransform.position).normalized;
                var dist = Vector3.Distance(_startRaycastTransform.position, targetPos);
                if (!Physics.Raycast(_startRaycastTransform.position, rayDirToTarget, dist, _obstacleMask))
                {
                    Detected = true;
                    return;
                }
            }
        }

        Detected = false;
    }
}
