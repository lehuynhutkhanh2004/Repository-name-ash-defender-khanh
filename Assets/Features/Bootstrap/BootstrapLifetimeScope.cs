using AshDefender.Features.Combat.Application.UseCases;
using AshDefender.Features.Combat.Domain.Interfaces;
using AshDefender.Features.Combat.Infrastructure.Services;
using AshDefender.Features.CommanderSkill.Application.UseCases;
using AshDefender.Features.CommanderSkill.Domain.Interfaces;
using AshDefender.Features.CommanderSkill.Infrastructure.Repositories;
using AshDefender.Features.CoreDefense.Domain.Interfaces;
using AshDefender.Features.CoreDefense.Infrastructure.Repositories;
using AshDefender.Features.Deployment.Application.UseCases;
using AshDefender.Features.Deployment.Application.Validators;
using AshDefender.Features.Enemy.Domain.Interfaces;
using AshDefender.Features.Enemy.Infrastructure.Repositories;
using AshDefender.Features.Hero.Domain.Interfaces;
using AshDefender.Features.Hero.Infrastructure.Repositories;
using AshDefender.Features.Reward.Application.UseCases;
using AshDefender.Features.Reward.Domain.Interfaces;
using AshDefender.Features.Reward.Infrastructure.Services;
using AshDefender.Features.SaveSystem.Application.UseCases;
using AshDefender.Features.SaveSystem.Domain.Interfaces;
using AshDefender.Features.SaveSystem.Infrastructure.SaveSystem;
using AshDefender.Features.Stage.Application.Interfaces;
using AshDefender.Features.Stage.Application.UseCases;
using AshDefender.Features.Stage.Domain.Interfaces;
using AshDefender.Features.Stage.Infrastructure.Repositories;
using AshDefender.Features.Unit.Domain.Interfaces;
using AshDefender.Features.Unit.Infrastructure.Repositories;
using AshDefender.Features.Upgrade.Application.UseCases;
using AshDefender.Features.Upgrade.Application.Validators;
using AshDefender.Features.Upgrade.Domain.Interfaces;
using AshDefender.Features.Upgrade.Infrastructure.Services;
using AshDefender.Features.Wave.Application.UseCases;
using AshDefender.Features.Wave.Domain.Interfaces;
using AshDefender.Features.Wave.Infrastructure.Repositories;
using AshDefender.ScriptableObjects;
using AshDefender.Shared.Configurations;
using AshDefender.Shared.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AshDefender.Features.Bootstrap
{
    public class BootstrapLifetimeScope : LifetimeScope
    {
        [Header("ScriptableObjects")]
        [SerializeField] private HeroConfigSO[] heroConfigs;
        [SerializeField] private UnitConfigSO[] unitConfigs;
        [SerializeField] private EnemyConfigSO[] enemyConfigs;
        [SerializeField] private SkillConfigSO[] skillConfigs;
        [SerializeField] private StageConfigSO[] stageConfigs;
        [SerializeField] private GameConfig gameConfig;

        [Header("Core HP")]
        [SerializeField] private int coreMaxHP = 100;

        protected override void Configure(IContainerBuilder builder)
        {
            // Shared
            builder.Register<EventBus>(Lifetime.Singleton).As<IEventBus>();
            builder.Register<SceneLoader>(Lifetime.Singleton);

            // Hero
            builder.Register(_ => new HeroRepository(heroConfigs), Lifetime.Singleton).As<IHeroRepository>();

            // Unit
            builder.Register(_ => new UnitRepository(unitConfigs), Lifetime.Singleton).As<IUnitRepository>();

            // Enemy
            builder.Register(_ => new EnemyRepository(enemyConfigs), Lifetime.Singleton).As<IEnemyRepository>();

            // CommanderSkill
            builder.Register(_ => new CommanderSkillRepository(skillConfigs), Lifetime.Singleton).As<ICommanderSkillRepository>();
            builder.Register<CastCommanderSkillUseCase>(Lifetime.Singleton);

            // Stage
            builder.Register(_ => new StageRepository(stageConfigs), Lifetime.Singleton).As<IStageRepository>();
            builder.Register<StartStageUseCase>(Lifetime.Singleton);
            builder.Register<CompleteStageUseCase>(Lifetime.Singleton);

            // Wave
            builder.Register(_ => new WaveRepository(stageConfigs), Lifetime.Singleton).As<IWaveRepository>();
            builder.Register<StartWaveUseCase>(Lifetime.Singleton);

            // CoreDefense
            builder.Register(_ => new CoreRepository(coreMaxHP), Lifetime.Singleton).As<ICoreRepository>();

            // Combat
            builder.Register<CombatService>(Lifetime.Singleton).As<ICombatService>();
            builder.Register<ProcessAttackUseCase>(Lifetime.Singleton);

            // Deployment
            builder.Register<DeploymentValidator>(Lifetime.Singleton);
            builder.Register<DeployUnitUseCase>(Lifetime.Singleton);

            // Reward
            builder.Register<RewardService>(Lifetime.Singleton).As<IRewardService>();
            builder.Register<GrantRewardUseCase>(Lifetime.Singleton);

            // Upgrade
            builder.Register<UpgradeService>(Lifetime.Singleton).As<IUpgradeService>();
            builder.Register<UpgradeValidator>(Lifetime.Singleton);
            builder.Register<UpgradeHeroUseCase>(Lifetime.Singleton);
            builder.Register<UpgradeUnitUseCase>(Lifetime.Singleton);
            builder.Register<UpgradeCommanderUseCase>(Lifetime.Singleton);

            // SaveSystem
            builder.Register<JsonSaveService>(Lifetime.Singleton).As<ISaveService>();
            builder.Register<SaveProgressUseCase>(Lifetime.Singleton);
        }
    }
}
