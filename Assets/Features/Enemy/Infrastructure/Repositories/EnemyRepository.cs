using System.Collections.Generic;
using System.Linq;
using AshDefender.Features.Enemy.Domain.Entities;
using AshDefender.Features.Enemy.Domain.Enums;
using AshDefender.Features.Enemy.Domain.Interfaces;
using AshDefender.ScriptableObjects;

namespace AshDefender.Features.Enemy.Infrastructure.Repositories
{
    public class EnemyRepository : IEnemyRepository
    {
        private readonly Dictionary<string, Enemy> _enemies = new();
        private readonly Dictionary<string, Boss> _bosses = new();

        public EnemyRepository(EnemyConfigSO[] configs)
        {
            foreach (var config in configs)
            {
                var enemyType = MapEnemyType(config.enemyType);
                _enemies[config.enemyId] = new Enemy(
                    config.enemyId, config.enemyName, enemyType,
                    config.health, config.attack, config.moveSpeed, config.reward
                );
            }
        }

        public Enemy GetById(string enemyId) =>
            _enemies.TryGetValue(enemyId, out var e) ? e : null;

        public IReadOnlyList<Enemy> GetAll() =>
            _enemies.Values.ToList().AsReadOnly();

        public Boss GetBossById(string bossId) =>
            _bosses.TryGetValue(bossId, out var b) ? b : null;

        private static EnemyType MapEnemyType(AshDefender.ScriptableObjects.EnemyType soType) =>
            soType switch
            {
                AshDefender.ScriptableObjects.EnemyType.Slime => EnemyType.Slime,
                AshDefender.ScriptableObjects.EnemyType.Goblin => EnemyType.Goblin,
                AshDefender.ScriptableObjects.EnemyType.Spider => EnemyType.Spider,
                AshDefender.ScriptableObjects.EnemyType.MushroomMonster => EnemyType.MushroomMonster,
                AshDefender.ScriptableObjects.EnemyType.Skeleton => EnemyType.Skeleton,
                AshDefender.ScriptableObjects.EnemyType.Bat => EnemyType.Bat,
                AshDefender.ScriptableObjects.EnemyType.GoblinWarrior => EnemyType.GoblinWarrior,
                AshDefender.ScriptableObjects.EnemyType.IceSlime => EnemyType.IceSlime,
                AshDefender.ScriptableObjects.EnemyType.IceGoblin => EnemyType.IceGoblin,
                AshDefender.ScriptableObjects.EnemyType.SnowWolf => EnemyType.SnowWolf,
                AshDefender.ScriptableObjects.EnemyType.DemonSlime => EnemyType.DemonSlime,
                AshDefender.ScriptableObjects.EnemyType.DarkKnight => EnemyType.DarkKnight,
                AshDefender.ScriptableObjects.EnemyType.FireBat => EnemyType.FireBat,
                _ => EnemyType.Slime
            };
    }
}
