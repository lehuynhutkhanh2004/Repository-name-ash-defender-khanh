using System.Collections.Generic;
using UnitEntity = AshDefender.Features.Unit.Domain.Entities.Unit;

namespace AshDefender.Features.Unit.Domain.Interfaces
{
    public interface IUnitRepository
    {
        UnitEntity GetById(string unitId);
        IReadOnlyList<UnitEntity> GetAll();
        void Save(UnitEntity unit);
    }
}
