using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace AshDefender.Shared.Core
{
    public class SceneLoader
    {
        public async UniTask LoadAsync(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask();
        }

        public async UniTask LoadAdditiveAsync(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask();
        }

        public async UniTask UnloadAsync(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName).ToUniTask();
        }
    }
}
