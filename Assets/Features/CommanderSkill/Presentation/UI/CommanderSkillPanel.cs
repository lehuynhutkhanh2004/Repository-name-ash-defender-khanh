using AshDefender.Features.CommanderSkill.Application.UseCases;
using AshDefender.Shared.Exceptions;
using UnityEngine;
using VContainer;

namespace AshDefender.Features.CommanderSkill.Presentation.UI
{
    public class CommanderSkillPanel : MonoBehaviour
    {
        private CastCommanderSkillUseCase _castSkillUseCase;
        private int _currentMana;

        [Inject]
        public void Construct(CastCommanderSkillUseCase castSkillUseCase)
        {
            _castSkillUseCase = castSkillUseCase;
        }

        public void UpdateMana(int mana) => _currentMana = mana;

        public void OnSkillButtonClicked(string skillId)
        {
            try
            {
                _castSkillUseCase.Execute(skillId, _currentMana);
            }
            catch (GameException e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }
}
