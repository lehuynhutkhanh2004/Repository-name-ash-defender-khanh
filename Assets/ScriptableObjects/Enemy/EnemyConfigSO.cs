using UnityEngine;

namespace AshDefender.ScriptableObjects
{
    public enum EnemyType
    {
        Slime,
        Goblin,
        Spider,
        MushroomMonster,
        Skeleton,
        Bat,
        GoblinWarrior,
        IceSlime,
        IceGoblin,
        SnowWolf,
        DemonSlime,
        DarkKnight,
        FireBat
    }

    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "AshDefender/ScriptableObjects/EnemyConfigSO")]
    public class EnemyConfigSO : ScriptableObject
    {
        [Header("Identity")]
        public string enemyId;
        public string enemyName;
        public Sprite icon;
        public EnemyType enemyType;

        [Header("Base Stats")]
        public int health;
        public int attack;
        public float moveSpeed;

        [Header("Reward")]
        public int reward;
    }
}
