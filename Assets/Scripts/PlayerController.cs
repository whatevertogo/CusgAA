using Managers;
using UnityEngine;
using System;

public class PlayerController : Singleton<PlayerController>
{
    [Header("人物移动参数")]
    [SerializeField] private float moveSpeed = 7f; // 移动速度
    [SerializeField] private float rotationSpeed = 8f; // 旋转速度
    [SerializeField] private float newMass = 1f;// 质量
    private Vector2 moveDir; // 移动方向

    [Header("人物跳跃参数")]
    [SerializeField] private float jumpForce = 10f;// 跳跃力度
    [SerializeField] private int maxJumpCount = 2;// 最大跳跃次数
    [SerializeField] private float jumpCooldown = 0.1f;// 跳跃冷却时间
    [SerializeField] private Transform groundCheckPoint;// 地面检测点
    [SerializeField] private LayerMask groundLayer;// 地面层
    [SerializeField] private float groundCheckRadius = 0.2f;// 地面检测半径
    [Tooltip("设置用来表示地面的Layer")]
    [SerializeField] private string groundLayerName = "Ground"; // 地面层的名称



    public bool isGrounded { get; private set; } = false;// 是否在地面上
    private int jumpCount = 0;// 跳跃次数
    private float lastJumpTime = 0f;// 上次跳跃时间
    private bool canJumpAgain = true; // 控制多段跳跃

    private Vector2 direction; // 向量化后的方向
    private Rigidbody2D _rb2D; // 刚体组件
    private SpriteRenderer spriteRender; // 精灵渲染器组件
    protected override void Awake()
    {
        base.Awake();
        _rb2D = GetComponent<Rigidbody2D>();
        spriteRender = GetComponent<SpriteRenderer>();

        // 如果没有设置地面检测点，创建一个
        if (groundCheckPoint == null)
        {
            GameObject checkPoint = new GameObject("GroundCheck");
            checkPoint.transform.parent = transform;
            checkPoint.transform.localPosition = new Vector3(0, -0.5f, 0); // 设置在角色脚底
            groundCheckPoint = checkPoint.transform;
            Debug.Log("已自动创建地面检测点");
        }

        // 自动设置地面层
        groundLayer = LayerMask.GetMask(groundLayerName);
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
        #region 跳跃实现
        // 检测是否在地面上
        CheckGrounded();
        // 处理跳跃逻辑
        HandleJump();
        #endregion

    }

    private void FixedUpdate()
    {
        HandleMovement();// 移动
    }

    #region 移动

    private void HandleMovement()
    {
        moveDir = Managers.GameInput.Instance.moveDir;
        direction = moveDir.normalized;

        if (direction != Vector2.zero && CanMove())
        {
            // 使用线性速度实现移动，保持垂直方向的速度不变
            Vector2 targetVelocity = new Vector2(direction.x * moveSpeed, _rb2D.linearVelocity.y);

            // 平滑过渡到目标速度
            _rb2D.linearVelocity = Vector2.Lerp(
                _rb2D.linearVelocity,
                targetVelocity,
                Time.fixedDeltaTime * 10f
            );

            // 如果需要角色面向移动方向，可以添加以下代码
            if (direction.x != 0)
            {
                if (direction.x > 0)
                {
                    spriteRender.flipX = true;
                }
                else
                {
                    spriteRender.flipX = false;
                }
            }
        }
        else
        {
            // 如果没有输入，逐渐减速到停止（仅水平方向）
            _rb2D.linearVelocity = new Vector2(
                Mathf.Lerp(_rb2D.linearVelocity.x, 0, Time.fixedDeltaTime * 8f),
                _rb2D.linearVelocity.y
            );
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
        // 尝试跳跃
        TryJump();
    }

    private void CheckGrounded()
    {
        // 使用OverlapCircle检测与地面层的接触
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        // 如果在地面上，重置跳跃状态
        if (isGrounded && _rb2D.linearVelocity.y <= 0.1f)
        {
            jumpCount = 0;
            canJumpAgain = true;
        }
    }

    private void HandleJump()
    {
        // 处理跳跃控制(更平滑的下落)
        if (!GameInput.Instance.JumpPressed && _rb2D.linearVelocity.y > 0)
        {
            // 当松开跳跃键时，减缓上升速度，实现可变跳跃高度
            _rb2D.linearVelocity = new Vector2(_rb2D.linearVelocity.x, _rb2D.linearVelocity.y * 0.6f);
        }
    }

    private void TryJump()
    {
        // 检查是否可以跳跃
        if (isGrounded)
        {
            // 第一段跳跃
            PerformJump(jumpForce);
            jumpCount = 1;
            canJumpAgain = true;
        }
        else if (jumpCount < maxJumpCount && canJumpAgain && Time.time >= lastJumpTime + jumpCooldown)
        {
            // 第二段跳跃(或多段跳跃)
            PerformJump(jumpForce * 0.8f);
            jumpCount++;

            // 如果达到最大跳跃次数，禁止再次跳跃直到落地
            if (jumpCount >= maxJumpCount)
            {
                canJumpAgain = false;
            }
        }
    }

    private void PerformJump(float force)
    {
        _rb2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        // 记录跳跃时间
        lastJumpTime = Time.time;
    }

    // 在Editor中显示地面检测范围及当前状态调试用 
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            // 根据是否触地显示不同颜色
            Gizmos.color = Application.isPlaying && isGrounded ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);

            // 显示检测范围的实心部分
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.2f);
            Gizmos.DrawSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }

    #endregion
}