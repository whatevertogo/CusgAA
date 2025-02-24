using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // 引入场景管理命名空间

namespace Managers
{
    public class MySceneManager : MonoBehaviour
    {
        private static MySceneManager _instance;

        public static MySceneManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<MySceneManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject(nameof(MySceneManager));
                        _instance = go.AddComponent<MySceneManager>();
                        DontDestroyOnLoad(go);
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Load scene by name
        public void LoadSceneByName(string sceneName)
        {
            // 可以在这里添加加载前的处理逻辑
            SceneManager.LoadScene(sceneName);
        }

        // Load scene by index (scene build index)
        public void LoadSceneByIndex(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        
        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }

        private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            while (asyncOperation is not { isDone: true })
            {
                //todo - 在这里显示加载进度条
                yield return null;
            }
        }
        
        
        
    }
}