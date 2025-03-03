using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    [Header("人物参数")] 
    [SerializeField] private float moveSpeed = 7f; // 移动速度
    [SerializeField] private float velocityXSmoothing = 0.1f; // 平滑速度
    [SerializeField] private float rotationSpeed = 800f; // 旋转速度
    [SerializeField] private float newMass = 1f; // 质量
    [SerializeField] private float jumpForce = 8f; // 跳跃力度

    [SerializeField] private Vector2 moveDir; // 移动方向

    private Vector2 direction; // 向量化后的方向
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb2D; // 刚体组件

    [Header("JumpCheck")] 
    [SerializeField] private Vector2 bottomOffset;
    [SerializeField] private float checkRaduis;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGround;

    protected override void Awake()
    {
        base.Awake();
        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _rb2D.mass = newMass; // 设置刚体质量
        Managers.GameInput.Instance.RegisterJumpAction(Leap);//注册
    }

    private void OnDestroy()
    {
        Managers.GameInput.Instance.UnregisterJumpAction(Leap);//注销
    }

    private void Update()
    {
        isGround = CheckIfGrounded();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    #region 移动

    private void HandleMovement()
    {
        moveDir = Managers.GameInput.Instance.moveDir;
        direction = moveDir.normalized;

        if (direction != Vector2.zero)
        {
            // 更新水平速度
            
            float targetSpeed = moveSpeed * direction.x; // 仅考虑水平方向
            float velocityX = Mathf.SmoothDamp(_rb2D.linearVelocity.x, targetSpeed, ref velocityXSmoothing, 0.1f);
            if (CanMove()) //原来的移动方法会覆盖重力
            {
                _rb2D.linearVelocity = new Vector2(velocityX, _rb2D.linearVelocity.y);
            }

            // 更新角色的朝向（可选，根据需要调整）
            if (moveDir.sqrMagnitude > 0.01f)
            {
                _spriteRenderer.flipX = moveDir.x > 0;
            }
        }
    }

    private void Leap(InputAction.CallbackContext context)
    {
        if (isGround)
        {
            // 执行跳跃逻辑
            _rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        //todo-其他的地面
    }

    private bool CanMove()
    {
        return true;
    }

    private bool CheckIfGrounded()//还是感觉画网格要好点
    {
        return Physics2D.OverlapCircle((Vector2)transform.position+bottomOffset, checkRaduis, groundLayer);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position+bottomOffset,checkRaduis);
    }

    #endregion
}
