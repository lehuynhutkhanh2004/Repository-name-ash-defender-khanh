using UnityEngine;

namespace AshDefender.ScriptableObjects
{
    public enum UnitType
    {
        Melee,
        Ranged,
        Tank,
        Support
    }

    [CreateAssetMenu(fileName = "UnitConfig", menuName = "AshDefender/ScriptableObjects/UnitConfigSO")]
    public class UnitConfigSO : ScriptableObject
    {
        [Header("Identity")]
        public string unitId;
        public string unitName;
        public Sprite icon;
        public UnitType unitType;

        [Header("Deployment")]
        public int cost;

        [Header("Base Stats")]
        public int health;
        public int attack;
        public float moveSpeed;
    }
}
