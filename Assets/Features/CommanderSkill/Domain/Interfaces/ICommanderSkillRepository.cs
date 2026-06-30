using System.Collections.Generic;

namespace AshDefender.Features.CommanderSkill.Domain.Interfaces
{
    using CommanderSkill = global::AshDefender.Features.CommanderSkill.Domain.Entities.CommanderSkill;

    public interface ICommanderSkillRepository
    {
        CommanderSkill GetById(string skillId);
        IReadOnlyList<CommanderSkill> GetAll();
        void Save(CommanderSkill skill);
    }
}
