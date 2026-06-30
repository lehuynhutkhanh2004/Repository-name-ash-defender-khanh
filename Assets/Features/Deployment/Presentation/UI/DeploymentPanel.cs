using AshDefender.Features.Deployment.Application.DTOs;
using AshDefender.Features.Deployment.Application.UseCases;
using AshDefender.Shared.Exceptions;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.Deployment.Presentation.UI
{
    public class DeploymentPanel : MonoBehaviour
    {
        private DeployUnitUseCase _deployUseCase;
        private int _currentEnergy;

        [Inject]
        public void Construct(DeployUnitUseCase deployUseCase)
        {
            _deployUseCase = deployUseCase;
        }

        public void UpdateEnergy(int energy) => _currentEnergy = energy;

        public void OnDeployButtonClicked(string unitId, Vector2 position)
        {
            try
            {
                _deployUseCase.Execute(new DeploymentRequestDTO(unitId, position, _currentEnergy));
            }
            catch (InvalidDeploymentException e)
            {
                Debug.LogWarning(e.Message);
            }
            catch (InsufficientResourcesException e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }
}
