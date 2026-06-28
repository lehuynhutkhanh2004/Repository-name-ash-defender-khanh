using AshDefender.Features.Deployment.Application.DTOs;

namespace AshDefender.Features.Deployment.Domain.Interfaces
{
    public interface IDeploymentService
    {
        bool CanDeploy(DeploymentRequestDTO request);
        void Deploy(DeploymentRequestDTO request);
    }
}
