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
        [SerializeField] protected GameplayTag _EffectTag;
        [SerializeField] private GameplayTag[] _AttributeTags;
        [SerializeField] private FConditionTags _ConditionTags;
        [SerializeField] private FGameplayTags _GrantTags;
        [SerializeField] private GameplayTag[] _CancelEffectTags;
        public GameplayTag EffectTag => _EffectTag;
        public IReadOnlyCollection<GameplayTag> AttributeTags => _AttributeTags;
        public FConditionTags ConditionTags => _ConditionTags;
        public FGameplayTags GrantTags => _GrantTags;
        public IReadOnlyCollection<GameplayTag> CancelEffectTags => _CancelEffectTags;
    }

    public abstract class GASGameplayEffectSpec : GameplayEffectSpec
    {
        protected new GASGameplayEffect _GameplayEffect;
        protected IGameplayTagSystem _GameplayTagSystem;

        public override void SetupSpec(GameplayEffect gameplayEffect, IGameplayEffectSystem gameplayEffectSystem, GameObject instigator, int level = 0, object data = null)
        {
            base.SetupSpec(gameplayEffect, gameplayEffectSystem, instigator, level, data);

            this._GameplayEffect = gameplayEffect as GASGameplayEffect;
            _GameplayTagSystem = gameplayEffectSystem.gameObject.GetGameplayTagSystem();
        }

        public override bool CanTakeEffect()
        {
            if (!base.CanTakeEffect())
                return false;

            if (_GameplayTagSystem.ContainBlockTag(_GameplayEffect.EffectTag))
            {
                Log($"Block Effect Tag");

                return false;
            }
            if (_GameplayTagSystem.ContainAnyTagsInBlock(_GameplayEffect.AttributeTags))
            {
                Log($"Block Atrribute Tags");

                return false;
            }

            if(!_GameplayTagSystem.ContainConditionTags(_GameplayEffect.ConditionTags))
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

            if (gameplayTags.Contains(_GameplayEffect.EffectTag))
            {
                Log($"Remove Effect From Source Contain EffectTag [{_GameplayEffect.EffectTag.name}]");

                return true;
            }

            foreach (var tag in gameplayTags)
            {
                if (_GameplayEffect.AttributeTags.Contains(tag))
                {
                    Log($"Remove Effect From Source [{tag.name}]");

                    return true;
                }
            }

            return false;
        }

        protected override void OnEnterEffect()
        {
            GameplayEffectSystem.CancelEffectFromSource(_GameplayEffect.CancelEffectTags);

            _GameplayTagSystem.GrantGameplayTags(_GameplayEffect.GrantTags);

        }
        protected override void OnExitEffect()
        {
            _GameplayTagSystem.RemoveGameplayTags(_GameplayEffect.GrantTags);
        }
    }
}

#endif