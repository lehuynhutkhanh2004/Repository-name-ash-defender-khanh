using AshDefender.Features.Stage.Domain.Enums;

namespace AshDefender.Features.Stage.Domain.Entities
{
    public class Stage
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int StageNumber { get; private set; }
        public string RequiredPreviousStageId { get; private set; }
        public StageState State { get; private set; }
        public int BestScore { get; private set; }

        public bool IsUnlocked => State != StageState.Locked;
        public bool IsCompleted => State == StageState.Completed;

        public Stage(string id, string name, int stageNumber, string requiredPreviousStageId = null)
        {
            Id = id;
            Name = name;
            StageNumber = stageNumber;
            RequiredPreviousStageId = requiredPreviousStageId;
            State = stageNumber == 1 ? StageState.Available : StageState.Locked;
        }

        public void Unlock() => State = StageState.Available;

        public void Complete(int score)
        {
            State = StageState.Completed;
            if (score > BestScore) BestScore = score;
        }
    }
}
