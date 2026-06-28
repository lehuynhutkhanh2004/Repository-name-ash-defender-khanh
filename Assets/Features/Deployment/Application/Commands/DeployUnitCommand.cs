using UnityEngine;

namespace AshDefender.Features.Deployment.Application.Commands
{
    public record DeployUnitCommand(string UnitId, Vector2 Position);
}
