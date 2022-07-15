using UnityEngine;

namespace KimScor.GameplayTagSystem.Effect
{
    [System.Serializable]
    public struct FGameplayEffectTags
    {
        /// <summary>
        /// 이펙트의 태그
        /// </summary>
        [Header("이펙트 태그")]
        [SerializeField] public GameplayTag AssetTag;

        [Header("이펙트 속성")]
        [SerializeField] public GameplayTag[] AttributeTags;

        /// <summary>
        /// 이펙트가 적용 되는 동안 부여되는 태그
        /// </summary>
        [Header("활성화 부여 태그")]
        [SerializeField] public GameplayTag[] ActivateGrantedTags;
        /// <summary>
        /// 이펙트가 부여되기 되기 위한 태그
        /// </summary>
        [Header("활성화 필수 태그")]
        [SerializeField] public GameplayTag[] ActivateEffectRequiredTags;
        [SerializeField] public GameplayTag[] ActivateEffectIgnoreTags;


        /// <summary>
        /// 이펙트가 활성화 되는 동안 부여되는 태그
        /// </summary>
        [Header("젹용 부여 태그")]
        [SerializeField] public GameplayTag[] ApplyGrantedTags;
        /// <summary>
        /// 이펙트가 활성화 되기 위한 태그
        /// </summary>
        [Header("적용 필수 태그")]
        [SerializeField] public GameplayTag[] ApplyEffectRequiredTags;
        [SerializeField] public GameplayTag[] ApplyEffectIgnoreTags;

        /// <summary>
        /// 이펙트가 부여될 때 제거할 이펙트의 태그
        /// </summary>
        [Header("태그로 이펙트를 제거")]
        [SerializeField] public GameplayTag[] RemoveGameplayEffectsWithTags;
    }
}
