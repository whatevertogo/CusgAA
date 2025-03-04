using UnityEngine;

namespace Managers
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance is null)
                {
                   _instance = FindFirstObjectByType<T>();  // 尝试在场景中查找实例
                    if (_instance is null)
                    {
                        GameObject go = new GameObject(typeof(T).Name); // 如果没有找到，创建新的 GameObject
                        _instance = go.AddComponent<T>(); // 添加组件并赋值
                        DontDestroyOnLoad(go); // 确保实例在场景切换时不被销毁
                    }
                }
                return _instance;
            }
        }

        // 保护的虚拟方法，可以在派生类中重写
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T; // 设置实例
                DontDestroyOnLoad(gameObject); // 防止场景切换时销毁
            }
            else if (_instance != this)
            {
                Destroy(gameObject); // 如果已有实例，销毁当前对象
            }
        }
    }
}
