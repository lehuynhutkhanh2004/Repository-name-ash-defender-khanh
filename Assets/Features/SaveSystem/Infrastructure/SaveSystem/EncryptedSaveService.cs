using System;
using AshDefender.Features.SaveSystem.Domain.Entities;
using AshDefender.Features.SaveSystem.Domain.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AshDefender.Features.SaveSystem.Infrastructure.SaveSystem
{
    public class EncryptedSaveService : ISaveService
    {
        private const string Key = "AshDefender2025!";

        public async UniTask SaveAsync(SaveData data, string slot)
        {
            var json = JsonUtility.ToJson(data);
            var encrypted = Encrypt(json);
            PlayerPrefs.SetString(slot + "_enc", encrypted);
            PlayerPrefs.Save();
            await UniTask.Yield();
        }

        public async UniTask<SaveData> LoadAsync(string slot)
        {
            await UniTask.Yield();
            var encrypted = PlayerPrefs.GetString(slot + "_enc", null);
            if (string.IsNullOrEmpty(encrypted)) return new SaveData();
            var json = Decrypt(encrypted);
            return JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
        }

        public async UniTask DeleteAsync(string slot)
        {
            PlayerPrefs.DeleteKey(slot + "_enc");
            PlayerPrefs.Save();
            await UniTask.Yield();
        }

        public bool HasSave(string slot) => PlayerPrefs.HasKey(slot + "_enc");

        private static string Encrypt(string plain)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(plain);
            return Convert.ToBase64String(bytes);
        }

        private static string Decrypt(string encrypted)
        {
            var bytes = Convert.FromBase64String(encrypted);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
