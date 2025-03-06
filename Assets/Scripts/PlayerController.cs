/* C# 中的 PlayerController 类管理玩家的移动、加速、跳跃和地面
使用各种参数和优化进行检测。*/
using Managers;
using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("人物移动参数")]
    [Tooltip("移动速度（参考蔚蓝）")]
    [SerializeField] private float moveSpeed = 9f; // 移动速度（参考蔚蓝）
    [Tooltip("最大移动速度限制")]
    [SerializeField] private float maxMoveSpeed = 10f; // 最大移动速度限制
    [Tooltip("质量")]
    [SerializeField] private float newMass = 1f;// 质量

    [Header("人物加减速度")]
    [Tooltip("加速度（调整）")]
    [SerializeField] private float acceleration = 90f; // 加速度（调整）
    [Tooltip("减速度（增加）")]
    [SerializeField] private float deceleration = 60f; // 减速度（增加）

    [Header("速度曲线参数，空中控制系数，空气阻力")]
    [Tooltip("速度曲线指数")]
    [SerializeField] private float velocityPower = 0.9f; // 速度曲线指数
    [Tooltip("空中控制系数（减小）")]
    [SerializeField] private float airControl = 0.6f; // 空中控制系数（减小）
    [Tooltip("空气阻力（减小）")]
    [SerializeField] private float airDrag = 0.4f; // 空气阻力（减小）
    [Tooltip("移动方向")]
    private Vector2 _moveDirection; // 移动方向
    [Tooltip("记录最后移动方向")]
    private float _lastMoveDirection; // 记录最后移动方向

    [Header("人物跳跃参数")]
    [Tooltip("跳跃力度（调整）")]
    [SerializeField] private float jumpForce = 10f;// 跳跃力度（调整）
    [Tooltip("最大跳跃按住时间（调整）")]
    [SerializeField] private float maxJumpHoldTime = 0.2f;// 最大跳跃按住时间（调整）
    [Tooltip("射线长度")]
    [SerializeField] private float rayLength = 1.6f; // 射线长度
    [Tooltip("重力")]
    [SerializeField] private Vector2 gravity;

    [Header("跳跃优化")]
    [Tooltip("土狼时间")]
    [SerializeField] private float coyoteTime = 0.1f; // 土狼时间（缩短）
    [Tooltip("跳跃缓冲(缩短)")]
    [SerializeField] private float jumpBuffer = 0.1f; // 跳跃缓冲（缩短）
    [Tooltip("下落加速度倍数")]
    [SerializeField] private float fallMultiplier = 1.8f; // 下落加速度倍数
    [Tooltip("短跳加速倍数")]
    [SerializeField] private float shortJumpMultiplier = 2.5f; // 短跳加速倍数（新增）
    [Tooltip("落地特效时间")]
    [SerializeField] private float landingVFXTime = 0.15f; // 落地特效时间

    [Header("互动参数")]
    [Tooltip("互动半径")]
    [SerializeField] private float interactionRadius = 1f; // 互动半径
    [Header("互动物体检测")]
    private TriggerObject nearestTriggerObject; // 最近的互动物体

    //[Header("未使用")]
    //[SerializeField] private float minJumpForce = 7f;// 最小跳跃力度
    //[SerializeField] private float preLandingTime = 0.15f; // 预落地时间

    [Header("地面检测")]
    [SerializeField] private LayerMask groundLayer;// 地面层

    [Header("互动物体")]
    private List<TriggerObject> triggerObjects = new List<TriggerObject>(); // 互动物体列表

    public class OnTriggerObjectChoosedEventArgs:EventArgs
    {

    }

     public event EventHandler<OnTriggerObjectChoosedEventArgs> OnTriggerObjectChoosed;  // 选择互动物体事件

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
    private RaycastHit2D _groundHit;// 地面检测结果

    // 初始化组件和物理系统
    // 说明：
    // 1. 获取刚体组件
    // 2. 获取精灵渲染器组件
    // 3. 设置全局重力
    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Physics2D.gravity = gravity;
    }

    // 初始化角色状态和事件订阅
    // 说明：
    // 1. 设置刚体质量
    // 2. 初始化地面检测位置
    // 3. 订阅输入系统的跳跃和互动事件
    private void Start()
    {
        _rb2D.mass = newMass; // 设置刚体质量
        _lastGroundedY = transform.position.y; // 初始化最后着地位置
        // 订阅跳跃事件
        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    // 清理事件订阅，防止内存泄漏
    // 说明：在对象销毁时取消对输入系统事件的订阅
    private void OnDestroy()
    {
        // 取消订阅以防止内存泄漏
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnJumpAction -= GameInput_OnJumpAction;
            GameInput.Instance.OnInteractAction -= GameInput_OnInteractAction;
        }
    }

    // 每帧更新逻辑
    // 说明：
    // 1. 进行地面检测
    // 2. 更新各种计时器
    // 3. 处理跳跃持续时间
    // 4. 处理着地效果
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

    // 固定时间间隔的物理更新
    // 说明：
    // 1. 处理角色移动
    // 2. 应用下落加速度
    // 3. 限制最大水平速度
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
    // 处理角色移动
    // 说明：
    // 1. 获取输入方向并归一化
    // 2. 计算目标速度和当前速度的差值
    // 3. 应用加速度和控制系数
    // 4. 处理地面和空中的不同移动状态
    // 5. 更新角色朝向
    // 6. 处理下落检测和预落地效果
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
            _rb2D.AddForce(Vector2.right * (movement * Time.fixedDeltaTime), ForceMode2D.Impulse);
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
                if (_groundHit.collider is not null)
                {
                    _isPreLanding = true;
                    // 这里可以触发预落地动画
                }
            }
        }
    }

    // 检查角色是否可以移动
    // 返回：true - 可以移动，false - 不能移动
    // 说明：可以在这里添加各种移动限制条件
    private bool CanMove()
    {
        return true;
    }
    #endregion

    #region 跳跃

    // 处理跳跃输入事件
    // 说明：
    // 1. 设置跳跃缓冲计时器
    // 2. 尝试执行跳跃
    private void GameInput_OnJumpAction(object sender, EventArgs e)
    {
        _jumpBufferCounter = jumpBuffer;
        TryJump();
    }

    // 更新各种计时器状态
    // 说明：
    // 1. 更新土狼时间计数器
    // 2. 更新跳跃缓冲计数器
    // 3. 在合适的时机尝试执行跳跃
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

    // 检测角色是否在地面上
    // 说明：
    // 1. 使用射线检测地面
    // 2. 更新地面状态
    // 3. 处理刚落地和刚离地的状态变化
    // 4. 重置相关计数器和状态
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

    // 尝试执行跳跃
    // 说明：
    // 1. 检查是否满足跳跃条件（在地面上或在土狼时间内）
    // 2. 检查垂直速度确保不会在上升时二次跳跃
    // 3. 执行跳跃并重置相关状态
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

    // 执行跳跃动作
    // 参数：force - 跳跃力度
    // 说明：
    // 1. 重置垂直速度
    // 2. 应用跳跃力
    // 3. 重置跳跃相关状态
    private void PerformJump(float force)
    {
        // 重置垂直速度，确保每次跳跃都从0开始
        _rb2D.linearVelocity = new Vector2(_rb2D.linearVelocity.x, 0f);

        // 应用跳跃力 - 直接使用力而非插值，更接近蔚蓝的感觉
        _rb2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        _hasBufferedJump = false;
        _jumpBufferCounter = 0;
    }

    // 应用下落加速度修正
    // 说明：
    // 1. 在下落时增加重力
    // 2. 在短跳时（松开跳跃键）应用更大的下落速度
    // 目的：实现更好的跳跃手感
    private void ApplyFallMultiplier()
    {
        if (!_isGrounded) // 只在非地面状态应用
        {
            // 在下落时应用更大的重力
            if (_rb2D.linearVelocity.y < 0)
            {
                // 确保这是一个向下的力
                float fallForce = fallMultiplier - 1;
                _rb2D.linearVelocity += Vector2.up * gravity * (fallForce * Time.fixedDeltaTime);
            }
            // 短跳（当玩家释放跳跃键时）
            else if (_rb2D.linearVelocity.y > 0 && !GameInput.Instance.JumpPressed)
            {
                // 应用更大的向下力量
                float shortJumpForce = shortJumpMultiplier - 1;
                _rb2D.linearVelocity += Vector2.up * gravity * (shortJumpForce * Time.fixedDeltaTime);
            }
        }
    }
    #endregion

    #region 互动
    // 处理互动输入事件
    // 说明：当玩家按下互动键时，触发最近的可互动物体的交互功能
    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        //TODO: 互动逻辑
        if (nearestTriggerObject != null)
        {
            nearestTriggerObject.Interact();
        }

    }

    // 处理触发器进入事件
    // 参数：other - 进入触发区域的碰撞体
    // 说明：
    // 1. 检查进入的物体是否是可交互物体
    // 2. 将可交互物体添加到列表
    // 3. 更新最近的可交互物体
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.TryGetComponent<TriggerObject>(out var triggerObject))
        {
            triggerObjects.Add(triggerObject);
            float distance = Vector3.Distance(transform.position, triggerObject.transform.position);
            if (nearestTriggerObject == null ||
           distance < Vector3.Distance(transform.position, nearestTriggerObject.transform.position))
            {
                nearestTriggerObject = triggerObject;
                FindNearestTriggerObject();
            }
        }

    }

    // 处理触发器退出事件
    // 参数：other - 离开触发区域的碰撞体
    // 说明：
    // 1. 从列表中移除离开的可交互物体
    // 2. 如果移除的是当前最近的物体，重新寻找最近物体
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<TriggerObject>(out var triggerObject))
        {
            // 从列表中移除
            triggerObjects.Remove(triggerObject);

            // 如果移除的是当前最近的触发物体，需要重新查找最近的触发物体
            if (triggerObject == nearestTriggerObject)
            {
                FindNearestTriggerObject();
            }
        }
    }

    // 寻找最近的可交互物体
    // 说明：
    // 1. 遍历所有可交互物体
    // 2. 计算与玩家的距离
    // 3. 更新最近的可交互物体
    // 4. 触发选中事件
    private void FindNearestTriggerObject()
    {
        nearestTriggerObject = null;
        float nearestDistance = float.MaxValue;

        foreach (var obj in triggerObjects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < nearestDistance)
            {
                nearestTriggerObject = obj;
                nearestDistance = distance;
                OnnearestTriggerObjectChoosed();
            }
        }
    }

    // 当选中最近的可交互物体时触发事件
    // 说明：如果存在最近的可交互物体，触发选中事件
    private void OnnearestTriggerObjectChoosed()
    {
        if (nearestTriggerObject != null)
        {
            OnTriggerObjectChoosed?.Invoke(this, new OnTriggerObjectChoosedEventArgs());
        }

        #endregion
    }



}
