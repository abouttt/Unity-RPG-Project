using UnityEngine;
using Newtonsoft.Json.Linq;

public class PlayerMovement : MonoBehaviour, ISavable
{
    public static string SaveKey => "SaveTransform";

    public bool CanMove { get; set; } = true;
    public bool CanRotation { get; set; } = true;
    public bool CanSprint { get; set; } = true;
    public bool CanJump { get; set; } = true;
    public bool CanRoll { get; set; } = true;

    public bool IsGrounded { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsRolling { get; private set; }

    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _sprintSpeed;

    [SerializeField]
    private float _rotationSmoothTime;

    [SerializeField]
    private float _speedChangeRate;

    [SerializeField]
    private float _jumpLandDecreaseSpeedPercentage;

    [SerializeField]
    private float _rollTimeout;

    [Space(10)]
    [SerializeField]
    private float _jumpHeight;

    [SerializeField]
    private float _gravity;

    [SerializeField]
    private float _jumpTimeout;

    [SerializeField]
    private float _fallTimeout;

    [Space(10)]
    [SerializeField]
    private float _requiredSprintSP;

    [SerializeField]
    private float _requiredJumpSP;

    [SerializeField]
    private float _requiredRollSP;

    [Space(10)]
    [SerializeField]
    private float _groundedOffset;

    [SerializeField]
    private float _groundedRadius;

    [SerializeField]
    private LayerMask _groundLayers;

    private float _speed;
    private float _animationBlend;
    private float _posXBlend;
    private float _posZBlend;
    private float _targetRotation;
    private float _cameraTargetRotation;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private bool _isJumpLand;
    private bool _isJumpWithSprint;

    private float _sprintInputDeltaTime;
    private bool _isPressedSprint;

    private bool _isPrevLockOn;

    // animation IDs
    private readonly int _animIDPosX = Animator.StringToHash("PosX");
    private readonly int _animIDPosZ = Animator.StringToHash("PosZ");
    private readonly int _animIDSpeed = Animator.StringToHash("Speed");
    private readonly int _animIDGrounded = Animator.StringToHash("Grounded");
    private readonly int _animIDJump = Animator.StringToHash("Jump");
    private readonly int _animIDFreeFall = Animator.StringToHash("FreeFall");
    private readonly int _animIDRoll = Animator.StringToHash("Roll");

    private CharacterController _controller;
    private GameObject _mainCamera;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _mainCamera = Camera.main.gameObject;
    }

    private void Start()
    {
        _targetRotation = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        JumpAndGravity(deltaTime);
        GroundedCheck();
        Roll();
        Move(deltaTime);
    }

    public void ClearJump()
    {
        IsJumping = false;
        _isJumpLand = false;
        _isJumpWithSprint = false;
    }

    public void ClearRoll()
    {
        IsRolling = false;
        Player.Animator.SetBool(_animIDRoll, false);
    }

    public JArray CreateSaveData()
    {
        var saveData = new JArray();

        var transformSaveData = new TransformSaveData()
        {
            X = transform.position.x,
            Y = transform.position.y,
            Z = transform.position.z,
            RotationYaw = transform.eulerAngles.y,
        };

        saveData.Add(JObject.FromObject(transformSaveData));

        return saveData;
    }

    public void LoadSaveData()
    {
    }

    private void JumpAndGravity(float deltaTime)
    {
        if (IsGrounded)
        {
            // �߶� ���ѽð� ����
            _fallTimeoutDelta = _fallTimeout;

            Player.Animator.SetBool(_animIDJump, false);
            Player.Animator.SetBool(_animIDFreeFall, false);

            // �������� �� �ӵ��� ������ �������� ���� ����
            if (_verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            // ����
            if (Managers.Input.Jump && CanJump && _jumpTimeoutDelta <= 0f)
            {
                if (Player.Status.SP > 0f)
                {
                    Player.Status.SP -= _requiredJumpSP;

                    IsJumping = true;
                    // ���ϴ� ���̿� �����ϴ� �� �ʿ��� �ӵ� = H * -2 * G �� ������.
                    _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                    _isJumpWithSprint = Managers.Input.Sprint;
                    Player.Animator.SetBool(_animIDJump, true);
                }
            }

            // ���� ���ѽð�
            if (_jumpTimeoutDelta >= 0f)
            {
                _jumpTimeoutDelta -= deltaTime;
            }
        }
        else
        {
            // ���� ���ѽð� ����
            _jumpTimeoutDelta = _jumpTimeout;

            // �߶� ���ѽð�
            if (_fallTimeoutDelta >= 0f)
            {
                _fallTimeoutDelta -= deltaTime;
            }
            else
            {
                Player.Animator.SetBool(_animIDFreeFall, true);
            }
        }

        // �͹̳� �Ʒ��� �ִ� ��� �ð��� ���� �߷��� ����
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += _gravity * deltaTime;
        }
    }

    private void GroundedCheck()
    {
        // �������� ����Ͽ� �� ��ġ ����
        var spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        IsGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
        Player.Animator.SetBool(_animIDGrounded, IsGrounded);
    }

    private void Roll()
    {
        if (!Managers.Input.Sprint && CanRoll && _isPressedSprint && _sprintInputDeltaTime < _rollTimeout)
        {
            if (Player.Status.SP > 0f)
            {
                CanRotation = true;
                Player.Animator.SetBool(_animIDRoll, true);
            }
        }
    }

    private void Move(float deltaTime)
    {
        float targetSpeed = _moveSpeed;
        float requiredSP = 0f;
        bool isZeroMoveInput = Managers.Input.Move == Vector2.zero;

        if (!Managers.Input.Sprint && !CanSprint)
        {
            CanSprint = true;
        }

        // ����, ������Ű�� �����Ƿ� �����ϱ� ����
        if (Managers.Input.Sprint)
        {
            _isPressedSprint = true;
            _sprintInputDeltaTime += deltaTime;
        }
        else if (_isPressedSprint)
        {
            IsSprinting = false;
            _isPressedSprint = false;
            _sprintInputDeltaTime = 0f;
        }

        if (_isJumpLand)
        {
            targetSpeed = _moveSpeed * _jumpLandDecreaseSpeedPercentage;
        }
        else if (IsJumping && _isJumpWithSprint)
        {
            targetSpeed = _sprintSpeed;
        }
        else if (_isPressedSprint && CanSprint && !IsJumping && !IsRolling && _sprintInputDeltaTime > _rollTimeout)
        {
            if (Player.Status.SP > 0f)
            {
                IsSprinting = true;
                targetSpeed = _sprintSpeed;
                requiredSP = _requiredSprintSP * deltaTime;
            }
            else
            {
                CanSprint = false;
            }
        }

        if (!IsRolling && (!CanMove || isZeroMoveInput))
        {
            targetSpeed = 0f;
            requiredSP = 0f;
        }

        // �÷��̾��� ���� ���� �ӵ��� ���� ����
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;
        float finalSpeedChangeRate = deltaTime * _speedChangeRate;
        float speedOffset = 0.1f;

        // ��ǥ �ӵ����� ������
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, finalSpeedChangeRate);

            // �ӵ��� �Ҽ��� ���� 3�ڸ����� �ݿø�
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, finalSpeedChangeRate);
        _posXBlend = Mathf.Lerp(_posXBlend, Managers.Input.Move.x, finalSpeedChangeRate);
        _posZBlend = Mathf.Lerp(_posZBlend, Managers.Input.Move.y, finalSpeedChangeRate);
        if (_animationBlend < 0.01f)
        {
            _animationBlend = 0f;
        }

        var inputDirection = new Vector3(Managers.Input.Move.x, 0f, Managers.Input.Move.y).normalized;

        if (CanRotation && !isZeroMoveInput)
        {
            _cameraTargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            _targetRotation = _cameraTargetRotation;
        }

        if (Player.Camera.IsLockOn)
        {
            if (!_isPrevLockOn)
            {
                _isPrevLockOn = true;
            }

            // ����, ����, ������� �� �½ÿ� ��ǲ �������� ȸ���Ѵ�
            if (!IsSprinting && !IsJumping && !IsRolling)
            {
                if (!isZeroMoveInput)
                {
                    // Ÿ�� �������� ���ϵ��� ȸ��
                    var dirToTarget = (Player.Camera.LockOnTarget.position - transform.position).normalized;
                    _targetRotation = Mathf.Atan2(dirToTarget.x, dirToTarget.z) * Mathf.Rad2Deg;
                }
            }
            else
            {
                // ������� ĳ���Ͱ� �ٶ󺸰� �ִ� �������� ������ ����
                if (!isZeroMoveInput)
                {
                    _targetRotation = _cameraTargetRotation;
                }
                else
                {
                    _cameraTargetRotation = _targetRotation;
                }
            }
        }
        else
        {
            if (_isPrevLockOn)
            {
                _cameraTargetRotation = _targetRotation;
                _isPrevLockOn = false;
            }
        }

        // ȸ��
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);

        // �̵�
        var targetDirection = Quaternion.Euler(0f, _cameraTargetRotation, 0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_speed * deltaTime) + new Vector3(0f, _verticalVelocity, 0f) * deltaTime);
        Player.Status.SP -= requiredSP;

        // �ִϸ����� ������Ʈ
        Player.Animator.SetFloat(_animIDSpeed, _animationBlend);
        Player.Animator.SetFloat(_animIDPosX, _posXBlend);
        Player.Animator.SetFloat(_animIDPosZ, _posZBlend);
    }

    private void OnBeginJumpLand()
    {
        _isJumpLand = true;
    }

    private void OnBeginRoll()
    {
        ClearJump();
        Player.Status.SP -= _requiredRollSP;
        IsRolling = true;
        CanMove = false;
        CanRotation = false;
        CanJump = false;
        CanRoll = false;
    }

    private void OnDrawGizmosSelected()
    {
        var spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        Gizmos.DrawWireSphere(spherePosition, _groundedRadius);
    }
}
