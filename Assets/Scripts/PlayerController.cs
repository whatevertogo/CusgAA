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
    private Vector2 moveDir; // 移动方向
    private float lastMoveDir; // 记录最后移动方向

    [Header("人物跳跃参数")]
    [SerializeField] private float jumpForce = 13f;// 跳跃力度（调整）
    [SerializeField] private float minJumpForce = 7f;// 最小跳跃力度
    [SerializeField] private float maxJumpHoldTime = 0.2f;// 最大跳跃按住时间（调整）
    [SerializeField] private float rayLength = 0.55f; // 射线长度
    [SerializeField] private float gravity;

    [Header("跳跃优化")]
    [SerializeField] private float coyoteTime = 0.1f; // 土狼时间（缩短）
    [SerializeField] private float jumpBuffer = 0.1f; // 跳跃缓冲（缩短）
    [SerializeField] private float fallMultiplier = 1.8f; // 下落加速度倍数
    [SerializeField] private float shortJumpMultiplier = 2.5f; // 短跳加速倍数（新增）

    [Header("未使用")]
    //[SerializeField] private float preLandingTime = 0.15f; // 预落地时间
    //[SerializeField] private float landingVFXTime = 0.15f; // 落地特效时间

    [Header("地面检测")]
    [SerializeField] private LayerMask groundLayer;// 地面层

    private bool isGrounded;// 是否在地面上
    private float coyoteTimeCounter; // 土狼时间计数器
    private float jumpBufferCounter; // 跳跃缓冲计数器
    private bool hasBufferedJump; // 是否有缓冲的跳跃
    private float jumpHoldTime; // 跳跃按住时间
    private bool isJumping; // 是否正在跳跃
    private float fallDistance; // 下落距离
    private float lastGroundedY; // 上次着地Y位置
    private bool isPreLanding; // 是否预落地
    private bool isLanding; // 是否正在着地
    
    private Vector2 direction; // 向量化后的方向
    private Rigidbody2D _rb2D; // 刚体组件
    private SpriteRenderer spriteRender; // 精灵渲染器组件
    
    // 缓存射线检测结果
    private RaycastHit2D groundHit;
    // 缓存上一帧位置，用于调试
    private Vector3 lastPosition;

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        spriteRender = GetComponent<SpriteRenderer>();
        gravity=Physics2D.gravity.y;
    }

    private void Start()
    {
        _rb2D.mass = newMass; // 设置刚体质量
        lastGroundedY = transform.position.y; // 初始化最后着地位置
        lastPosition = transform.position; // 初始化上一帧位置
        // 订阅跳跃事件
        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
    }

    private void OnDestroy()
    {
        // 取消订阅以防止内存泄漏
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnJumpAction -= GameInput_OnJumpAction;
        }
    }

    private void Update()
    {
        CheckGround(); // 检测地面
        UpdateTimers(); // 更新计时器
        
        // 处理跳跃按住时间
        if (isJumping && GameInput.Instance.JumpPressed)
        {
            jumpHoldTime += Time.deltaTime;
            if (jumpHoldTime >= maxJumpHoldTime)
            {
                isJumping = false;
            }
        }
        else if (isJumping)
        {
            isJumping = false;
        }

        // 处理着地效果
        if (isLanding)
        {
            // 可以在这里添加着地动画或特效
            if (Time.time >= landingVFXTime)
            {
                isLanding = false;
            }
        }
        
        // 保存当前位置用于下一帧比较
        lastPosition = transform.position;
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
        moveDir = GameInput.Instance.moveDir;
        direction = moveDir.normalized;

        if (direction != Vector2.zero && CanMove())
        {
            lastMoveDir = direction.x;
            
            // 计算目标速度
            float targetSpeed = direction.x * moveSpeed;
            
            // 应用控制系数
            float controlModifier = isGrounded ? 1f : airControl;
            
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
            float friction = isGrounded ? deceleration : (airDrag * deceleration);
            Vector2 frictionForce = -_rb2D.linearVelocity.x * friction * Vector2.right;
            _rb2D.AddForce(frictionForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }

        // 角色朝向 - 使用最后移动方向
        if (Mathf.Abs(lastMoveDir) > 0.1f)
        {
            spriteRender.flipX = lastMoveDir > 0;
        }

        // 更新下落检测
        if (!isGrounded && _rb2D.linearVelocity.y < 0)
        {
            fallDistance = lastGroundedY - transform.position.y;
            
            // 预落地检测 - 只有在下落距离较大时进行检测以减少开销
            if (!isPreLanding && fallDistance > 1f)
            {
                groundHit = Physics2D.Raycast(transform.position, Vector2.down, rayLength * 3f, groundLayer);
                if (groundHit.collider != null)
                {
                    isPreLanding = true;
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
        jumpBufferCounter = jumpBuffer;
        TryJump();
    }

    private void UpdateTimers()
    {
        // 更新土狼时间
        if (!isGrounded)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // 更新跳跃缓冲
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
            if (!hasBufferedJump && (isGrounded || coyoteTimeCounter > 0))
            {
                hasBufferedJump = true;
                TryJump();
            }
        }
    }

    private void CheckGround()
    {
        // 从角色中心向下发射射线
        groundHit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);
        
        // 更新地面状态
        bool wasGrounded = isGrounded;
        isGrounded = groundHit.collider is not null;

        // 如果刚接触地面
        if (isGrounded && !wasGrounded)
        {
            // 重置跳跃相关状态
            hasBufferedJump = false;
            coyoteTimeCounter = coyoteTime;
            
            // 处理着地效果
            if (fallDistance > 1f)
            {
                isLanding = true;
                // 这里可以添加着地音效或粒子效果
            }
            
            // 重置相关状态
            isPreLanding = false;
            fallDistance = 0;
            lastGroundedY = transform.position.y;
        }
        // 如果刚离开地面
        else if (!isGrounded && wasGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            lastGroundedY = transform.position.y;
        }
    }

    private void TryJump()
    {
        // 添加垂直速度检查，确保不会在上升过程中再次跳跃
        if ((isGrounded || coyoteTimeCounter > 0) && _rb2D.linearVelocity.y <= 0.1f)
        {
            // 执行跳跃
            isJumping = true;
            jumpHoldTime = 0f;
            PerformJump(jumpForce);
            coyoteTimeCounter = 0; // 确保清零土狼时间
        }
    }

    private void PerformJump(float force)
    {
        // 重置垂直速度，确保每次跳跃都从0开始
        _rb2D.linearVelocity = new Vector2(_rb2D.linearVelocity.x, 0f); 
        
        // 应用跳跃力 - 直接使用力而非插值，更接近蔚蓝的感觉
        _rb2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        
        hasBufferedJump = false;
        jumpBufferCounter = 0;
    }

    private void ApplyFallMultiplier()
    {
        if (!isGrounded) // 只在非地面状态应用
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
}
