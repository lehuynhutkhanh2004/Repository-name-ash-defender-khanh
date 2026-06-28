using UnityEngine;
using UnityEngine.InputSystem;

namespace AshDefender.Features.Deployment.Presentation.Input
{
    public class DeploymentInputHandler : MonoBehaviour
    {
        [SerializeField] private DeploymentPanel deploymentPanel;
        [SerializeField] private Camera gameCamera;

        private string _selectedUnitId;

        public void SelectUnit(string unitId) => _selectedUnitId = unitId;

        public void ClearSelection() => _selectedUnitId = null;

        private void Update()
        {
            if (string.IsNullOrEmpty(_selectedUnitId)) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                var worldPos = gameCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                deploymentPanel.OnDeployButtonClicked(_selectedUnitId, worldPos);
                ClearSelection();
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                ClearSelection();
        }
    }
}
