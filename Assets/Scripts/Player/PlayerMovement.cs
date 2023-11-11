using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;                         // 기본 속도
    public float SprintSpeed;                       // 달리기 속도
    public float RotationSmoothTime;                // 회전 속도
    public float SpeedChangeRate;                   // 가속 및 감속
    public float JumpLandDecreaseSpeedPercentage;   // 점프 착지시 속도 감소값

    [Space(10)]
    public float JumpHeight;                        // 점프 높이
    public float Gravity;                           // 자체 중력 값

    [Space(10)]
    public float JumpTimeout;                       // 다시 점프하기까지 시간
    public float FallTimeout;                       // 낙하 상태에 진입까지 시간

    [Space(10)]
    public float GroundedOffset;                    // 땅 체크 위치
    public float GroundedRadius;                    // 땅 체크 구체 반지름
    public LayerMask GroundLayers;                  // 땅 레이어

    public bool IsJumping { get; private set; }

    // 최종 적용 값
    private float _speed;
    private float _posXBlend;
    private float _posZBlend;
    private float _animationBlend;
    private float _lockOffRotation = 0f;
    private float _lockOnRotation = 0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private const float _terminalVelocity = 53.0f;

    // 타임아웃 델타타임
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private bool _isJumpLand;
    private bool _isGrounded;
    private bool _isPrevLockOn;

    private CharacterController _controller;

    private readonly int _animIDPosX = Animator.StringToHash("PosX");
    private readonly int _animIDPosZ = Animator.StringToHash("PosZ");
    private readonly int _animIDSpeed = Animator.StringToHash("Speed");
    private readonly int _animIDGrounded = Animator.StringToHash("Grounded");
    private readonly int _animIDJump = Animator.StringToHash("Jump");
    private readonly int _animIDFreeFall = Animator.StringToHash("FreeFall");
    private readonly int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    private void JumpAndGravity()
    {
        if (_isGrounded)
        {
            _fallTimeoutDelta = FallTimeout;

            Player.Animator.SetBool(_animIDJump, false);
            Player.Animator.SetBool(_animIDFreeFall, false);

            // 착지되었을 때 속도가 무한히 떨어지는 것을 멈춤.
            if (_verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            // 점프
            if (Managers.Input.Jump && _jumpTimeoutDelta <= 0f)
            {
                // 원하는 높이에 도달하는 데 필요한 속도 = H * -2 * G 의 제곱근.
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                Player.Animator.SetBool(_animIDJump, true);
                IsJumping = true;
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                Player.Animator.SetBool(_animIDFreeFall, true);
            }
        }

        // 중력
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private void GroundedCheck()
    {
        var position = transform.position;
        var spherePosition = new Vector3(position.x, position.y - GroundedOffset, position.z);
        _isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        Player.Animator.SetBool(_animIDGrounded, _isGrounded);
    }

    private void Move()
    {
        float targetSpeed = MoveSpeed;

        if (_isJumpLand)
        {
            targetSpeed = MoveSpeed * JumpLandDecreaseSpeedPercentage;
        }
        else
        {
            if (Managers.Input.Sprint)
            {
                targetSpeed = SprintSpeed;
            }
        }

        if (Managers.Input.Move == Vector2.zero)
        {
            targetSpeed = 0f;
        }

        var currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;

        // 목표 속도로 가속 또는 감속.
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
            // 소수점 이하 3자리까지의 반올림 속도.
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _posXBlend = Mathf.Lerp(_posXBlend, Managers.Input.Move.x, Time.deltaTime * SpeedChangeRate);
        _posZBlend = Mathf.Lerp(_posZBlend, Managers.Input.Move.y, Time.deltaTime * SpeedChangeRate);
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f)
        {
            _animationBlend = 0f;
        }

        var inputDirection = new Vector3(Managers.Input.Move.x, 0.0f, Managers.Input.Move.y).normalized;

        if (Managers.Input.Move != Vector2.zero)
        {
            _lockOffRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        }

        if (Player.Camera.IsLockOn)
        {
            if (!_isPrevLockOn)
            {
                _lockOnRotation = _lockOffRotation;
                _isPrevLockOn = true;
            }

            if (!Managers.Input.Sprint && !IsJumping)
            {
                if (Managers.Input.Move != Vector2.zero)
                {
                    Vector3 dir = Player.Camera.LockOnTarget.transform.position - transform.position;
                    _lockOnRotation = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                }
            }
            else
            {
                _lockOnRotation = _lockOffRotation;
            }
        }
        else
        {
            if (_isPrevLockOn)
            {
                _lockOffRotation = _lockOnRotation;
                _isPrevLockOn = false;
            }
        }

        // 회전.
        float finalTargetRotation = Player.Camera.IsLockOn ? _lockOnRotation : _lockOffRotation;
        var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, finalTargetRotation, ref _rotationVelocity, RotationSmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        // 움직임.
        var targetDirection = Quaternion.Euler(0.0f, _lockOffRotation, 0.0f) * Vector3.forward;
        _controller.Move((targetDirection.normalized * _speed + new Vector3(0.0f, _verticalVelocity, 0.0f)) * Time.deltaTime);

        // 애니메이터 업데이트.
        Player.Animator.SetFloat(_animIDPosX, _posXBlend);
        Player.Animator.SetFloat(_animIDPosZ, _posZBlend);
        Player.Animator.SetFloat(_animIDSpeed, _animationBlend);
        Player.Animator.SetFloat(_animIDMotionSpeed, _animationBlend);
    }

    private void OnBeginJumpLand()
    {
        _isJumpLand = true;
    }

    private void OnEndJumpLand()
    {
        _isJumpLand = false;
        IsJumping = false;
    }
}
