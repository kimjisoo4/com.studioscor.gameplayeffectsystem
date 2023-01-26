using System.Linq;
using System.Diagnostics;

#if SCOR_ENABLE_GAMEPLAYTAG
using StudioScor.GameplayTagSystem;
#endif

namespace StudioScor.EffectSystem
{
    public abstract partial class GameplayEffectSpec
    {
#if SCOR_ENABLE_GAMEPLAYTAG
        public GameplayTagSystemComponent GameplayTagSystemComponent => GameplayEffectSystem.GameplayTagSystemComponent;
        
        public FGameplayEffectTags BaseEffectTags => GameplayEffect.BaseEffectTags.Tags;

        public FGameplayEffectTags EffectTags => GameplayEffect.EffectTags;
#endif
        [Conditional("SCOR_ENABLE_GAMEPLAYTAG")]
        private void ActivateEffectWithGameplayTag()
        {
#if SCOR_ENABLE_GAMEPLAYTAG
            CancelGameplayEffectWithGameplayTag();
            AddActivatedTags();

            if (HasApplyConditionTag())
            {
                GameplayEffectSystem.GameplayTagSystemComponent.OnGrantedOwnedTag += GameplayTagSystem_OnUpdateOwnedTag;
                GameplayEffectSystem.GameplayTagSystemComponent.OnRemovedOwnedTag += GameplayTagSystem_OnUpdateOwnedTag;
            }
#endif
        }

        [Conditional("SCOR_ENABLE_GAMEPLAYTAG")]
        private void EndGameplayEffectWithGameplayTag()
        {
#if SCOR_ENABLE_GAMEPLAYTAG
            if (HasApplyConditionTag())
            {
                GameplayEffectSystem.GameplayTagSystemComponent.OnGrantedOwnedTag -= GameplayTagSystem_OnUpdateOwnedTag;
                GameplayEffectSystem.GameplayTagSystemComponent.OnRemovedOwnedTag -= GameplayTagSystem_OnUpdateOwnedTag;
            }

            RemoveActivatedTags();
#endif
        }
        [Conditional("SCOR_ENABLE_GAMEPLAYTAG")]
        private void ApplyEffectWithGameplayTag()
        {
#if SCOR_ENABLE_GAMEPLAYTAG
            AddApplyedTags();
#endif
        }
        [Conditional("SCOR_ENABLE_GAMEPLAYTAG")]
        private void IgnoreEffectWithGameplayTag()
        {
#if SCOR_ENABLE_GAMEPLAYTAG
            RemoveApplyedTags();
#endif
        }

#if SCOR_ENABLE_GAMEPLAYTAG
        private void GameplayTagSystem_OnUpdateOwnedTag(GameplayTagSystemComponent gameplayTagSystemComponent, GameplayTag changedTag)
        {
            if (EffectTags.ApplyConditionTags.Obstacleds.Contains(changedTag) || EffectTags.ApplyConditionTags.Requireds.Contains(changedTag)
             || BaseEffectTags.ApplyConditionTags.Obstacleds.Contains(changedTag) || BaseEffectTags.ApplyConditionTags.Requireds.Contains(changedTag))
            {
                TryApplyEffect();
            }
        }
        private void CancelGameplayEffectWithGameplayTag()
        {
            GameplayEffectSystem.CancelGameplayEffectWithTags(EffectTags.RemoveGameplayEffectsAsTags);
            GameplayEffectSystem.CancelGameplayEffectWithTags(BaseEffectTags.RemoveGameplayEffectsAsTags);
        }

        private void AddActivatedTags()
        {
            GameplayEffectSystem.GameplayTagSystemComponent.AddOwnedTags(EffectTags.ActivateGrantedTags.Owneds);
            GameplayEffectSystem.GameplayTagSystemComponent.AddOwnedTags(BaseEffectTags.ActivateGrantedTags.Owneds);

            GameplayEffectSystem.GameplayTagSystemComponent.AddBlockTags(EffectTags.ActivateGrantedTags.Blocks);
            GameplayEffectSystem.GameplayTagSystemComponent.AddBlockTags(BaseEffectTags.ActivateGrantedTags.Blocks);
        }
        private void RemoveActivatedTags()
        {
            GameplayEffectSystem.GameplayTagSystemComponent.RemoveOwnedTags(EffectTags.ActivateGrantedTags.Owneds);
            GameplayEffectSystem.GameplayTagSystemComponent.RemoveOwnedTags(BaseEffectTags.ActivateGrantedTags.Owneds);

            GameplayEffectSystem.GameplayTagSystemComponent.RemoveBlockTags(EffectTags.ActivateGrantedTags.Blocks);
            GameplayEffectSystem.GameplayTagSystemComponent.RemoveBlockTags(BaseEffectTags.ActivateGrantedTags.Blocks);
        }
        private void AddApplyedTags()
        {
            GameplayEffectSystem.GameplayTagSystemComponent.AddOwnedTags(EffectTags.ApplyGrantedTags.Owneds);
            GameplayEffectSystem.GameplayTagSystemComponent.AddOwnedTags(BaseEffectTags.ApplyGrantedTags.Owneds);

            GameplayEffectSystem.GameplayTagSystemComponent.AddBlockTags(EffectTags.ApplyGrantedTags.Blocks);
            GameplayEffectSystem.GameplayTagSystemComponent.AddBlockTags(BaseEffectTags.ApplyGrantedTags.Blocks);
        }
        private void RemoveApplyedTags()
        {
            GameplayEffectSystem.GameplayTagSystemComponent.RemoveOwnedTags(EffectTags.ApplyGrantedTags.Owneds);
            GameplayEffectSystem.GameplayTagSystemComponent.RemoveOwnedTags(BaseEffectTags.ApplyGrantedTags.Owneds);

            GameplayEffectSystem.GameplayTagSystemComponent.RemoveBlockTags(EffectTags.ApplyGrantedTags.Blocks);
            GameplayEffectSystem.GameplayTagSystemComponent.RemoveBlockTags(BaseEffectTags.ApplyGrantedTags.Blocks);
        }

        protected bool HasApplyConditionTag()
        {
            return EffectTags.ApplyConditionTags.Requireds.Length > 0 || EffectTags.ApplyConditionTags.Obstacleds.Length > 0
                || BaseEffectTags.ApplyConditionTags.Requireds.Length > 0 || BaseEffectTags.ApplyConditionTags.Obstacleds.Length > 0;
        }
#endif
        protected virtual bool CanApplyConditionTags()
        {
        
#if SCOR_ENABLE_GAMEPLAYTAG
            return HasRequiredApplyTags() && !HasObstacledApplyTags();
#else
            return true;
#endif
        }

#if SCOR_ENABLE_GAMEPLAYTAG
        protected bool HasRequiredApplyTags()
        {
            return (EffectTags.ApplyConditionTags.Requireds is null || GameplayTagSystemComponent.ContainAllTagsInOwned(EffectTags.ApplyConditionTags.Requireds))
                && (BaseEffectTags.ApplyConditionTags.Requireds is null || GameplayTagSystemComponent.ContainAllTagsInOwned(BaseEffectTags.ApplyConditionTags.Requireds));
        }
        protected bool HasObstacledApplyTags()
        {
            return (EffectTags.ApplyConditionTags.Obstacleds is not null && GameplayTagSystemComponent.ContainAnyTagsInOwned(EffectTags.ApplyConditionTags.Obstacleds))
                || (BaseEffectTags.ApplyConditionTags.Obstacleds is not null && GameplayTagSystemComponent.ContainAnyTagsInOwned(BaseEffectTags.ApplyConditionTags.Obstacleds));
        }
#endif
    }
}
