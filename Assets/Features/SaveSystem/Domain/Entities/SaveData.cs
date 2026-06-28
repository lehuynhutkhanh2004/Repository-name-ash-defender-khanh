using System.Collections.Generic;

namespace AshDefender.Features.SaveSystem.Domain.Entities
{
    public class SaveData
    {
        public string PlayerId { get; set; } = "player_0";
        public int CurrentStageIndex { get; set; } = 1;
        public List<string> UnlockedHeroIds { get; set; } = new();
        public Dictionary<string, int> HeroLevels { get; set; } = new();
        public Dictionary<string, int> UnitLevels { get; set; } = new();
        public Dictionary<string, int> CommanderSkillLevels { get; set; } = new();
        public List<string> CompletedStageIds { get; set; } = new();
        public Dictionary<string, int> Inventory { get; set; } = new();
        public int Gold { get; set; }
        public int SoulFragments { get; set; }
        public float MasterVolume { get; set; } = 1f;
        public float MusicVolume { get; set; } = 0.8f;
        public float SfxVolume { get; set; } = 1f;
    }
}
