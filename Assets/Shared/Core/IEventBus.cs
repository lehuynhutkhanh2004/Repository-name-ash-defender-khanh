using System;

namespace AshDefender.Shared.Core
{
    public interface IEventBus
    {
        void Publish<T>(T gameEvent) where T : IGameEvent;
        IDisposable Subscribe<T>(Action<T> handler) where T : IGameEvent;
        void Unsubscribe<T>(Action<T> handler) where T : IGameEvent;
    }
}
