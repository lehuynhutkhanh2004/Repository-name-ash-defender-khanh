using AshDefender.Features.CoreDefense.Application.DTOs;

namespace AshDefender.Features.CoreDefense.Application.Interfaces
{
    public interface ICoreService
    {
        CoreDTO GetCoreStatus();
        void ApplyDamage(int damage);
    }
}
