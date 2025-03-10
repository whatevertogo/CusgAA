using Interfaces;
using Managers;
using UnityEngine;

/// <summary>
///     可交互对象基类
///     用途：
///     1. 定义所有可交互物体的基本功能
///     2. 提供交互范围和状态控制
///     3. 定义选中和交互的虚拟方法
/// </summary>
public abstract class TriggerObject : MonoBehaviour, IInteract
{
    #region 序列化字段

    [Header("交互设置")] [Tooltip("是否可以交互")] [SerializeField]
    protected bool isInteractable = true;

    [Tooltip("交互范围")] [SerializeField] protected float interactionRange = 2f;

    [Header("引用")] [SerializeField] private InventoryManager inventoryManager;

    #endregion

    #region 公共属性

    /// <summary>
    ///     是否可以交互
    ///     用于控制物体是否响应玩家的交互
    ///     在PlayerController中的HandleClickedObject和UpdateSelectedObject中使用
    /// </summary>
    public virtual bool CanInteract => isInteractable;

    /// <summary>
    ///     交互范围
    ///     定义玩家需要多近才能与物体交互
    ///     在PlayerController的HandleTargetMovement和HandleClickedObject中使用
    /// </summary>
    public float InteractionRange => interactionRange;

    #endregion

    #region 虚拟方法

    /// <summary>
    ///     交互行为
    ///     当玩家点击并移动到范围内时调用
    ///     由PlayerController中的HandleTargetMovement和HandleClickedObject调用
    /// </summary>
    public virtual void Interact()
    {
        if (!CanInteract) return;
        Debug.Log($"与{gameObject.name}交互");
        Debug.Log("不该在这里Interact,这里是基类，说明覆写失败");
    }

    /// <summary>
    ///     当被选中时触发
    ///     在鼠标悬停到物体上时调用
    ///     由PlayerController中的UpdateSelectedObject调用
    /// </summary>
    public virtual void OnSelected()
    {
        if (!CanInteract) return;
        Debug.Log($"{gameObject.name}被选中");
    }

    /// <summary>
    ///     当取消选中时触发
    ///     在鼠标移开物体时调用
    ///     由PlayerController中的UpdateSelectedObject调用
    /// </summary>
    public virtual void OnDeselected()
    {
        Debug.Log($"{gameObject.name}取消选中");
    }

    #endregion

    #region Unity生命周期

    protected virtual void OnEnable()
    {
        // 子类可以在此处注册额外的事件
    }

    protected virtual void OnDisable()
    {
        // 子类可以在此处注销额外的事件
    }

    #endregion
}