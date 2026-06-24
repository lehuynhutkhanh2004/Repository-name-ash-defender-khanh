using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshDefender.ScriptableObjects
{
    [Serializable]
    public class WaveConfig
    {
        public EnemyConfigSO enemyConfig;
        public int spawnCount;
        public float spawnInterval;
    }

    [CreateAssetMenu(fileName = "StageConfig", menuName = "AshDefender/ScriptableObjects/StageConfigSO")]
    public class StageConfigSO : ScriptableObject
    {
        [Header("Identity")]
        public string stageId;
        public string stageName;

        [Header("Waves")]
        public List<WaveConfig> waves = new();

        [Header("Boss")]
        public EnemyConfigSO bossConfig;

        [Header("Unlock")]
        public StageConfigSO requiredPreviousStage;
    }
}
