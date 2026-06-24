using UnityEngine;

namespace AshDefender.ScriptableObjects
{
    public enum SkillType
    {
        Heal,
        Buff,
        AoEDamage,
        CrowdControl
    }

    [CreateAssetMenu(fileName = "SkillConfig", menuName = "AshDefender/ScriptableObjects/SkillConfigSO")]
    public class SkillConfigSO : ScriptableObject
    {
        [Header("Identity")]
        public string skillId;
        public string skillName;
        public Sprite icon;
        public SkillType skillType;

        [Header("Cost")]
        public int manaCost;
        public float cooldown;

        [Header("Effect")]
        public float power;
    }
}
