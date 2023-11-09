using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("[Rotate]")]
    [SerializeField]
    private GameObject _cinemachineCameraTarget;
    [SerializeField]
    private float _sensitivity;
    [SerializeField]
    private float _topClamp;
    [SerializeField]
    private float _bottomClamp;

    private float _cinemachineTargetYaw;    // Y
    private float _cinemachineTargetPitch;  // X

    private void Start()
    {
        _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        if (Managers.Input.Look.sqrMagnitude >= 0.01f)
        {
            _cinemachineTargetYaw += Managers.Input.Look.x * _sensitivity;
            _cinemachineTargetPitch += Managers.Input.Look.y * _sensitivity;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
        _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
    }

    private float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle > 180)
        {
            return lfAngle - 360f;
        }
        else if (lfAngle < -180)
        {
            return lfAngle + 360f;
        }

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
