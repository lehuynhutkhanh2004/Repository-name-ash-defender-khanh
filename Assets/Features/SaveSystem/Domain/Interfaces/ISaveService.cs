using AshDefender.Features.SaveSystem.Domain.Entities;
using Cysharp.Threading.Tasks;

namespace AshDefender.Features.SaveSystem.Domain.Interfaces
{
    public interface ISaveService
    {
        UniTask SaveAsync(SaveData data, string slot);
        UniTask<SaveData> LoadAsync(string slot);
        UniTask DeleteAsync(string slot);
        bool HasSave(string slot);
    }
}
