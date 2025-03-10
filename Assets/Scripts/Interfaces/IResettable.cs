namespace Interfaces
{
    /// <summary>
    ///     可重置接口
    ///     用途：
    ///     1. 支持对象池中对象的重置功能
    ///     2. 确保对象在重用前恢复到初始状态
    ///     3. 避免对象池复用时的状态污染
    /// </summary>
    public interface IResettable
    {
        /// <summary>
        ///     重置对象状态
        ///     在对象被对象池回收前调用
        ///     实现此方法以清理对象的所有状态
        /// </summary>
        void Reset();
    }
}