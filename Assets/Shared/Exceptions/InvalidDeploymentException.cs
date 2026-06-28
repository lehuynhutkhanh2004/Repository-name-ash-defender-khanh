namespace AshDefender.Shared.Exceptions
{
    public class InvalidDeploymentException : GameException
    {
        public InvalidDeploymentException(string reason)
            : base($"Deployment failed: {reason}") { }
    }
}
