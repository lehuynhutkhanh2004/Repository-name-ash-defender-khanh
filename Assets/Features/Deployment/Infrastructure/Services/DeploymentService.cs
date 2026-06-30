using AshDefender.Features.Deployment.Application.DTOs;
using AshDefender.Features.Deployment.Application.Validators;
using AshDefender.Features.Deployment.Domain.Interfaces;
using AshDefender.Features.Unit.Domain.Interfaces;

namespace AshDefender.Features.Deployment.Infrastructure.Services
{
    public class DeploymentService : IDeploymentService
    {
        private readonly IUnitRepository _unitRepository;
        private readonly DeploymentValidator _validator;

        public DeploymentService(IUnitRepository unitRepository, DeploymentValidator validator)
        {
            _unitRepository = unitRepository;
            _validator = validator;
        }

        public bool CanDeploy(DeploymentRequestDTO request)
        {
            var unit = _unitRepository.GetById(request.UnitId);
            return unit != null && request.CurrentEnergy >= unit.Cost;
        }

        public void Deploy(DeploymentRequestDTO request)
        {
            _validator.Validate(request);
        }
    }
}
