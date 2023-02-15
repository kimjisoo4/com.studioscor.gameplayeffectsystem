#if SCOR_ENABLE_VISUALSCRIPTING
using System;
using Unity.VisualScripting;
using StudioScor.Utilities.VisualScripting;

namespace StudioScor.GameplayEffectSystem.VisualScripting
{
    public abstract class EffectSystemEventUnit: CustomEventUnit<GameplayEffectSystemComponent, IGameplayEffectSpec>
    {
        [DoNotSerialize]
        [PortLabel("EffectSpec")]
        public ValueOutput EffectSpec { get; private set; }

        public override Type MessageListenerType => typeof(EffectSystemMessageListener);

        protected override void Definition()
        {
            base.Definition();

            EffectSpec = ValueOutput<IGameplayEffectSpec>(nameof(EffectSpec));
        }

        protected override void AssignArguments(Flow flow, IGameplayEffectSpec data)
        {
            flow.SetValue(EffectSpec, data);
        }
    }
}

#endif