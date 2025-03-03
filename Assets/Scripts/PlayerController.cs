using Managers;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("人物移动参数")]
    [SerializeField] private float moveSpeed = 7f; // 移动速度
    [SerializeField] private float rotationSpeed = 8f; // 旋转速度
    [SerializeField] private float newMass = 1f;// 质量
    [Header("人物加减速度,速度曲线参数，空中控制系数，空气阻力")]
    [SerializeField] private float acceleration = 100f; // 加速度
    [SerializeField] private float deceleration = 15f; // 减速度
    [SerializeField] private float velocityPower = 0.96f; // 速度曲线指数
    [SerializeField] private float airControl = 0.8f; // 空中控制系数
    [SerializeField] private float airDrag = 0.85f; // 空气阻力
    private Vector2 moveDir; // 移动方向
    private float lastMoveDir; // 记录最后移动方向

    [Header("人物跳跃参数")]
    [SerializeField] private float jumpForce = 11f;// 跳跃力度
    [SerializeField] private float minJumpForce = 6f;// 最小跳跃力度
    [SerializeField] private float maxJumpHoldTime = 0.3f;// 最大跳跃按住时间
    [SerializeField] private int maxJumpCount = 2;// 最大跳跃次数
    [SerializeField] private float rayLength = 0.55f; // 射线长度
    [Header("跳跃优化")]
    [SerializeField] private float coyoteTime = 0.2f; // 土狼时间
    [SerializeField] private float jumpBuffer = 0.2f; // 跳跃缓冲
    [SerializeField] private float fallMultiplier = 2.2f; // 下落加速度倍数
    [Header("未使用")]
    [SerializeField] private float preLandingTime = 0.15f; // 预落地时间
    [SerializeField] private float landingVFXTime = 0.15f; // 落地特效时间

    [Header("地面检测")]
    [SerializeField] private LayerMask groundLayer;// 地面层

    private bool isGrounded;// 是否在地面上
    private int jumpCount;// 跳跃次数
    private bool canJumpAgain = true; // 控制多段跳跃
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

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        spriteRender = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _rb2D.mass = newMass; //设置刚体质量
        lastGroundedY = transform.position.y; // 初始化最后着地位置
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
    }

    private void FixedUpdate()
    {
        HandleMovement(); // 处理移动
        ApplyFallMultiplier(); // 应用下落加速
    }

    #region 移动
    private void HandleMovement()
    {
        moveDir = Managers.GameInput.Instance.moveDir;
        direction = moveDir.normalized;

        if (direction != Vector2.zero && CanMove())
        {
            lastMoveDir = direction.x;
            // 基础移动速度
            float targetSpeed = direction.x * moveSpeed;
            
            // 空中移动速度较慢
            if (!isGrounded)
            {
                targetSpeed *= airControl;
            }

            // 计算当前速度与目标速度的差值
            float speedDiff = targetSpeed - _rb2D.linearVelocity.x;
            
            // 应用加速度曲线
            float movement = Mathf.Pow(Mathf.Abs(speedDiff) * acceleration, velocityPower) * Mathf.Sign(speedDiff);
            
            // 应用水平力
            _rb2D.AddForce(Vector2.right * (movement * Time.fixedDeltaTime), ForceMode2D.Force);
        }
        else
        {
            // 停止移动时的减速
            float friction = isGrounded ? deceleration : (airDrag * deceleration);
            Vector2 frictionForce = -_rb2D.linearVelocity * friction;
            _rb2D.AddForce(frictionForce * Time.fixedDeltaTime, ForceMode2D.Force);
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
            
            // 预落地检测
            if (!isPreLanding && fallDistance > 1f)
            {
                RaycastHit2D preHit = Physics2D.Raycast(transform.position, Vector2.down, rayLength * 3f, groundLayer);
                if (preHit.collider != null)
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);
        
        // 更新地面状态
        bool wasGrounded = isGrounded;
        isGrounded = hit.collider is not null;

        // 如果刚接触地面
        if (isGrounded && !wasGrounded)
        {
            // 重置跳跃相关状态
            jumpCount = 0;
            canJumpAgain = true;
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
        if (isGrounded || coyoteTimeCounter > 0)
        {
            // 第一段跳跃
            isJumping = true;
            jumpHoldTime = 0f;
            PerformJump(jumpForce);
            jumpCount = 1;
            canJumpAgain = true;
            coyoteTimeCounter = 0;
        }
        else if (jumpCount < maxJumpCount && canJumpAgain)
        {
            // 二段跳跃
            isJumping = true;
            jumpHoldTime = 0f;
            PerformJump(jumpForce * 0.8f);
            jumpCount++;
            
            if (jumpCount >= maxJumpCount)
            {
                canJumpAgain = false;
            }
        }
    }

    private void PerformJump(float force)
    {
        _rb2D.linearVelocity = new Vector2(_rb2D.linearVelocity.x, 0f); // 重置垂直速度
        
        // 根据按住时间调整跳跃力度
        float finalForce = Mathf.Lerp(minJumpForce, force, jumpHoldTime / maxJumpHoldTime);
        _rb2D.AddForce(Vector2.up * finalForce, ForceMode2D.Impulse);
        
        hasBufferedJump = false;
        jumpBufferCounter = 0;
    }

    private void ApplyFallMultiplier()
    {
        if (_rb2D.linearVelocity.y < 0)
        {
            // 下落加速
            _rb2D.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime);
        }
        else if (_rb2D.linearVelocity.y > 0 && !GameInput.Instance.JumpPressed)
        {
            // 短跳
            _rb2D.linearVelocity += Vector2.up * (Physics2D.gravity.y * (0.5f) * Time.fixedDeltaTime);
        }
    }

    // 在Editor中显示射线检测范围
    private void OnDrawGizmos()
    {
        // 显示地面检测射线
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayLength);
        
        // 显示预落地检测射线
        if (!isGrounded && _rb2D != null && _rb2D.linearVelocity.y < 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (rayLength * 3f));
        }
    }
    #endregion
}
