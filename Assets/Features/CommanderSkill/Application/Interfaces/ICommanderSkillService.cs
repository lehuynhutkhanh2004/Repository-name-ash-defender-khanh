using System.Collections.Generic;
using AshDefender.Features.CommanderSkill.Application.DTOs;

namespace AshDefender.Features.CommanderSkill.Application.Interfaces
{
    public interface ICommanderSkillService
    {
        IReadOnlyList<CommanderSkillDTO> GetAllSkills();
        bool TryCast(string skillId, int currentMana);
    }
}
