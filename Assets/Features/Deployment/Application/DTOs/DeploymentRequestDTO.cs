using UnityEngine;

namespace AshDefender.Features.Deployment.Application.DTOs
{
    public record DeploymentRequestDTO(string UnitId, Vector2 Position, int CurrentEnergy);
}
