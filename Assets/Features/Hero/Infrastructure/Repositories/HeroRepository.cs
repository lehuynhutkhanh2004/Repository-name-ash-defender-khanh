using System.Collections.Generic;
using System.Linq;
using AshDefender.Features.Hero.Domain.Interfaces;
using AshDefender.ScriptableObjects;
using HeroEntity = AshDefender.Features.Hero.Domain.Entities.Hero;

namespace AshDefender.Features.Hero.Infrastructure.Repositories
{
    public class HeroRepository : IHeroRepository
    {
        private readonly Dictionary<string, HeroEntity> _heroes = new();
        private readonly HashSet<string> _unlockedIds = new();

        public HeroRepository(HeroConfigSO[] configs)
        {
            foreach (var config in configs)
            {
                var skillIds = config.skillSet.Select(s => s.skillId).ToList().AsReadOnly();
                _heroes[config.heroId] = new HeroEntity(
                    config.heroId, config.heroName,
                    config.baseHealth, config.baseAttack, config.baseDefense,
                    skillIds
                );
            }
        }

        public HeroEntity GetById(string heroId) =>
            _heroes.TryGetValue(heroId, out var hero) ? hero : null;

        public IReadOnlyList<HeroEntity> GetAll() =>
            _heroes.Values.ToList().AsReadOnly();

        public IReadOnlyList<HeroEntity> GetUnlocked() =>
            _heroes.Values.Where(h => _unlockedIds.Contains(h.Id)).ToList().AsReadOnly();

        public void Save(HeroEntity hero) => _heroes[hero.Id] = hero;

        public void Unlock(string heroId) => _unlockedIds.Add(heroId);
    }
}
