using UnityEngine;

#if SCOR_ENABLE_GAMEPLAYTAG
using StudioScor.GameplayTagSystem;

namespace StudioScor.EffectSystem
{
    [System.Serializable]   
    public struct FGameplayEffectTags
    {
        [Header(" [ Gameplay Effect Tags ] ")]

        [Header("태그")]
        public GameplayTag AssetTag;

        [Header("속성 태그")]
        public GameplayTag[] AttributeTags;

        [Space(10f)]

        [Header("활성화 조건 태그")]
        public FConditionTags ActivateConditionTags;
        [Header("활성화 부여 태그")]
        public FGameplayTags ActivateGrantedTags;

        [Space(10f)]

        [Header("적용 조건 태그")]
        public FConditionTags ApplyConditionTags;
        [Header("적용 부여 태그")]
        public FGameplayTags ApplyGrantedTags;

        [Space(10f)]

        [Header("이펙트 제거 태그")]
        public GameplayTag[] RemoveGameplayEffectsAsTags;
    }
}
#endif
