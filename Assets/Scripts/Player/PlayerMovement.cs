using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;                         // �⺻ �ӵ�
    public float SprintSpeed;                       // �޸��� �ӵ�
    public float RotationSmoothTime;                // ȸ�� �ӵ�
    public float SpeedChangeRate;                   // ���� �� ����
    public float JumpLandDecreaseSpeedPercentage;   // ���� ������ �ӵ� ���Ұ�

    [Space(10)]
    public float JumpHeight;                        // ���� ����
    public float Gravity;                           // ��ü �߷� ��

    [Space(10)]
    public float JumpTimeout;                       // �ٽ� �����ϱ���� �ð�
    public float FallTimeout;                       // ���� ���¿� ���Ա��� �ð�

    [Space(10)]
    public float GroundedOffset;                    // �� üũ ��ġ
    public float GroundedRadius;                    // �� üũ ��ü ������
    public LayerMask GroundLayers;                  // �� ���̾�

    public bool IsJumping { get; private set; }

    // ���� ���� ��
    private float _speed;
    private float _posXBlend;
    private float _posZBlend;
    private float _animationBlend;
    private float _lockOffRotation = 0f;
    private float _lockOnRotation = 0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private const float _terminalVelocity = 53.0f;

    // Ÿ�Ӿƿ� ��ŸŸ��
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

            // �����Ǿ��� �� �ӵ��� ������ �������� ���� ����.
            if (_verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            // ����
            if (Managers.Input.Jump && _jumpTimeoutDelta <= 0f)
            {
                // ���ϴ� ���̿� �����ϴ� �� �ʿ��� �ӵ� = H * -2 * G �� ������.
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

        // �߷�
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

        // ��ǥ �ӵ��� ���� �Ǵ� ����.
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
            // �Ҽ��� ���� 3�ڸ������� �ݿø� �ӵ�.
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

        // ȸ��.
        float finalTargetRotation = Player.Camera.IsLockOn ? _lockOnRotation : _lockOffRotation;
        var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, finalTargetRotation, ref _rotationVelocity, RotationSmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        // ������.
        var targetDirection = Quaternion.Euler(0.0f, _lockOffRotation, 0.0f) * Vector3.forward;
        _controller.Move((targetDirection.normalized * _speed + new Vector3(0.0f, _verticalVelocity, 0.0f)) * Time.deltaTime);

        // �ִϸ����� ������Ʈ.
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
