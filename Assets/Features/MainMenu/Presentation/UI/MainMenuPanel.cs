using AshDefender.Shared.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.MainMenu.Presentation.UI
{
    public class MainMenuPanel : MonoBehaviour
    {
        private SceneLoader _sceneLoader;

        [Inject]
        public void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void OnPlayClicked()
        {
            _sceneLoader.LoadAsync("SquadSelection").Forget();
        }

        public void OnSettingsClicked()
        {
            FindFirstObjectByType<SettingsPanel>()?.Show();
        }

        public void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
