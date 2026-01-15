using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Shooter_DOTS_Demo.Code.Infrastructure.Loading
{
    public interface ISceneLoader
    {
        UniTask<bool> LoadSceneAsync(
            string sceneName,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            Action<float> onProgress = null
        );
    }
}