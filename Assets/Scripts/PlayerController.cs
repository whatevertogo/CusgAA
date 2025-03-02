using Managers;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [Header("人物参数")] [SerializeField] private float moveSpeed = 7f; // 移动速度
    [SerializeField] private float rotationSpeed = 8f; // 旋转速度
    [SerializeField] private float newMass = 1f;


    [SerializeField] private Vector2 moveDir; // 移动方向

    private Vector2 direction;// 向量化后的方向

    private Rigidbody2D _rb2D; // 刚体组件
    
    protected override void Awake()
    {
        base.Awake();
        _rb2D = GetComponent<Rigidbody2D>();
    }
    
    private void Start()
    {
        _rb2D.mass = newMass;//设置刚体质量
    }
    
    private void Update()
    {
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    #region 移动

    private void HandleMovement()
    {
        
        moveDir = Managers.GameInput.Instance.moveDir;
        direction =moveDir.normalized;
        if (direction != Vector2.zero)
        {
            Vector2 newPosition =_rb2D.position+ direction * (moveSpeed * Time.fixedDeltaTime);
            if(CanMove())
            {
                _rb2D.MovePosition(newPosition);
            }
        }
        // Quaternion newRotation = Quaternion.LookRotation(moveDir);
        
        
        
        
    }

    private bool CanMove()
    {
        return true;
    }
    
    #endregion
}
