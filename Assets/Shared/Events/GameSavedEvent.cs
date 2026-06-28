using AshDefender.Shared.Core;

namespace AshDefender.Shared.Events
{
    public record GameSavedEvent(string SaveSlot) : IGameEvent;
}
