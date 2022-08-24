using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
namespace KimScor.GameplayTagSystem.Effect
{
    public abstract class GameplayEffect : ScriptableObject
    {
#if ODIN_INSPECTOR
        [TabGroup("Condition")]
        [InlineProperty]
        [HideLabel]
#else
        [Header("이펙트의 태그")]
#endif
        public FGameplayEffectTags EffectTags;

#if ODIN_INSPECTOR
        [TabGroup("Setting")]
        [EnumToggleButtons]
#else
        [Header("지속 여부")]
#endif
        public EDurationPolicy DurationPolicy;

#if ODIN_INSPECTOR
        [TabGroup("Setting")]
        [Title("DurationPolicy")]
        [ShowIf("DurationPolicy", EDurationPolicy.Duration)]
#endif
        public float Duration = 0.0f;

#if ODIN_INSPECTOR
        [TabGroup("Setting")]
        [Title("Debug Mode")]
#else
        [Header(" 디버그 ")]
#endif
        public bool DebugMode;

        public abstract GameplayEffectSpec CreateSpec(GameplayEffectSystem owner);

        public virtual bool CanActivateGameplayEffect(GameplayTagSystem targetGameplayTagSystem)
        {
            // 필요한 태그가 있으나 소유하고 있지 않다.
            if (EffectTags.ActivateEffectRequiredTags is not null
                && !targetGameplayTagSystem.ContainAllTagsInOwned(EffectTags.ActivateEffectRequiredTags))
                return false;

            // 방해 태그가 있고 방해 태그를 소유하고 있다.
            if (EffectTags.ActivateEffectIgnoreTags is not null
               && targetGameplayTagSystem.ContainOnceTagsInOwned(EffectTags.ActivateEffectIgnoreTags))
                return false;

            return true;
        }
    }
}

