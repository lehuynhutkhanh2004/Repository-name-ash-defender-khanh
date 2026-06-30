using AshDefender.Features.Hero.Application.DTOs;
using AshDefender.Features.Hero.Domain.Interfaces;
using AshDefender.Shared.Exceptions;

namespace AshDefender.Features.Hero.Application.UseCases
{
    public class SelectHeroUseCase
    {
        private readonly IHeroRepository _heroRepository;

        public SelectHeroUseCase(IHeroRepository heroRepository)
        {
            _heroRepository = heroRepository;
        }

        public HeroDTO Execute(string heroId)
        {
            var hero = _heroRepository.GetById(heroId);
            if (hero == null)
                throw new GameException($"Hero '{heroId}' not found.");

            return new HeroDTO(
                hero.Id, hero.Name, hero.Level,
                hero.CurrentHealth, hero.MaxHealth,
                hero.Attack, hero.Defense, hero.SkillSet
            );
        }
    }
}
