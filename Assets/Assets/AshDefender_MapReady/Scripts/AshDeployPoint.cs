using UnityEngine;

/// <summary>
/// Attach to a point where a hero/unit can be deployed.
/// You can later connect this to your deployment system.
/// </summary>
public class AshDeployPoint : MonoBehaviour
{
    [SerializeField] private bool occupied;

    public bool IsOccupied => occupied;

    public bool CanDeploy()
    {
        return !occupied;
    }

    public void SetOccupied(bool value)
    {
        occupied = value;
    }
}
