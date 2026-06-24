using System.Collections.Generic;
using UnityEngine;

namespace AshDefender.ScriptableObjects
{
    [CreateAssetMenu(fileName = "HeroConfig", menuName = "AshDefender/ScriptableObjects/HeroConfigSO")]
    public class HeroConfigSO : ScriptableObject
    {
        [Header("Identity")]
        public string heroId;
        public string heroName;
        public Sprite icon;

        [Header("Base Stats")]
        public int baseHealth;
        public int baseAttack;
        public int baseDefense;

        [Header("Skills")]
        public List<SkillConfigSO> skillSet = new();
    }
}
