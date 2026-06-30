using System.Collections.Generic;
using System.Linq;
using AshDefender.Features.CommanderSkill.Domain.Enums;
using AshDefender.Features.CommanderSkill.Domain.Interfaces;
using AshDefender.ScriptableObjects;

namespace AshDefender.Features.CommanderSkill.Infrastructure.Repositories
{
    using CommanderSkill = global::AshDefender.Features.CommanderSkill.Domain.Entities.CommanderSkill;
    using SkillType = global::AshDefender.Features.CommanderSkill.Domain.Enums.SkillType;

    public class CommanderSkillRepository : ICommanderSkillRepository
    {
        private readonly Dictionary<string, CommanderSkill> _skills = new();

        public CommanderSkillRepository(SkillConfigSO[] configs)
        {
            foreach (var config in configs)
            {
                var skillType = config.skillType switch
                {
                    AshDefender.ScriptableObjects.SkillType.Heal => SkillType.Heal,
                    AshDefender.ScriptableObjects.SkillType.Buff => SkillType.Buff,
                    AshDefender.ScriptableObjects.SkillType.AoEDamage => SkillType.AoEDamage,
                    AshDefender.ScriptableObjects.SkillType.CrowdControl => SkillType.CrowdControl,
                    _ => SkillType.Buff
                };

                _skills[config.skillId] = new CommanderSkill(
                    config.skillId, config.skillName,
                    skillType, config.manaCost, config.cooldown, config.power
                );
            }
        }

        public CommanderSkill GetById(string skillId) =>
            _skills.TryGetValue(skillId, out var s) ? s : null;

        public IReadOnlyList<CommanderSkill> GetAll() =>
            _skills.Values.ToList().AsReadOnly();

        public void Save(CommanderSkill skill) => _skills[skill.Id] = skill;
    }
}
