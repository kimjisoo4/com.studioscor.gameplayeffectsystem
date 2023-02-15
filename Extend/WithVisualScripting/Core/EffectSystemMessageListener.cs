#if SCOR_ENABLE_VISUALSCRIPTING
using UnityEngine;
using Unity.VisualScripting;

namespace StudioScor.EffectSystem.VisualScripting
{
    public static class EffectSystemWithVisualScripting
    {
        public const string EFFECTSYSTEM_ON_GRANTED_EFFECT = "OnGrantedEffect";
        public const string EFFECTSYSTEM_ON_REMOVED_EFFECT = "OnRemovedEffect";
    }
    [DisableAnnotation]
    [AddComponentMenu("")]
    [IncludeInSettings(false)]
    public sealed class EffectSystemMessageListener : MessageListener
    {
        private void Awake()
        {
            var effectSystem = GetComponent<EffectSystemComponent>();

            effectSystem.OnGrantedEffect += EffectSystem_OnGrantedEffect;
            effectSystem.OnRemovedEffect += EffectSystem_OnRemovedEffect;
        }

        

        private void OnDestroy()
        {
            if (TryGetComponent(out EffectSystemComponent effectSystem))
            {
                effectSystem.OnGrantedEffect -= EffectSystem_OnGrantedEffect;
                effectSystem.OnRemovedEffect -= EffectSystem_OnRemovedEffect;
            }
        }

        private void EffectSystem_OnRemovedEffect(EffectSystemComponent effectSystem, IEffectSpec effectSpec)
        {
            EventBus.Trigger(new EventHook(EffectSystemWithVisualScripting.EFFECTSYSTEM_ON_REMOVED_EFFECT, effectSystem), effectSpec);
        }

        private void EffectSystem_OnGrantedEffect(EffectSystemComponent effectSystem, IEffectSpec effectSpec)
        {
            EventBus.Trigger(new EventHook(EffectSystemWithVisualScripting.EFFECTSYSTEM_ON_GRANTED_EFFECT, effectSystem), effectSpec);
        }

    }
}

#endif