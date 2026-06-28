using UnityEngine;

namespace AshDefender.Shared.Configurations
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "AshDefender/Configurations/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Combat")]
        public float criticalHitMultiplier = 2f;
        public float criticalHitChance = 0.05f;

        [Header("Energy")]
        public int maxEnergy = 10;
        public float energyRegenRate = 1f;

        [Header("Squad")]
        public int maxSquadSize = 4;

        [Header("Wave")]
        public float timeBetweenWaves = 5f;
        public float bossWarningDuration = 3f;

        [Header("Save")]
        public string defaultSaveSlot = "slot_0";
    }
}
