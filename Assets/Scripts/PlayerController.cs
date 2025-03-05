/* C# 中的 PlayerController 类管理玩家的移动、加速、跳跃和地面
使用各种参数和优化进行检测。*/
using Managers;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("人物移动参数")]
    [SerializeField] private float moveSpeed = 9f; // 移动速度（参考蔚蓝）
    [SerializeField] private float maxMoveSpeed = 10f; // 最大移动速度限制
    [SerializeField] private float newMass = 1f;// 质量

    [Header("人物加减速度")]
    [SerializeField] private float acceleration = 90f; // 加速度（调整）
    [SerializeField] private float deceleration = 60f; // 减速度（增加）

    [Header("速度曲线参数，空中控制系数，空气阻力")]
    [SerializeField] private float velocityPower = 0.9f; // 速度曲线指数
    [SerializeField] private float airControl = 0.6f; // 空中控制系数（减小）
    [SerializeField] private float airDrag = 0.4f; // 空气阻力（减小）
    private Vector2 _moveDirection; // 移动方向
    private float _lastMoveDirection; // 记录最后移动方向

    [Header("人物跳跃参数")]
    [SerializeField] private float jumpForce = 10f;// 跳跃力度（调整）
    [SerializeField] private float maxJumpHoldTime = 0.2f;// 最大跳跃按住时间（调整）
    [SerializeField] private float rayLength = 1.6f; // 射线长度
    [SerializeField] private Vector2 gravity;

    [Header("跳跃优化")]
    [SerializeField] private float coyoteTime = 0.1f; // 土狼时间（缩短）
    [SerializeField] private float jumpBuffer = 0.1f; // 跳跃缓冲（缩短）
    [SerializeField] private float fallMultiplier = 1.8f; // 下落加速度倍数
    [SerializeField] private float shortJumpMultiplier = 2.5f; // 短跳加速倍数（新增）
    [SerializeField] private float landingVFXTime = 0.15f; // 落地特效时间

    //[Header("未使用")]
    //[SerializeField] private float minJumpForce = 7f;// 最小跳跃力度
    //[SerializeField] private float preLandingTime = 0.15f; // 预落地时间

    [Header("地面检测")]
    [SerializeField] private LayerMask groundLayer;// 地面层

    private bool _isGrounded;// 是否在地面上
    private float _coyoteTimeCounter; // 土狼时间计数器
    private float _jumpBufferCounter; // 跳跃缓冲计数器
    private bool _hasBufferedJump; // 是否有缓冲的跳跃
    private float _jumpHoldTime; // 跳跃按住时间
    private bool _isJumping; // 是否正在跳跃
    private float _fallDistance; // 下落距离
    private float _lastGroundedY; // 上次着地Y位置
    private bool _isPreLanding; // 是否预落地
    private bool _isLanding; // 是否正在着地

    private Vector2 _direction; // 向量化后的方向
    private Rigidbody2D _rb2D; // 刚体组件
    private SpriteRenderer _spriteRenderer; // 精灵渲染器组件

    // 缓存射线检测结果
    private RaycastHit2D _groundHit;

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Physics2D.gravity = gravity;
    }

    private void Start()
    {
        _rb2D.mass = newMass; // 设置刚体质量
        _lastGroundedY = transform.position.y; // 初始化最后着地位置
        // 订阅跳跃事件
        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
        GameInput.Instance.OnInteractAction+=GameInput_OnInteractAction;
    }

    private void OnDestroy()
    {
        // 取消订阅以防止内存泄漏
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnJumpAction -= GameInput_OnJumpAction;
            GameInput.Instance.OnInteractAction-=GameInput_OnInteractAction;
        }
    }

    private void Update()
    {
        CheckGround(); // 检测地面
        UpdateTimers(); // 更新计时器

        // 处理跳跃按住时间
        if (_isJumping && GameInput.Instance.JumpPressed)
        {
            _jumpHoldTime += Time.deltaTime;
            if (_jumpHoldTime >= maxJumpHoldTime)
            {
                _isJumping = false;
            }
        }
        else if (_isJumping)
        {
            _isJumping = false;
        }

        // 处理着地效果
        if (_isLanding)
        {
            // 可以在这里添加着地动画或特效
            if (Time.time >= landingVFXTime)
            {
                _isLanding = false;
            }
        }
    }

    private void FixedUpdate()
    {
        HandleMovement(); // 处理移动
        ApplyFallMultiplier(); // 应用下落加速

        // 强制限制最大水平速度，防止角色飘浮
        _rb2D.linearVelocity = new Vector2(
            Mathf.Clamp(_rb2D.linearVelocity.x, -maxMoveSpeed, maxMoveSpeed),
            _rb2D.linearVelocity.y
        );
    }

    #region 移动
    private void HandleMovement()
    {
        _moveDirection = GameInput.Instance.moveDir;
        _direction = _moveDirection.normalized;

        if (_direction != Vector2.zero && CanMove())
        {
            _lastMoveDirection = _direction.x;

            // 计算目标速度
            float targetSpeed = _direction.x * moveSpeed;

            // 应用控制系数
            float controlModifier = _isGrounded ? 1f : airControl;

            // 获取当前水平速度
            float currentSpeed = _rb2D.linearVelocity.x;

            // 计算速度差距
            float speedDifference = targetSpeed - currentSpeed;

            // 选择合适的加速度
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

            // 应用加速度曲线
            float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelRate, velocityPower) * Mathf.Sign(speedDifference);
            movement *= controlModifier; // 应用空中/地面控制系数

            // 改用脉冲力模式，避免持续累积
            _rb2D.AddForce(Vector2.right * movement * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }
        else
        {
            // 停止移动时的减速 - 对地面和空中使用不同的减速度
            float friction = _isGrounded ? deceleration : (airDrag * deceleration);
            Vector2 frictionForce = -_rb2D.linearVelocity.x * friction * Vector2.right;
            _rb2D.AddForce(frictionForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }

        // 角色朝向 - 使用最后移动方向
        if (Mathf.Abs(_lastMoveDirection) > 0.1f)
        {
            _spriteRenderer.flipX = _lastMoveDirection > 0;
        }

        // 更新下落检测
        if (!_isGrounded && _rb2D.linearVelocity.y < 0)
        {
            _fallDistance = _lastGroundedY - transform.position.y;

            // 预落地检测 - 只有在下落距离较大时进行检测以减少开销
            if (!_isPreLanding && _fallDistance > 1f)
            {
                _groundHit = Physics2D.Raycast(transform.position, Vector2.down, rayLength * 3f, groundLayer);
                if (_groundHit.collider != null)
                {
                    _isPreLanding = true;
                    // 这里可以触发预落地动画
                }
            }
        }
    }

    private bool CanMove()
    {
        return true;
    }
    #endregion

    #region 跳跃

    private void GameInput_OnJumpAction(object sender, EventArgs e)
    {
        _jumpBufferCounter = jumpBuffer;
        TryJump();
    }

    private void UpdateTimers()
    {
        // 更新土狼时间
        if (!_isGrounded)
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        // 更新跳跃缓冲
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

    private void CheckGround()
    {
        // 从角色中心向下发射射线
        _groundHit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);

        // 更新地面状态
        bool wasGrounded = _isGrounded;
        _isGrounded = _groundHit.collider is not null;

        // 如果刚接触地面
        if (_isGrounded && !wasGrounded)
        {
            // 重置跳跃相关状态
            _hasBufferedJump = false;
            _coyoteTimeCounter = coyoteTime;

            // 处理着地效果
            if (_fallDistance > 1f)
            {
                _isLanding = true;
                // 这里可以添加着地音效或粒子效果
            }

            // 重置相关状态
            _isPreLanding = false;
            _fallDistance = 0;
            _lastGroundedY = transform.position.y;
        }
        // 如果刚离开地面
        else if (!_isGrounded && wasGrounded)
        {
            _coyoteTimeCounter = coyoteTime;
            _lastGroundedY = transform.position.y;
        }
    }

    private void TryJump()
    {
        // 添加垂直速度检查，确保不会在上升过程中再次跳跃
        if ((_isGrounded || _coyoteTimeCounter > 0) && _rb2D.linearVelocity.y <= 0.1f)
        {
            // 执行跳跃
            _isJumping = true;
            _jumpHoldTime = 0f;
            PerformJump(jumpForce);
            _coyoteTimeCounter = 0; // 确保清零土狼时间
        }
    }

    private void PerformJump(float force)
    {
        // 重置垂直速度，确保每次跳跃都从0开始
        _rb2D.linearVelocity = new Vector2(_rb2D.linearVelocity.x, 0f);

        // 应用跳跃力 - 直接使用力而非插值，更接近蔚蓝的感觉
        _rb2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        _hasBufferedJump = false;
        _jumpBufferCounter = 0;
    }

    private void ApplyFallMultiplier()
    {
        if (!_isGrounded) // 只在非地面状态应用
        {
            // 在下落时应用更大的重力
            if (_rb2D.linearVelocity.y < 0)
            {
                // 确保这是一个向下的力
                float fallForce = fallMultiplier - 1;
                _rb2D.linearVelocity += Vector2.up * (gravity * fallForce * Time.fixedDeltaTime);
            }
            // 短跳（当玩家释放跳跃键时）
            else if (_rb2D.linearVelocity.y > 0 && !GameInput.Instance.JumpPressed)
            {
                // 应用更大的向下力量
                float shortJumpForce = shortJumpMultiplier - 1;
                _rb2D.linearVelocity += Vector2.up * (gravity * shortJumpForce * Time.fixedDeltaTime);
            }
        }
    }
    #endregion



    #region 互动
    private void GameInput_OnInteractAction(object sender,EventArgs e)
    {
        //TODO-互动功能

    }


    #endregion
}