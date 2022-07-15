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
    }
}

