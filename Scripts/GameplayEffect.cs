using UnityEngine;

namespace KimScor.GameplayTagSystem.Effect
{
    public abstract class GameplayEffect : ScriptableObject
    {
        [Header("이펙트의 태그")]
        public FGameplayEffectTags EffectTags;

        [Header("지속 여부")]
        public EDurationPolicy DurationPolicy;

        public float Duration = 0.0f;

        [Header("업데이트 사용 여부")]
        public EUpdateType UpdateType;

        [Header(" 무시중 업데이트 여부 ")]
        public bool CanIgnoreUpdated = false;

        [Header(" 디버그 ")]
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

