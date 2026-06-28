using System.Collections.Generic;
using AshDefender.Features.Unit.Application.DTOs;

namespace AshDefender.Features.Unit.Application.Interfaces
{
    public interface IUnitService
    {
        UnitDTO GetUnit(string unitId);
        IReadOnlyList<UnitDTO> GetAllUnits();
    }
}
