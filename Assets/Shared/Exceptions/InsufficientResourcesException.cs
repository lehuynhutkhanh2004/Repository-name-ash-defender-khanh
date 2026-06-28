namespace AshDefender.Shared.Exceptions
{
    public class InsufficientResourcesException : GameException
    {
        public InsufficientResourcesException(string resource, int required, int available)
            : base($"Insufficient {resource}: required {required}, available {available}") { }
    }
}
