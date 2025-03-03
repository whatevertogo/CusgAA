using Managers;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("人物移动参数")]
    [SerializeField] private float moveSpeed = 7f; // 移动速度
    [SerializeField] private float rotationSpeed = 8f; // 旋转速度
    [SerializeField] private float newMass = 1f;// 质量
    [SerializeField] private float acceleration = 50f; // 加速度
    [SerializeField] private float airControl = 0.8f; // 空中控制系数
    private Vector2 moveDir; // 移动方向

    [Header("人物跳跃参数")]
    [SerializeField] private float jumpForce = 10f;// 跳跃力度
    [SerializeField] private int maxJumpCount = 2;// 最大跳跃次数
    [SerializeField] private float rayLength = 0.6f; // 射线长度
    [Header("跳跃优化")]
    [SerializeField] private float coyoteTime = 0.15f; // 土狼时间
    [SerializeField] private float jumpBuffer = 0.15f; // 跳跃缓冲
    [SerializeField] private float fallMultiplier = 3f; // 下落加速度倍数

    [Header("地面检测")]
    [SerializeField] private LayerMask groundLayer;// 地面层

    private bool isGrounded;// 是否在地面上
    private int jumpCount;// 跳跃次数
    private bool canJumpAgain = true; // 控制多段跳跃
    private float coyoteTimeCounter; // 土狼时间计数器
    private float jumpBufferCounter; // 跳跃缓冲计数器
    private bool hasBufferedJump; // 是否有缓冲的跳跃
    
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
            // 基础移动速度
            float targetSpeed = direction.x * moveSpeed;
            
            // 空中移动速度较慢
            if (!isGrounded)
            {
                targetSpeed *= airControl;
            }

            // 计算速度差
            float speedDiff = targetSpeed - _rb2D.linearVelocity.x;
            // 计算加速度
            float movement = speedDiff * acceleration;

            // 应用水平力
            _rb2D.AddForce(Vector2.right * (movement * Time.fixedDeltaTime), ForceMode2D.Force);

            // 角色朝向
            if (direction.x != 0)
            {
                spriteRender.flipX = direction.x > 0;
            }
        }
        else if (isGrounded)
        {
            // 在地面上时快速停止
            float friction = 0.7f;
            _rb2D.linearVelocity = new Vector2(_rb2D.linearVelocity.x * friction, _rb2D.linearVelocity.y);
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

    /// <summary>
    /// 使用射线检测地面
    /// </summary>
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
        }
        // 如果刚离开地面
        else if (!isGrounded && wasGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
    }

    private void TryJump()
    {
        if (isGrounded || coyoteTimeCounter > 0)
        {
            // 第一段跳跃
            PerformJump(jumpForce);
            jumpCount = 1;
            canJumpAgain = true;
            coyoteTimeCounter = 0;
        }
        else if (jumpCount < maxJumpCount && canJumpAgain)
        {
            // 二段跳跃
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
        _rb2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);
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
    }
    #endregion
}
