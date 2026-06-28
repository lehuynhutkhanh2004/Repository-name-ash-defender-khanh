using AshDefender.Features.CoreDefense.Domain.Entities;

namespace AshDefender.Features.CoreDefense.Domain.Interfaces
{
    public interface ICoreRepository
    {
        Core GetCore();
        void Save(Core core);
    }
}
