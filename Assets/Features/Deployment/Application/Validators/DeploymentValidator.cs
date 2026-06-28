using AshDefender.Features.Deployment.Application.DTOs;
using AshDefender.Features.Unit.Domain.Interfaces;
using AshDefender.Shared.Exceptions;

namespace AshDefender.Features.Deployment.Application.Validators
{
    public class DeploymentValidator
    {
        private readonly IUnitRepository _unitRepository;

        public DeploymentValidator(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
        }

        public void Validate(DeploymentRequestDTO request)
        {
            var unit = _unitRepository.GetById(request.UnitId);
            if (unit == null)
                throw new InvalidDeploymentException($"Unit '{request.UnitId}' does not exist.");

            if (request.CurrentEnergy < unit.Cost)
                throw new InsufficientResourcesException("Energy", unit.Cost, request.CurrentEnergy);
        }
    }
}
