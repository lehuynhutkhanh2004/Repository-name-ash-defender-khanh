using AshDefender.Features.CommanderSkill.Domain.Interfaces;
using AshDefender.Shared.Exceptions;

namespace AshDefender.Features.CommanderSkill.Application.UseCases
{
    public class CastCommanderSkillUseCase
    {
        private readonly ICommanderSkillRepository _skillRepository;

        public CastCommanderSkillUseCase(ICommanderSkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public void Execute(string skillId, int currentMana)
        {
            var skill = _skillRepository.GetById(skillId);
            if (skill == null)
                throw new GameException($"Skill '{skillId}' not found.");
            if (!skill.IsReady)
                throw new GameException($"Skill '{skillId}' is on cooldown.");
            if (currentMana < skill.ManaCost)
                throw new InsufficientResourcesException("Mana", skill.ManaCost, currentMana);

            skill.Activate();
            _skillRepository.Save(skill);
        }
    }
}
