using UnityEngine;
using Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    public Transform LockOnTarget
    {
        get => _stateDrivenCamera.LookAt;
        set
        {
            if (IsLockOn && value == null)
            {
                _stateDrivenCamera.LookAt.GetComponent<LockOnTarget>().IsLockOn = false;
            }

            IsLockOn = value != null;
            _stateDrivenCamera.LookAt = value;
            _stateDrivenCameraAnimator.SetBool(_animIDLockOn, IsLockOn);
            _lockOnTargetImage.SetTarget(_stateDrivenCamera.LookAt);
            Player.Animator.SetBool(_animIDLockOn, IsLockOn);

            if (IsLockOn)
            {
                _stateDrivenCamera.LookAt.GetComponent<LockOnTarget>().IsLockOn = true;
            }
        }
    }

    public bool IsLockOn { get; private set; }

    [Header("[Rotate]")]
    [SerializeField]
    private GameObject _cinemachineCameraTarget;

    [SerializeField]
    private float _sensitivity;

    [SerializeField]
    private float _topClamp;

    [SerializeField]
    private float _bottomClamp;

    [Space(10)]
    [Header("[Lock Target]")]
    [SerializeField]
    private CinemachineStateDrivenCamera _stateDrivenCamera;

    [SerializeField]
    private float _viewRadius;

    [Range(0, 360)]
    [SerializeField]
    private float _viewAngle;

    [SerializeField]
    private LayerMask _targetMask;

    [SerializeField]
    private LayerMask _obstacleMask;

    private Animator _stateDrivenCameraAnimator;
    private readonly Collider[] _lockOnTargets = new Collider[10];
    private readonly int _animIDLockOn = Animator.StringToHash("LockOn");

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private readonly float _threshold = 0.01f;

    private GameObject _mainCamera;
    private UI_FollowWorldObject _lockOnTargetImage;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _stateDrivenCameraAnimator = _stateDrivenCamera.GetComponent<Animator>();
    }

    private void Start()
    {
        _lockOnTargetImage = Managers.Resource.Instantiate(
            "UI_LockOnTargetImage.prefab", Managers.UI.Get<UI_AutoCanvas>().transform).GetComponent<UI_FollowWorldObject>();
        _lockOnTargetImage.gameObject.SetActive(false);
        _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void Update()
    {
        if (Managers.Input.LockOn)
        {
            if (IsLockOn)
            {
                LockOnTarget = null;
            }
            else
            {
                FindTarget();
            }
        }
    }

    private void LateUpdate()
    {
        CameraRotation();

        if (IsLockOn)
        {
            TrackingLockOnTarget();
        }
    }

    private void CameraRotation()
    {
        if (IsLockOn)
        {
            _cinemachineCameraTarget.transform.rotation = _stateDrivenCamera.transform.rotation;
            var eulerAngles = _cinemachineCameraTarget.transform.eulerAngles;
            _cinemachineTargetPitch = eulerAngles.x;
            _cinemachineTargetYaw = eulerAngles.y;
        }
        else
        {
            if (Managers.Input.Look.sqrMagnitude >= _threshold)
            {
                _cinemachineTargetYaw += Managers.Input.Look.x * _sensitivity;
                _cinemachineTargetPitch += Managers.Input.Look.y * _sensitivity;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
            _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
        }
    }

    private void FindTarget()
    {
        float shortestAngle = Mathf.Infinity;
        Transform finalTarget = null;

        int targetCnt = Physics.OverlapSphereNonAlloc(_mainCamera.transform.position, _viewRadius, _lockOnTargets, _targetMask);
        for (int i = 0; i < targetCnt; i++)
        {
            var dirToTarget = (_lockOnTargets[i].transform.position - _mainCamera.transform.position).normalized;
            float currentAngle = Vector3.Angle(_mainCamera.transform.forward, dirToTarget);

            if (currentAngle < _viewAngle * 0.5f)
            {
                float distToTarget = Vector3.Distance(_mainCamera.transform.position, _lockOnTargets[i].transform.position);

                if (currentAngle < shortestAngle)
                {
                    if (!Physics.Raycast(_mainCamera.transform.position, dirToTarget, distToTarget, _obstacleMask))
                    {
                        finalTarget = _lockOnTargets[i].transform;
                        shortestAngle = currentAngle;
                    }
                }
            }
        }

        LockOnTarget = finalTarget;
    }

    private void TrackingLockOnTarget()
    {
        if (!LockOnTarget.gameObject.activeInHierarchy)
        {
            LockOnTarget = null;
            return;
        }

        float distToTarget = Vector3.Distance(_mainCamera.transform.position, LockOnTarget.position);
        if (distToTarget > _viewRadius)
        {
            LockOnTarget = null;
            return;
        }

        var dirToTarget = (LockOnTarget.position - _mainCamera.transform.position).normalized;
        if (Physics.Raycast(_mainCamera.transform.position, dirToTarget, distToTarget, _obstacleMask))
        {
            LockOnTarget = null;
            return;
        }

        float pitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
        if (_bottomClamp > pitch || pitch > _topClamp)
        {
            LockOnTarget = null;
        }
    }

    private float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle > 180)
        {
            return lfAngle - 360f;
        }

        if (lfAngle < -180)
        {
            return lfAngle + 360f;
        }

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
