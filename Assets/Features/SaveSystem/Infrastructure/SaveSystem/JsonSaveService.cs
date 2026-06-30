using AshDefender.Features.SaveSystem.Domain.Entities;
using AshDefender.Features.SaveSystem.Domain.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AshDefender.Features.SaveSystem.Infrastructure.SaveSystem
{
    public class JsonSaveService : ISaveService
    {
        public async UniTask SaveAsync(SaveData data, string slot)
        {
            var json = JsonUtility.ToJson(data, true);
            PlayerPrefs.SetString(slot, json);
            PlayerPrefs.Save();
            await UniTask.Yield();
        }

        public async UniTask<SaveData> LoadAsync(string slot)
        {
            await UniTask.Yield();
            var json = PlayerPrefs.GetString(slot, null);
            if (string.IsNullOrEmpty(json)) return new SaveData();
            return JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
        }

        public async UniTask DeleteAsync(string slot)
        {
            PlayerPrefs.DeleteKey(slot);
            PlayerPrefs.Save();
            await UniTask.Yield();
        }

        public bool HasSave(string slot) => PlayerPrefs.HasKey(slot);
    }
}
