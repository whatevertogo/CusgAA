using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // 引入场景管理命名空间

namespace Managers
{
    public class MySceneManager : Singleton<MySceneManager>
    {

        // 初始化场景管理器单例
        protected override void Awake()
        {
            base.Awake();
        }

        // 通过场景名称加载场景
        // 参数：sceneName - 要加载的场景名称
        // 说明：同步加载场景，在加载完成前会阻塞主线程
        public void LoadSceneByName(string sceneName)
        {
            // 可以在这里添加加载前的处理逻辑
            SceneManager.LoadScene(sceneName);
        }

        // 通过场景索引加载场景
        // 参数：sceneIndex - 场景在Build Settings中的索引号
        // 说明：同步加载场景，在加载完成前会阻塞主线程
        public void LoadSceneByIndex(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        // 异步加载场景
        // 参数：sceneName - 要加载的场景名称
        // 说明：通过协程异步加载场景，不会阻塞主线程
        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }

        // 异步加载场景的协程实现
        // 参数：sceneName - 要加载的场景名称
        // 返回：IEnumerator - 协程迭代器
        // 说明：
        // 1. 使用异步操作加载场景
        // 2. 可以在加载过程中显示进度条（TODO待实现）
        // 3. 通过while循环检测加载进度
        public IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            while (asyncOperation is not { isDone: true })
            {
                //TODO - 在这里显示加载进度条
                yield return null;
            }
        }

        // 重新加载当前场景
        // 说明：
        // 1. 获取当前活动场景
        // 2. 使用场景名称重新加载该场景
        // 用途：可用于重置关卡或重新开始当前场景
        public void ReloadCurrentLevel()
        {
            // 获取当前活动场景
            Scene currentScene = SceneManager.GetActiveScene();

            // 重新加载当前场景
            SceneManager.LoadScene(currentScene.name);
        }



    }
}
