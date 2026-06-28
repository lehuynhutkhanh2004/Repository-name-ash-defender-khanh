using AshDefender.Features.SaveSystem.Domain.Entities;
using AshDefender.Features.SaveSystem.Domain.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AshDefender.Features.SaveSystem.Infrastructure.SaveSystem
{
    public class CloudSaveService : ISaveService
    {
        private readonly ISaveService _localFallback;

        public CloudSaveService(ISaveService localFallback)
        {
            _localFallback = localFallback;
        }

        public async UniTask SaveAsync(SaveData data, string slot)
        {
            await _localFallback.SaveAsync(data, slot);
            Debug.Log($"[CloudSave] Saved slot '{slot}' — cloud sync not yet implemented.");
        }

        public async UniTask<SaveData> LoadAsync(string slot)
        {
            Debug.Log($"[CloudSave] Loading slot '{slot}' — falling back to local.");
            return await _localFallback.LoadAsync(slot);
        }

        public async UniTask DeleteAsync(string slot)
        {
            await _localFallback.DeleteAsync(slot);
        }

        public bool HasSave(string slot) => _localFallback.HasSave(slot);
    }
}
