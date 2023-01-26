using UnityEngine;

#if SCOR_ENABLE_GAMEPLAYTAG
namespace StudioScor.EffectSystem
{
    [CreateAssetMenu(menuName ="StudioScor/GAS/new BaseEffectTags", fileName ="BaseEffectTag_")]
    public class BaseEffectTags : ScriptableObject
    {
        public FGameplayEffectTags Tags;
    }
}
#endif
