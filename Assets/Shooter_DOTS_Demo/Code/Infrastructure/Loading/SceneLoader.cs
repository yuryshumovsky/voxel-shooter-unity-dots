using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shooter_DOTS_Demo.Code.Infrastructure.Loading
{
    public class SceneLoader : ISceneLoader
    {
        public async UniTask<bool> LoadSceneAsync(
            string sceneName,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            Action<float> onProgress = null
        )
        {
            try
            {
                if (!SceneExists(sceneName))
                {
                    Debug.LogError($"SceneLoader: can't find scene: \"{sceneName}\" in Build Settings!");
                    return false;
                }

                var operation = SceneManager.LoadSceneAsync(sceneName, loadMode);
                if (operation == null)
                {
                    Debug.LogError($"SceneLoader: can't start load scene \"{sceneName}\"");

                    return false;
                }

                operation.allowSceneActivation = false;

                while (operation.progress < 0.9f)
                {
                    onProgress?.Invoke(operation.progress);
                    await UniTask.Yield();
                }

                onProgress?.Invoke(1f);

                await UniTask.DelayFrame(1);
                operation.allowSceneActivation = true;


                await operation.ToUniTask();

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SceneLoader: scene \"{sceneName}\" wasn't loaded: {ex}");

                return false;
            }
        }

        private static bool SceneExists(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                if (string.Equals(name, sceneName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}