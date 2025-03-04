using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // 引入场景管理命名空间

namespace Managers
{
    public class MySceneManager : Singleton<MySceneManager>
    {
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

        public IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            while (asyncOperation is not { isDone: true })
            {
                //TODO - 在这里显示加载进度条
                yield return null;
            }
        }

        public void ReloadCurrentLevel()
        {
            // 获取当前活动场景
            Scene currentScene = SceneManager.GetActiveScene();

            // 重新加载当前场景
            SceneManager.LoadScene(currentScene.name);
        }



    }
}