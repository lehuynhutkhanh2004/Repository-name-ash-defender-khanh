using System.Collections.Generic;
using System.Linq;
using AshDefender.Features.Enemy.Domain.Entities;
using AshDefender.Features.Enemy.Domain.Interfaces;
using AshDefender.ScriptableObjects;

namespace AshDefender.Features.Enemy.Infrastructure.Repositories
{
    using Enemy = global::AshDefender.Features.Enemy.Domain.Entities.Enemy;
    using DomainEnemyType = global::AshDefender.Features.Enemy.Domain.Enums.EnemyType;

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

        private static DomainEnemyType MapEnemyType(EnemyType soType) =>
            soType switch
            {
                EnemyType.Slime => DomainEnemyType.Slime,
                EnemyType.Goblin => DomainEnemyType.Goblin,
                EnemyType.Spider => DomainEnemyType.Spider,
                EnemyType.MushroomMonster => DomainEnemyType.MushroomMonster,
                EnemyType.Skeleton => DomainEnemyType.Skeleton,
                EnemyType.Bat => DomainEnemyType.Bat,
                EnemyType.GoblinWarrior => DomainEnemyType.GoblinWarrior,
                EnemyType.IceSlime => DomainEnemyType.IceSlime,
                EnemyType.IceGoblin => DomainEnemyType.IceGoblin,
                EnemyType.SnowWolf => DomainEnemyType.SnowWolf,
                EnemyType.DemonSlime => DomainEnemyType.DemonSlime,
                EnemyType.DarkKnight => DomainEnemyType.DarkKnight,
                EnemyType.FireBat => DomainEnemyType.FireBat,
                _ => DomainEnemyType.Slime
            };
    }
}
