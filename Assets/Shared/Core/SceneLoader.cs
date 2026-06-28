using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace AshDefender.Shared.Core
{
    public class SceneLoader
    {
        public async UniTask LoadAsync(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName);
        }

        public async UniTask LoadAdditiveAsync(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        public async UniTask UnloadAsync(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
