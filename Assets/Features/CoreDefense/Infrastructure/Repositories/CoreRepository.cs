using AshDefender.Features.CoreDefense.Domain.Entities;
using AshDefender.Features.CoreDefense.Domain.Interfaces;

namespace AshDefender.Features.CoreDefense.Infrastructure.Repositories
{
    public class CoreRepository : ICoreRepository
    {
        private Core _core;

        public CoreRepository(int maxHP)
        {
            _core = new Core(maxHP);
        }

        public Core GetCore() => _core;
        public void Save(Core core) => _core = core;
    }
}
