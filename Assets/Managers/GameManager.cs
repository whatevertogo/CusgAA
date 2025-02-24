using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        // 单例
        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject(nameof(GameManager));
                        _instance = go.AddComponent<GameManager>();
                        DontDestroyOnLoad(go); // 保证 GameManager 在场景切换时不销毁
                    }
                }

                return _instance;
                
            }
            
        }
        
    }
    
}