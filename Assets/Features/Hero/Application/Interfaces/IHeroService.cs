using System.Collections.Generic;
using AshDefender.Features.Hero.Application.DTOs;

namespace AshDefender.Features.Hero.Application.Interfaces
{
    public interface IHeroService
    {
        HeroDTO GetHero(string heroId);
        IReadOnlyList<HeroDTO> GetAllHeroes();
        IReadOnlyList<HeroDTO> GetUnlockedHeroes();
    }
}
