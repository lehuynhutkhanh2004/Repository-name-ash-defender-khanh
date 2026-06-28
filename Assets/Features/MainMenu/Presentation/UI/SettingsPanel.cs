using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace AshDefender.Features.MainMenu.Presentation.UI
{
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private GameObject panelRoot;

        public void Show() => panelRoot?.SetActive(true);
        public void Hide() => panelRoot?.SetActive(false);

        public void OnMasterVolumeChanged(float value)
        {
            var db = value > 0.001f ? 20f * Mathf.Log10(value) : -80f;
            audioMixer?.SetFloat("MasterVolume", db);
        }

        public void OnMusicVolumeChanged(float value)
        {
            var db = value > 0.001f ? 20f * Mathf.Log10(value) : -80f;
            audioMixer?.SetFloat("MusicVolume", db);
        }

        public void OnSfxVolumeChanged(float value)
        {
            var db = value > 0.001f ? 20f * Mathf.Log10(value) : -80f;
            audioMixer?.SetFloat("SFXVolume", db);
        }

        public void OnCloseClicked() => Hide();
    }
}
