#if SCOR_ENABLE_GAMEPLAYTAGSYSTEM
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using StudioScor.GameplayTagSystem;

namespace StudioScor.GameplayEffectSystem
{
    public interface IGASEffect
    {
        public GameplayTag EffectTag { get; }
        public IReadOnlyCollection<GameplayTag> AttributeTags { get; }
        public FConditionTags ConditionTags { get; }
        public FGameplayTags GrantTags { get; }
        public IReadOnlyCollection<GameplayTag> CancelEffectTags { get; }
    }
    public interface IGASGameplayEffect
    {
        public GameplayTag EffectTag { get; }
        public IReadOnlyCollection<GameplayTag> AttributeTags { get; }
        public FConditionTags ConditionTags { get; }
        public FGameplayTags GrantTags { get; }
        public IReadOnlyCollection<GameplayTag> CancelEffectTags { get; }
    }

    public abstract class GASGameplayEffect : GameplayEffect, IGASGameplayEffect
    {
        [Header(" [ Character Effect ] ")]
        [SerializeField]private GameplayTag effectTag;
        [SerializeField]private GameplayTag[] attributeTags;
        [SerializeField] private FConditionTags conditionTags;
        [SerializeField] private FGameplayTags grantTags;
        [SerializeField] private GameplayTag[] cancelEffectTags;
        public GameplayTag EffectTag => effectTag;
        public IReadOnlyCollection<GameplayTag> AttributeTags => attributeTags;
        public FConditionTags ConditionTags => conditionTags;
        public FGameplayTags GrantTags => grantTags;
        public IReadOnlyCollection<GameplayTag> CancelEffectTags => cancelEffectTags;
    }

    public abstract class GASGameplayEffectSpec : GameplayEffectSpec
    {
        protected new GASGameplayEffect gameplayEffect;
        protected IGameplayTagSystem gameplayTagSystem;

        public override void SetupSpec(GameplayEffect gameplayEffect, IGameplayEffectSystem gameplayEffectSystem, int level = 0, object data = null)
        {
            base.SetupSpec(gameplayEffect, gameplayEffectSystem, level, data);

            this.gameplayEffect = gameplayEffect as GASGameplayEffect;
            gameplayTagSystem = gameplayEffectSystem.gameObject.GetGameplayTagSystem();
        }

        public override bool CanTakeEffect()
        {
            if (!base.CanTakeEffect())
                return false;

            if (gameplayTagSystem.ContainBlockTag(gameplayEffect.EffectTag))
            {
                Log($"Block Effect Tag");

                return false;
            }
            if (gameplayTagSystem.ContainAnyTagsInBlock(gameplayEffect.AttributeTags))
            {
                Log($"Block Atrribute Tags");

                return false;
            }

            if(!gameplayTagSystem.ContainConditionTags(gameplayEffect.ConditionTags))
            {
                Log($"Contain Condition Tags is False");

                return false;
            }

            return true;
        }

        public override bool CancelEffectFromSource(object source)
        {
            Log(" Can Remove Effect From Source ? ");

            IReadOnlyCollection<GameplayTag> gameplayTags = source as IReadOnlyCollection<GameplayTag>;

            if (gameplayTags is null)
                return false;

            if (gameplayTags.Contains(gameplayEffect.EffectTag))
            {
                Log($"Remove Effect From Source Contain EffectTag [{gameplayEffect.EffectTag.name}]");

                return true;
            }

            foreach (var tag in gameplayTags)
            {
                if (gameplayEffect.AttributeTags.Contains(tag))
                {
                    Log($"Remove Effect From Source [{tag.name}]");

                    return true;
                }
            }

            return false;
        }

        protected override void OnEnterEffect()
        {
            GameplayEffectSystem.CancelEffectFromSource(gameplayEffect.CancelEffectTags);

            gameplayTagSystem.GrantGameplayTags(gameplayEffect.GrantTags);

        }
        protected override void OnExitEffect()
        {
            gameplayTagSystem.RemoveGameplayTags(gameplayEffect.GrantTags);
        }
    }
}

#endif