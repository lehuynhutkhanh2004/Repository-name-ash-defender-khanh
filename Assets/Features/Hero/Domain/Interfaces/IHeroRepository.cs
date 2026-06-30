using System.Collections.Generic;
using HeroEntity = AshDefender.Features.Hero.Domain.Entities.Hero;

namespace AshDefender.Features.Hero.Domain.Interfaces
{
    public interface IHeroRepository
    {
        HeroEntity GetById(string heroId);
        IReadOnlyList<HeroEntity> GetAll();
        IReadOnlyList<HeroEntity> GetUnlocked();
        void Save(HeroEntity hero);
    }
}
