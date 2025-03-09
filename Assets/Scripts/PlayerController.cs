/* C# 中的 PlayerController 类管理玩家的移动、加速、跳跃和地面使用各种参数和优化进行检测。
 *加上互动功能及鼠标高亮选择功能
 * 
*/

using Managers;
using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 玩家角色控制器
/// 功能：
/// 1. AD键移动、空格跳跃
/// 2. 点击移动到交互物体位置
/// 3. 鼠标悬停高亮可交互物体
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region 人物参数

    [Header("人物移动参数")]
    [Tooltip("移动速度（参考蔚蓝）")]
    [SerializeField]
    private float moveSpeed = 9f; // 移动速度（参考蔚蓝）

    [Tooltip("最大移动速度限制")][SerializeField] private float maxMoveSpeed = 10f; // 最大移动速度限制
    [Tooltip("质量")][SerializeField] private float newMass = 1f; // 质量

    [Header("人物加减速度")]
    [Tooltip("加速度（调整）")]
    [SerializeField]
    private float acceleration = 90f; // 加速度（调整）

    [Tooltip("减速度（增加）")][SerializeField] private float deceleration = 60f; // 减速度（增加）

    [Header("速度曲线参数，空中控制系数，空气阻力")]
    [Tooltip("速度曲线指数")]
    [SerializeField]
    private float velocityPower = 0.9f; // 速度曲线指数

    [Tooltip("空中控制系数（减小）")]
    [SerializeField]
    private float airControl = 0.6f; // 空中控制系数（减小）

    [Tooltip("空气阻力（减小）")][SerializeField] private float airDrag = 0.4f; // 空气阻力（减小）
    [Tooltip("移动方向")] private Vector2 _moveDirection; // 移动方向
    [Tooltip("记录最后移动方向")] private float _lastMoveDirection; // 记录最后移动方向
    [Header("地面检测")][SerializeField] private LayerMask groundLayer; // 地面层

    //====================================================================================================
    /*跳跃参数
     *跳跃力度，跳跃按住时间，射线长度等等
     */
    //====================================================================================================

    [Header("人物跳跃参数")]
    [Tooltip("跳跃力度（调整）")]
    [SerializeField]
    private float jumpForce = 10f; // 跳跃力度（调整）

    [Tooltip("最大跳跃按住时间（调整）")]
    [SerializeField]
    private float maxJumpHoldTime = 0.2f; // 最大跳跃按住时间（调整）

    [Tooltip("射线长度")][SerializeField] private float rayLength = 1.6f; // 射线长度
    [Tooltip("重力")][SerializeField] private Vector2 gravity;

    [Header("跳跃优化")]
    [Tooltip("土狼时间")]
    [SerializeField]
    private float coyoteTime = 0.1f; // 土狼时间（缩短）

    [Tooltip("跳跃缓冲(缩短)")][SerializeField] private float jumpBuffer = 0.1f; // 跳跃缓冲（缩短）
    [Tooltip("下落加速度倍数")][SerializeField] private float fallMultiplier = 1.8f; // 下落加速度倍数
    [Tooltip("短跳加速倍数")][SerializeField] private float shortJumpMultiplier = 2.5f; // 短跳加速倍数（新增）
    [Tooltip("落地特效时间")][SerializeField] private float landingVFXTime = 0.15f; // 落地特效时间
    #endregion

    #region 互动
    [Header("互动物体")]
    private List<TriggerObject> _triggerObjects = new List<TriggerObject>(); // 触发范围内的物体列表

    // [旧版本] 最近物体检测相关变量和属性
    /*
    private TriggerObject _nearestTriggerObject;
    public TriggerObject NearestTriggerObject => _nearestTriggerObject;
    */

    [Header("点击交互")]
    private TriggerObject _clickedTriggerObject; // 当前点击的交互对象
    private bool _isMovingToTarget; // 是否正在移动到目标
    private Vector2 _targetPosition; // 目标位置

    [SerializeField] private float selectRadius = 1f; // 鼠标选择的半径范围
    private TriggerObject _selectedObject; // 当前选中的物体（鼠标附近的）
    public TriggerObject SelectedObject => _selectedObject; // 供外部访问当前选中物体

    #endregion

    #region 私有参数

    private bool _isGrounded; // 是否在地面上
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
    private RaycastHit2D _groundHit; // 地面检测结果

    #endregion

    public float overlapRadius = 0.5f;
    public static PlayerController Instance { get; private set; }
    Vector2 mousePosition;
   
    #region 事件

    /// <summary>
    /// 物体选中事件参数
    /// </summary>
    public class TriggerObjectSelectedEventArgs : EventArgs
    {
        public TriggerObject SelectedObject;
    }

    public event EventHandler<TriggerObjectSelectedEventArgs> OnTriggerObjectSelected;

    #endregion
    
    #region 生命周期函数

    /// <summary>
    /// 初始化核心组件和单例
    /// </summary>
    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Physics2D.gravity = gravity;
        if (Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// 初始化状态并订阅输入事件
    /// </summary>
    private void Start()
    {
        _rb2D.mass = newMass;
        _lastGroundedY = transform.position.y;
        GameInput.Instance.OnClickAction += GameInput_OnClickAction;
        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
    }

    /// <summary>
    /// 退订事件，防止内存泄漏
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
    /// 每帧更新：处理输入、状态和高亮系统
    /// </summary>
    private void Update()
    {
        CheckGround();
        UpdateTimers();
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        UpdateSelectedObject();

        // 处理跳跃持续时间
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
            if (Time.time >= landingVFXTime)
            {
                _isLanding = false;
            }
        }

        // 处理移动状态
        if (_isMovingToTarget && _clickedTriggerObject != null)
        {
            float directionX = Mathf.Sign(_targetPosition.x - transform.position.x);
            Vector2 autoMoveDirection = new Vector2(directionX, 0);

            if (Mathf.Abs(transform.position.x - _targetPosition.x) < 0.5f)
            {
                _isMovingToTarget = false;
                autoMoveDirection = Vector2.zero;
            }

            if (Mathf.Abs(GameInput.Instance.moveDir.x) > 0.1f)
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
        else
        {
            _moveDirection = GameInput.Instance.moveDir;
        }
    }

    /// <summary>
    /// 物理更新：处理移动和跳跃物理
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
    /// 处理移动逻辑：速度计算、力的应用和朝向控制
    /// </summary>
    private void HandleMovement()
    {
        if (!_isMovingToTarget)
        {
            _moveDirection = GameInput.Instance.moveDir;
        }
        _direction = _moveDirection.normalized;

        if (_direction != Vector2.zero && CanMove())
        {
            _lastMoveDirection = _direction.x;

            float targetSpeed = _direction.x * moveSpeed;
            float controlModifier = _isGrounded ? 1f : airControl;
            float currentSpeed = _rb2D.linearVelocity.x;
            float speedDifference = targetSpeed - currentSpeed;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

            float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelRate, velocityPower) *
                             Mathf.Sign(speedDifference);
            movement *= controlModifier;

            _rb2D.AddForce(Vector2.right * (movement * Time.fixedDeltaTime), ForceMode2D.Impulse);
        }
        else
        {
            float friction = _isGrounded ? deceleration : (airDrag * deceleration);
            Vector2 frictionForce = -_rb2D.linearVelocity.x * friction * Vector2.right;
            _rb2D.AddForce(frictionForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }

        // 角色朝向 - 基于移动方向
        if (_moveDirection.x < 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (_moveDirection.x > 0)
        {
            _spriteRenderer.flipX = true;
        }

        if (!_isGrounded && _rb2D.linearVelocity.y < 0)
        {
            _fallDistance = _lastGroundedY - transform.position.y;

            if (!_isPreLanding && _fallDistance > 1f)
            {
                _groundHit = Physics2D.Raycast(transform.position, Vector2.down, rayLength * 3f, groundLayer);
                if (_groundHit.collider is not null)
                {
                    _isPreLanding = true;
                }
            }
        }
    }

    /// <summary>
    /// 检查是否可以移动
    /// </summary>
    private bool CanMove()
    {
        return true;
    }

    #endregion

    #region 跳跃系统

    /// <summary>
    /// 跳跃输入响应
    /// </summary>
    private void GameInput_OnJumpAction(object sender, EventArgs e)
    {
        _jumpBufferCounter = jumpBuffer;
        TryJump();
    }

    /// <summary>
    /// 更新计时器：土狼时间和跳跃缓冲
    /// </summary>
    private void UpdateTimers()
    {
        if (!_isGrounded)
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

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
    /// 地面检测和着地处理
    /// </summary>
    private void CheckGround()
    {
        _groundHit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);

        bool wasGrounded = _isGrounded;
        _isGrounded = _groundHit.collider is not null;

        if (_isGrounded && !wasGrounded)
        {
            _hasBufferedJump = false;
            _coyoteTimeCounter = coyoteTime;

            if (_fallDistance > 1f)
            {
                _isLanding = true;
            }

            _isPreLanding = false;
            _fallDistance = 0;
            _lastGroundedY = transform.position.y;
        }
        else if (!_isGrounded && wasGrounded)
        {
            _coyoteTimeCounter = coyoteTime;
            _lastGroundedY = transform.position.y;
        }
    }

    /// <summary>
    /// 尝试执行跳跃
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
    /// 执行跳跃物理
    /// </summary>
    private void PerformJump(float force)
    {
        _rb2D.linearVelocity = new Vector2(_rb2D.linearVelocity.x, 0f);
        _rb2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        _hasBufferedJump = false;
        _jumpBufferCounter = 0;
    }

    /// <summary>
    /// 应用下落加速和短跳修正
    /// </summary>
    private void ApplyFallMultiplier()
    {
        if (!_isGrounded)
        {
            if (_rb2D.linearVelocity.y < 0)
            {
                float fallForce = fallMultiplier - 1;
                _rb2D.linearVelocity += Vector2.up * (gravity.y * (fallForce * Time.fixedDeltaTime));
            }
            else if (_rb2D.linearVelocity.y > 0 && !GameInput.Instance.JumpPressed)
            {
                float shortJumpForce = shortJumpMultiplier - 1;
                _rb2D.linearVelocity += Vector2.up * (gravity.y * (shortJumpForce * Time.fixedDeltaTime));
            }
        }
    }

    #endregion

    #region 交互系统

    /// <summary>
    /// 处理鼠标点击：移动到目标或触发交互
    /// </summary>
    private void GameInput_OnClickAction(object sender, EventArgs e)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            TriggerObject clickedObject = hit.collider.GetComponent<TriggerObject>();
            
            if (clickedObject != null)
            {
                Debug.Log("点击到TriggerObject: " + clickedObject.name);
                
                Vector2 targetPosition = new Vector2(
                    clickedObject.transform.position.x,
                    transform.position.y
                );

                if (clickedObject == _clickedTriggerObject && _triggerObjects.Contains(clickedObject))
                {
                    clickedObject.Interact();
                    _clickedTriggerObject = null;
                    _isMovingToTarget = false;
                    _moveDirection = Vector2.zero;
                }
                else 
                {
                    _clickedTriggerObject = clickedObject;
                    _targetPosition = targetPosition;
                    _isMovingToTarget = true;
                    
                    Debug.Log($"开始移动到目标位置: {_targetPosition}");
                }
            }
        }
    }

    /// <summary>
    /// 进入触发区域：添加到可交互列表
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<TriggerObject>(out var triggerObject))
        {
            _triggerObjects.Add(triggerObject);
        }
    }

    /// <summary>
    /// 离开触发区域：移除并清除选中状态
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<TriggerObject>(out var triggerObject))
        {
            _triggerObjects.Remove(triggerObject);
            if (_selectedObject == triggerObject)
            {
                _selectedObject = null;
                OnObjectSelected(null);
            }
        }
    }

    /// <summary>
    /// 更新鼠标附近最近的可交互物体
    /// </summary>
    private void UpdateSelectedObject()
    {
        TriggerObject nearestToMouse = null;
        float closestDistance = float.MaxValue;

        foreach (var obj in _triggerObjects)
        {
            float distance = Vector2.Distance(mousePosition, obj.transform.position);
            if (distance < selectRadius && distance < closestDistance)
            {
                nearestToMouse = obj;
                closestDistance = distance;
            }
        }

        if (_selectedObject != nearestToMouse)
        {
            _selectedObject = nearestToMouse;
            OnObjectSelected(_selectedObject);
        }
    }

    /// <summary>
    /// 触发选中事件
    /// </summary>
    private void OnObjectSelected(TriggerObject selectedObject)
    {
        OnTriggerObjectSelected?.Invoke(this, new TriggerObjectSelectedEventArgs
        {
            SelectedObject = selectedObject
        });
    }

    #endregion


}
