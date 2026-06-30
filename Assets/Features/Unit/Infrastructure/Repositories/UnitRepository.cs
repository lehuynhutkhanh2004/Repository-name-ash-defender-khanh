using System.Collections.Generic;
using System.Linq;
using AshDefender.Features.Unit.Domain.Interfaces;
using AshDefender.ScriptableObjects;
using UnitEntity = AshDefender.Features.Unit.Domain.Entities.Unit;
using UnitTypeEnum = AshDefender.Features.Unit.Domain.Enums.UnitType;

namespace AshDefender.Features.Unit.Infrastructure.Repositories
{
    public class UnitRepository : IUnitRepository
    {
        private readonly Dictionary<string, UnitEntity> _units = new();

        public UnitRepository(UnitConfigSO[] configs)
        {
            foreach (var config in configs)
            {
                var unitType = config.unitType switch
                {
                    AshDefender.ScriptableObjects.UnitType.Melee => UnitTypeEnum.Melee,
                    AshDefender.ScriptableObjects.UnitType.Ranged => UnitTypeEnum.Ranged,
                    AshDefender.ScriptableObjects.UnitType.Tank => UnitTypeEnum.Tank,
                    AshDefender.ScriptableObjects.UnitType.Support => UnitTypeEnum.Support,
                    _ => UnitTypeEnum.Melee
                };

                _units[config.unitId] = new UnitEntity(
                    config.unitId, config.unitName, unitType,
                    config.cost, config.health, config.attack, config.moveSpeed
                );
            }
        }

        public UnitEntity GetById(string unitId) =>
            _units.TryGetValue(unitId, out var unit) ? unit : null;

        public IReadOnlyList<UnitEntity> GetAll() =>
            _units.Values.ToList().AsReadOnly();

        public void Save(UnitEntity unit) => _units[unit.Id] = unit;
    }
}
