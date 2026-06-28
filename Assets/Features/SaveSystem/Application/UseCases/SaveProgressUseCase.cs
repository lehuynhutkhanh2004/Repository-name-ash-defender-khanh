using AshDefender.Features.SaveSystem.Domain.Entities;
using AshDefender.Features.SaveSystem.Domain.Interfaces;
using AshDefender.Shared.Core;
using AshDefender.Shared.Events;
using Cysharp.Threading.Tasks;

namespace AshDefender.Features.SaveSystem.Application.UseCases
{
    public class SaveProgressUseCase
    {
        private readonly ISaveService _saveService;
        private readonly IEventBus _eventBus;

        public SaveProgressUseCase(ISaveService saveService, IEventBus eventBus)
        {
            _saveService = saveService;
            _eventBus = eventBus;
        }

        public async UniTask ExecuteAsync(SaveData data, string slot = "slot_0")
        {
            await _saveService.SaveAsync(data, slot);
            _eventBus.Publish(new GameSavedEvent(slot));
        }
    }
}
