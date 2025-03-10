using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

/// <summary>
///     玩家角色控制器
///     主要功能：
///     1. 平滑的移动和跳跃系统，包括加速度和空气阻力
///     2. 优化的跳跃手感，包括土狼时间和跳跃缓冲
///     3. 可交互物体的高亮和选择系统
///     4. 点击移动和自动寻路功能
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region 序列化字段

    [Header("人物移动参数")] [Tooltip("基础移动速度")] [SerializeField]
    private float moveSpeed = 9f; // 基础移动速度

    [Tooltip("最大移动速度")] [SerializeField] private float maxMoveSpeed = 10f; // 最大移动速度

    [Tooltip("新的质量")] [SerializeField] private float newMass = 1f; // 角色质量

    [Tooltip("加速度")] [SerializeField] private float acceleration = 90f; // 加速度

    [Tooltip("减速度")] [SerializeField] private float deceleration = 60f; // 减速度

    [Tooltip("速度曲线指数")] [SerializeField] private float velocityPower = 0.9f; // 速度曲线指数

    [Tooltip("空中控制系数")] [SerializeField] private float airControl = 0.6f; // 空中控制系数

    [Tooltip("空气阻力")] [SerializeField] private float airDrag = 0.4f; // 空气阻力

    [Tooltip("选择CheckGround层级")] [SerializeField]
    private LayerMask groundLayer; // 地面层

    [Tooltip("跳跃力度")] [SerializeField] private float jumpForce = 10f; // 跳跃力度

    [Tooltip("最大跳跃按住时间")] [SerializeField] private float maxJumpHoldTime = 0.2f; // 最大跳跃按住时间 

    [Tooltip("地面检测射线长度")] [SerializeField] private float rayLength = 1.6f; // 地面检测射线长度

    [Tooltip("重力")] [SerializeField] private Vector2 gravity; // 重力

    [Tooltip("土狼时间")] [SerializeField] private float coyoteTime = 0.1f; // 土狼时间

    [Tooltip("跳跃缓冲时间")] [SerializeField] private float jumpBuffer = 0.1f; // 跳跃缓冲时间

    [Tooltip("下落加速倍数")] [SerializeField] private float fallMultiplier = 1.8f; // 下落加速倍数

    [Tooltip("短跳加速倍数")] [SerializeField] private float shortJumpMultiplier = 2.5f; // 短跳加速倍数

    [Tooltip("特效持续事件")] [SerializeField] private float landingVFXTime = 0.15f; // 落地特效持续时间

    [Tooltip("鼠标选择物体范围")] [SerializeField] private float selectRadius = 1f; // 物体选择范围

    #endregion

    #region 私有字段

    // 交互相关
    private readonly List<TriggerObject> _triggerObjects = new();
    private TriggerObject _clickedTriggerObject;
    private bool _isMovingToTarget;
    private Vector2 _targetPosition;

    // 移动相关
    private Vector2 _moveDirection;
    private float _lastMoveDirection;
    private bool _isGrounded;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private bool _hasBufferedJump;
    private float _jumpHoldTime;
    private bool _isJumping;
    private float _fallDistance;
    private float _lastGroundedY;
    private bool _isPreLanding;
    private bool _isLanding;
    private Vector2 _direction;

    // 组件引用
    private Rigidbody2D _rb2D;
    private SpriteRenderer _spriteRenderer;
    private RaycastHit2D _groundHit;
    private Vector2 _mousePosition;

    #endregion

    #region 公共属性

    [Header("公共属性")] public float overlapRadius = 0.5f;

    public static PlayerController Instance { get; private set; }
    public Camera cam;
    public TriggerObject SelectedObject { get; private set; }

    #endregion

    #region Unity生命周期

    /// <summary>
    ///     初始化组件引用和单例
    /// </summary>
    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Physics2D.gravity = gravity;
        cam = Camera.main;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    ///     初始化状态和注册事件
    /// </summary>
    private void Start()
    {
        _rb2D.mass = newMass;
        _lastGroundedY = transform.position.y;

        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnClickAction += GameInput_OnClickAction;
            GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
        }
    }

    /// <summary>
    ///     注销事件
    /// </summary>
    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnJumpAction -= GameInput_OnJumpAction;
            GameInput.Instance.OnClickAction -= GameInput_OnClickAction;
        }
    }

    /// <summary>
    ///     每帧更新状态和输入
    /// </summary>
    private void Update()
    {
        CheckGround();
        UpdateTimers();

        _mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        UpdateSelectedObject();

        HandleJumpingState();
        HandleMovementState();
    }

    /// <summary>
    ///     物理更新
    /// </summary>
    private void FixedUpdate()
    {
        HandleMovement();
        ApplyFallMultiplier();

        _rb2D.linearVelocity = new Vector2(
            Mathf.Clamp(_rb2D.linearVelocity.x, -maxMoveSpeed, maxMoveSpeed),
            _rb2D.linearVelocity.y
        );
    }

    #endregion

    #region 移动系统

    /// <summary>
    ///     处理角色移动
    ///     包含加速度、空气阻力等计算
    ///     在FixedUpdate中调用
    /// </summary>
    private void HandleMovement()
    {
        if (!_isMovingToTarget && GameInput.Instance != null) _moveDirection = GameInput.Instance.moveDir;
        _direction = _moveDirection.normalized;

        if (_direction != Vector2.zero && CanMove())
            ApplyMovementForce();
        else
            ApplyFriction();

        UpdateSpriteDirection();
        HandleFalling();
    }

    /// <summary>
    ///     应用移动力
    ///     使用加速度和速度曲线计算
    ///     在HandleMovement中调用
    /// </summary>
    private void ApplyMovementForce()
    {
        _lastMoveDirection = _direction.x;
        var targetSpeed = _direction.x * moveSpeed;
        var controlModifier = _isGrounded ? 1f : airControl;
        var currentSpeed = _rb2D.linearVelocity.x;
        var speedDifference = targetSpeed - currentSpeed;
        var accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        var movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelRate, velocityPower) * Mathf.Sign(speedDifference);
        movement *= controlModifier;

        _rb2D.AddForce(Vector2.right * (movement * Time.fixedDeltaTime), ForceMode2D.Impulse);
    }

    /// <summary>
    ///     应用摩擦力
    ///     停止移动时减速
    ///     在HandleMovement中调用
    /// </summary>
    private void ApplyFriction()
    {
        var friction = _isGrounded ? deceleration : airDrag * deceleration;
        var frictionForce = -_rb2D.linearVelocity.x * friction * Vector2.right;
        _rb2D.AddForce(frictionForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }

    /// <summary>
    ///     处理移动状态
    ///     包括自动移动和手动移动的切换
    ///     在Update中调用
    /// </summary>
    private void HandleMovementState()
    {
        if (_isMovingToTarget && _clickedTriggerObject != null)
            HandleTargetMovement();
        else if (GameInput.Instance != null) _moveDirection = GameInput.Instance.moveDir;
    }

    /// <summary>
    ///     处理目标移动
    ///     点击物体后自动移动到目标位置
    ///     在HandleMovementState中调用
    /// </summary>
    private void HandleTargetMovement()
    {
        var directionX = Mathf.Sign(_targetPosition.x - transform.position.x);
        var autoMoveDirection = new Vector2(directionX, 0);

        if (Mathf.Abs(transform.position.x - _targetPosition.x) < 0.5f)
        {
            _isMovingToTarget = false;
            autoMoveDirection = Vector2.zero;

            if (_clickedTriggerObject != null &&
                Vector2.Distance(transform.position, _clickedTriggerObject.transform.position) <=
                _clickedTriggerObject.InteractionRange)
                _clickedTriggerObject.Interact();
        }

        if (GameInput.Instance != null && Mathf.Abs(GameInput.Instance.moveDir.x) > 0.1f)
        {
            _isMovingToTarget = false;
            _clickedTriggerObject = null;
            _moveDirection = GameInput.Instance.moveDir;
        }
        else
        {
            _moveDirection = autoMoveDirection;
        }
    }

    #endregion

    #region 跳跃系统

    /// <summary>
    ///     跳跃事件处理
    ///     当按下跳跃键时触发
    ///     由GameInput事件系统调用
    /// </summary>
    private void GameInput_OnJumpAction(object sender, EventArgs e)
    {
        _jumpBufferCounter = jumpBuffer;
        TryJump();
    }

    /// <summary>
    ///     尝试执行跳跃
    ///     检查跳跃条件并执行跳跃
    ///     在GameInput_OnJumpAction和UpdateTimers中调用
    /// </summary>
    private void TryJump()
    {
        if ((_isGrounded || _coyoteTimeCounter > 0) && _rb2D.linearVelocity.y <= 0.1f)
        {
            _isJumping = true;
            _jumpHoldTime = 0f;
            PerformJump(jumpForce);
            _coyoteTimeCounter = 0;
        }
    }

    /// <summary>
    ///     执行跳跃
    ///     应用跳跃力
    ///     在TryJump中调用
    /// </summary>
    private void PerformJump(float force)
    {
        _rb2D.linearVelocity = new Vector2(_rb2D.linearVelocity.x, 0f);
        _rb2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        _hasBufferedJump = false;
        _jumpBufferCounter = 0;
    }

    /// <summary>
    ///     处理跳跃状态
    ///     控制跳跃高度和落地状态
    ///     在Update中调用
    /// </summary>
    private void HandleJumpingState()
    {
        if (_isJumping && GameInput.Instance != null && GameInput.Instance.JumpPressed)
        {
            _jumpHoldTime += Time.deltaTime;
            if (_jumpHoldTime >= maxJumpHoldTime) _isJumping = false;
        }
        else if (_isJumping)
        {
            _isJumping = false;
        }

        if (_isLanding && Time.time >= landingVFXTime) _isLanding = false;
    }

    #endregion

    #region 交互系统

    /// <summary>
    ///     点击事件处理
    ///     检测点击的物体并处理交互
    ///     由GameInput事件系统调用
    /// </summary>
    private void GameInput_OnClickAction(object sender, EventArgs e)
    {
        var hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            var clickedObject = hit.collider.GetComponent<TriggerObject>();

            if (clickedObject != null && clickedObject.CanInteract) HandleClickedObject(clickedObject);
        }
    }

    /// <summary>
    ///     处理点击的物体
    ///     设置移动目标和交互状态
    ///     在GameInput_OnClickAction中调用
    /// </summary>
    private void HandleClickedObject(TriggerObject clickedObject)
    {
        var targetPosition = new Vector2(
            clickedObject.transform.position.x,
            transform.position.y
        );

        if (clickedObject == _clickedTriggerObject && _triggerObjects.Contains(clickedObject))
        {
            if (Vector2.Distance(transform.position, clickedObject.transform.position) <=
                clickedObject.InteractionRange)
            {
                clickedObject.Interact();
                _clickedTriggerObject = null;
                _isMovingToTarget = false;
                _moveDirection = Vector2.zero;
            }
        }
        else
        {
            _clickedTriggerObject = clickedObject;
            _targetPosition = targetPosition;
            _isMovingToTarget = true;
        }
    }

    /// <summary>
    ///     更新选中的物体
    ///     检测鼠标指向的最近可交互物体
    ///     在Update中调用
    /// </summary>
    private void UpdateSelectedObject()
    {
        TriggerObject nearestObject = null;
        var closestDistance = float.MaxValue;

        foreach (var obj in _triggerObjects)
        {
            if (!obj.CanInteract) continue;

            var distance = Vector2.Distance(_mousePosition, obj.transform.position);
            if (distance < selectRadius && distance < closestDistance)
            {
                nearestObject = obj;
                closestDistance = distance;
            }
        }

        if (SelectedObject != nearestObject)
        {
            if (SelectedObject != null) SelectedObject.OnDeselected();

            SelectedObject = nearestObject;

            if (SelectedObject != null) SelectedObject.OnSelected();

            EventManager.Instance?.TriggerObjectSelected(SelectedObject);
        }
    }

    #endregion

    #region 地面检测

    /// <summary>
    ///     检查地面状态
    ///     使用射线检测判断是否着地
    ///     在Update中调用
    /// </summary>
    private void CheckGround()
    {
        _groundHit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);
        var wasGrounded = _isGrounded;
        _isGrounded = _groundHit.collider != null;

        if (_isGrounded && !wasGrounded)
            HandleLanding();
        else if (!_isGrounded && wasGrounded) HandleTakeoff();
    }

    /// <summary>
    ///     处理落地
    ///     设置落地状态和效果
    ///     在CheckGround中调用
    /// </summary>
    private void HandleLanding()
    {
        _hasBufferedJump = false;
        _coyoteTimeCounter = coyoteTime;

        if (_fallDistance > 1f) _isLanding = true;

        _isPreLanding = false;
        _fallDistance = 0;
        _lastGroundedY = transform.position.y;
    }

    /// <summary>
    ///     处理起跳
    ///     设置土狼时间和记录起跳高度
    ///     在CheckGround中调用
    /// </summary>
    private void HandleTakeoff()
    {
        _coyoteTimeCounter = coyoteTime;
        _lastGroundedY = transform.position.y;
    }

    #endregion

    #region 辅助方法

    /// <summary>
    ///     更新计时器
    ///     处理跳跃缓冲和土狼时间
    ///     在Update中调用
    /// </summary>
    private void UpdateTimers()
    {
        if (!_isGrounded) _coyoteTimeCounter -= Time.deltaTime;

        if (_jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
            if (!_hasBufferedJump && (_isGrounded || _coyoteTimeCounter > 0))
            {
                _hasBufferedJump = true;
                TryJump();
            }
        }
    }

    /// <summary>
    ///     更新精灵朝向
    ///     根据移动方向翻转角色
    ///     在HandleMovement中调用
    /// </summary>
    private void UpdateSpriteDirection()
    {
        if (Mathf.Abs(_moveDirection.x) > 0.1f)
        {
            // 移动时根据移动方向朝向
            _spriteRenderer.flipX = _moveDirection.x > 0;
        }
        else
        {
            // 静止时根据鼠标位置朝向
            var lookDirection = _mousePosition - (Vector2)transform.position;
            _spriteRenderer.flipX = lookDirection.x > 0;
        }
    }

    /// <summary>
    ///     处理下落
    ///     检测预落地状态
    ///     在HandleMovement中调用
    /// </summary>
    private void HandleFalling()
    {
        if (!_isGrounded && _rb2D.linearVelocity.y < 0)
        {
            _fallDistance = _lastGroundedY - transform.position.y;

            if (!_isPreLanding && _fallDistance > 1f)
            {
                _groundHit = Physics2D.Raycast(transform.position, Vector2.down, rayLength * 3f, groundLayer);
                if (_groundHit.collider != null) _isPreLanding = true;
            }
        }
    }

    /// <summary>
    ///     应用下落倍增器
    ///     使下落和短跳感更好
    ///     在FixedUpdate中调用
    /// </summary>
    private void ApplyFallMultiplier()
    {
        if (!_isGrounded)
        {
            if (_rb2D.linearVelocity.y < 0)
            {
                var fallForce = fallMultiplier - 1;
                _rb2D.linearVelocity += Vector2.up * (gravity.y * (fallForce * Time.fixedDeltaTime));
            }
            else if (GameInput.Instance != null && _rb2D.linearVelocity.y > 0 && !GameInput.Instance.JumpPressed)
            {
                var shortJumpForce = shortJumpMultiplier - 1;
                _rb2D.linearVelocity += Vector2.up * (gravity.y * (shortJumpForce * Time.fixedDeltaTime));
            }
        }
    }

    /// <summary>
    ///     是否可以移动
    ///     可以在此添加额外的移动限制条件
    ///     在HandleMovement中调用
    /// </summary>
    private bool CanMove()
    {
        return true;
    }

    #endregion

    #region 触发器方法

    /// <summary>
    ///     进入触发区域
    ///     添加可交互物体
    ///     由Unity触发器系统调用
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<TriggerObject>(out var triggerObject)) _triggerObjects.Add(triggerObject);
    }

    /// <summary>
    ///     离开触发区域
    ///     移除可交互物体并取消选中
    ///     由Unity触发器系统调用
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<TriggerObject>(out var triggerObject))
        {
            _triggerObjects.Remove(triggerObject);
            if (SelectedObject == triggerObject)
            {
                SelectedObject = null;
                EventManager.Instance?.TriggerObjectSelected(null);
            }
        }
    }

    #endregion
}