
using UnityEngine;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        protected override void Awake()
        {
            base.Awake();
            // 初始化
        }

        private void StartGame()
        {
            MySceneManager.Instance.LoadSceneAsync("Level1", null, OnLevelLoaded);
        }

        private void OnLevelLoaded(object sender, MySceneManager.OnSceneLoadCompleteEventArgs args)
        {
            Debug.Log($"场景 {args.SceneName} 加载完成");
            // 在这里写场景加载后的初始化代码
            InitializeLevel();
        }

        private static void InitializeLevel()
        {
            // 关卡初始化逻辑
        }
        //TODO-特定的GameManager功能


    }

}