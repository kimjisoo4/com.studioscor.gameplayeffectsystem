using UnityEngine;

#if SCOR_ENABLE_GAMEPLAYTAG
using StudioScor.GameplayTagSystem;
#endif

namespace StudioScor.EffectSystem
{
    public abstract partial class GameplayEffect : ScriptableObject
    {

#if SCOR_ENABLE_GAMEPLAYTAG
        [field: Header(" [ Use GameplayTagSystem] ")]
        [field: SerializeField] public BaseEffectTags BaseEffectTags { get; private set; }

        [field: SerializeField, ContextMenuItem("Reset Effect Tags", "ResetEffectTags")] public FGameplayEffectTags EffectTags { get; private set; }

        public void ResetEffectTags()
        {
            EffectTags = new();
        }
#endif

        protected bool CheckGameplayTags(GameplayEffectSystem effectSystem)
        {
#if SCOR_ENABLE_GAMEPLAYTAG
            if (!HasRequiredTags(effectSystem.GameplayTagSystemComponent) 
                || HasObstacledTags(effectSystem.GameplayTagSystemComponent))
                return false;

            return true;
#else
            return true;
#endif
        }

#if SCOR_ENABLE_GAMEPLAYTAG
        protected bool HasRequiredTags(GameplayTagSystemComponent gameplayTagSystemComponent)
        {

            return (EffectTags.ActivateConditionTags.Requireds is null || gameplayTagSystemComponent.ContainAllTagsInOwned(EffectTags.ActivateConditionTags.Requireds))
                && (BaseEffectTags.Tags.ActivateConditionTags.Requireds is null || gameplayTagSystemComponent.ContainAllTagsInOwned(BaseEffectTags.Tags.ActivateConditionTags.Requireds));
        }
        protected bool HasObstacledTags(GameplayTagSystemComponent gameplayTagSystemComponent)
        {
            return (EffectTags.ActivateConditionTags.Obstacleds is not null && gameplayTagSystemComponent.ContainAnyTagsInOwned(EffectTags.ActivateConditionTags.Obstacleds))
                || (BaseEffectTags.Tags.ActivateConditionTags.Obstacleds is not null && gameplayTagSystemComponent.ContainAnyTagsInOwned(BaseEffectTags.Tags.ActivateConditionTags.Obstacleds));
        }
#endif
    }
}

