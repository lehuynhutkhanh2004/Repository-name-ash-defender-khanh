using AshDefender.Features.Enemy.Application.DTOs;

namespace AshDefender.Features.Enemy.Application.Interfaces
{
    public interface IEnemyService
    {
        EnemyDTO GetEnemy(string enemyId);
        BossDTO GetBoss(string bossId);
    }
}
