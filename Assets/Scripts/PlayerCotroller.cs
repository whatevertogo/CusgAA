using Managers;
using UnityEngine;

public class PlayerCotroller : MonoBehaviour
{
    [SerializeField] private GameInput gameInput; // 玩家输入
    
    [SerializeField] private float moveSpeed = 7f; // 移动速度
    
    private Vector2 moveDir; // 移动方向
    
    private Rigidbody2D _rigidbody2D; // 刚体组件


    private void Awake()
    {
        _rigidbody2D=GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        moveDir= gameInput.moveDir; // 获取移动方向
        Vector2 moveDirection = moveDir.normalized;
        _rigidbody2D.linearVelocity = moveSpeed*Time.deltaTime*moveDirection; // 设置刚体速度
    }
}
