using AshDefender.Features.Deployment.Application.DTOs;
using AshDefender.Features.Deployment.Application.Validators;
using AshDefender.Features.Unit.Domain.Interfaces;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;

namespace AshDefender.Features.Deployment.Application.UseCases
{
    public class DeployUnitUseCase
    {
        private readonly IUnitRepository _unitRepository;
        private readonly DeploymentValidator _validator;
        private readonly IEventBus _eventBus;

        public DeployUnitUseCase(IUnitRepository unitRepository, DeploymentValidator validator, IEventBus eventBus)
        {
            _unitRepository = unitRepository;
            _validator = validator;
            _eventBus = eventBus;
        }

        public void Execute(DeploymentRequestDTO request)
        {
            _validator.Validate(request);

            var unit = _unitRepository.GetById(request.UnitId);
            _eventBus.Publish(new UnitDeployedEvent(
                unit.Id,
                unit.UnitType.ToString(),
                request.Position.x,
                request.Position.y
            ));
        }
    }
}
