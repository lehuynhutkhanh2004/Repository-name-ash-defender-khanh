using System.Collections.Generic;
using AshDefender.Features.CommanderSkill.Domain.Entities;

namespace AshDefender.Features.CommanderSkill.Domain.Interfaces
{
    public interface ICommanderSkillRepository
    {
        CommanderSkill GetById(string skillId);
        IReadOnlyList<CommanderSkill> GetAll();
        void Save(CommanderSkill skill);
    }
}
