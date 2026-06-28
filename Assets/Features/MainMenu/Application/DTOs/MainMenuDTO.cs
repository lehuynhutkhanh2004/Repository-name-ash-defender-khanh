namespace AshDefender.Features.MainMenu.Application.DTOs
{
    public record MainMenuDTO(bool HasSaveData, string LastPlayedStageId, int TotalGold);
}
