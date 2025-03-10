namespace Managers
{
    /// <summary>
    /// 可重置接口，用于对象池中的对象重置状态
    /// </summary>
    public interface IResettable
    {
        /// <summary>
        /// 重置对象状态
        /// </summary>
        void Reset();
    }
}
