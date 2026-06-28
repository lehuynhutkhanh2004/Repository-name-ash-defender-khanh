using System.Collections.Generic;
using AshDefender.Features.Enemy.Domain.Entities;

namespace AshDefender.Features.Enemy.Domain.Interfaces
{
    public interface IEnemyRepository
    {
        Enemy GetById(string enemyId);
        IReadOnlyList<Enemy> GetAll();
        Boss GetBossById(string bossId);
    }
}
