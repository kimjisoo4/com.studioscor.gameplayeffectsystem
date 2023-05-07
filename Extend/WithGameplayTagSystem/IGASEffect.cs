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
        [SerializeField] private GameplayTag _EffectTag;
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
        private readonly IGASGameplayEffect _GASEffect;
        private IGameplayTagSystem _GameplayTagSystem;

        public IGASGameplayEffect GASEffect => _GASEffect;
        public IGameplayTagSystem GameplayTagSystem => _GameplayTagSystem;



        protected GASGameplayEffectSpec(GameplayEffect effect) : base(effect)
        {
            _GASEffect = effect as IGASGameplayEffect;
        }

        public override void SetupSpec(IGameplayEffectSystem gameplayEffectSystem, int level = 0, object data = null)
        {
            base.SetupSpec(gameplayEffectSystem, level, data);

            _GameplayTagSystem = gameplayEffectSystem.transform.GetComponent<IGameplayTagSystem>();
        }

        public override bool CanTakeEffect()
        {
            if (!base.CanTakeEffect())
                return false;

            if (GameplayTagSystem.ContainBlockTag(GASEffect.EffectTag))
            {
                Log($"Block Effect Tag");

                return false;
            }
            if (GameplayTagSystem.ContainAnyTagsInBlock(GASEffect.AttributeTags))
            {
                Log($"Block Atrribute Tags");

                return false;
            }

            if (!GameplayTagSystem.ContainAllTagsInOwned(GASEffect.ConditionTags.Requireds))
            {
                Log($"Not Contained All Reauired Tags In Owned");

                return false;
            }
            if (GameplayTagSystem.ContainAnyTagsInOwned(GASEffect.ConditionTags.Obstacleds))
            {
                Log($"Contained Any Obstacled Tags In Owned");

                return false;
            }

            return true;
        }

        public override bool CanRemoveEffectFromSource(object source)
        {
            Log(" Can Remove Effect From Source ? ");

            IReadOnlyCollection<GameplayTag> gameplayTags = source as IReadOnlyCollection<GameplayTag>;

            if (gameplayTags is null)
                return false;

            if (gameplayTags.Contains(GASEffect.EffectTag))
            {
                Log($"Remove Effect From Source Contain EffectTag [{GASEffect.EffectTag.name}]");

                return true;
            }

            foreach (var tag in gameplayTags)
            {
                if (GASEffect.AttributeTags.Contains(tag))
                {
                    Log($"Remove Effect From Source [{tag.name}]");

                    return true;
                }
            }

            return false;
        }

        protected override void OnEnterEffect()
        {
            GameplayEffectSystem.RemoveEffectFromSource(GASEffect.CancelEffectTags);

            GameplayTagSystem.AddOwnedTags(GASEffect.GrantTags.Owneds);
            GameplayTagSystem.AddBlockTags(GASEffect.GrantTags.Blocks);

        }
        protected override void OnExitEffect()
        {
            GameplayTagSystem.RemoveOwnedTags(GASEffect.GrantTags.Owneds);
            GameplayTagSystem.RemoveBlockTags(GASEffect.GrantTags.Blocks);

            if (_UsePool)
                _GameplayEffect.ReleaseSpec(this);
        }
    }
}

#endif