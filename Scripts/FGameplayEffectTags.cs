using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace KimScor.GameplayTagSystem.Effect
{
    [System.Serializable]   
    public struct FGameplayEffectTags
    {
        /// <summary>
        /// 이펙트의 태그
        /// </summary>
#if ODIN_INSPECTOR
        [BoxGroup("Attribute Tags")]
        [ListDrawerSettings(Expanded = true)]
#else
        [Header("이펙트 태그")]
#endif
        [SerializeField] public GameplayTag AssetTag;
#if ODIN_INSPECTOR
        [BoxGroup("Attribute Tags")]
        [ListDrawerSettings(Expanded = true)]
#else
        [Header("이펙트 속성")]
#endif
        [SerializeField] public GameplayTag[] AttributeTags;

        /// <summary>
        /// 이펙트가 적용 되는 동안 부여되는 태그
        /// </summary>
#if ODIN_INSPECTOR
        [BoxGroup("Activate Tags")]
        [ListDrawerSettings(Expanded = true)]
#else
        [Header("활성화 부여 태그")]
#endif
        [SerializeField] public GameplayTag[] ActivateGrantedTags;
        /// <summary>
        /// 이펙트가 부여되기 되기 위한 태그
        /// </summary>
#if ODIN_INSPECTOR
        [BoxGroup("Activate Tags")]
        [Title("Activate Condition")]
        [ListDrawerSettings(Expanded = true)]
#else
        [Header("활성화 필수 태그")]
#endif
        [SerializeField] public GameplayTag[] ActivateEffectRequiredTags;
#if ODIN_INSPECTOR
        [BoxGroup("Activate Tags")]
        [ListDrawerSettings(Expanded = true)]
#endif
        [SerializeField] public GameplayTag[] ActivateEffectIgnoreTags;


        /// <summary>
        /// 이펙트가 활성화 되는 동안 부여되는 태그
        /// </summary>
#if ODIN_INSPECTOR
        [BoxGroup("Apply Tags")]
        [ListDrawerSettings(Expanded = true)]
#else
        [Header("젹용 부여 태그")]
#endif
        [SerializeField] public GameplayTag[] ApplyGrantedTags;
        /// <summary>
        /// 이펙트가 활성화 되기 위한 태그
        /// </summary>
#if ODIN_INSPECTOR
        [BoxGroup("Apply Tags")]
        [Title("Apply Condition")]
        [ListDrawerSettings(Expanded = true)]
#else
        [Header("적용 필수 태그")]
#endif        
        [SerializeField] public GameplayTag[] ApplyEffectRequiredTags;
#if ODIN_INSPECTOR
        [BoxGroup("Apply Tags")]
        [ListDrawerSettings(Expanded = true)]
#endif
        [SerializeField] public GameplayTag[] ApplyEffectIgnoreTags;


        /// <summary>
        /// 이펙트가 부여될 때 제거할 이펙트의 태그
        /// </summary>
#if ODIN_INSPECTOR
        [BoxGroup("Remove Effect Tags")]
        [ListDrawerSettings(Expanded = true)]
#else
        [Header("태그로 이펙트를 제거")]
#endif
        [SerializeField] public GameplayTag[] RemoveGameplayEffectsWithTags;
    }
}
