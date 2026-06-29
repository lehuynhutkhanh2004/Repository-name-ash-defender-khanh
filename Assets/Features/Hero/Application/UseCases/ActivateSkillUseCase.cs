using AshDefender.Features.Hero.Domain.Interfaces;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using AshDefender.Shared.Exceptions;
using System.Linq;

namespace AshDefender.Features.Hero.Application.UseCases
{
    public class ActivateSkillUseCase
    {
        private readonly IHeroRepository _heroRepository;
        private readonly IEventBus _eventBus;

        public ActivateSkillUseCase(IHeroRepository heroRepository, IEventBus eventBus)
        {
            _heroRepository = heroRepository;
            _eventBus = eventBus;
        }

        public void Execute(string heroId, string skillId)
        {
            var hero = _heroRepository.GetById(heroId);
            if (hero == null)
                throw new GameException($"Hero '{heroId}' not found.");
            if (!hero.SkillSet.Contains(skillId))
                throw new GameException($"Hero '{heroId}' does not have skill '{skillId}'.");

            _eventBus.Publish(new SkillUnlockedEvent(heroId, skillId));
        }
    }
}
