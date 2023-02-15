#if SCOR_ENABLE_VISUALSCRIPTING
using System;
using Unity.VisualScripting;
using StudioScor.Utilities.VisualScripting;

namespace StudioScor.EffectSystem.VisualScripting
{
    public abstract class EffectSystemEventUnit: CustomEventUnit<EffectSystemComponent, IEffectSpec>
    {
        [DoNotSerialize]
        [PortLabel("EffectSpec")]
        public ValueOutput EffectSpec { get; private set; }

        public override Type MessageListenerType => typeof(EffectSystemMessageListener);

        protected override void Definition()
        {
            base.Definition();

            EffectSpec = ValueOutput<IEffectSpec>(nameof(EffectSpec));
        }

        protected override void AssignArguments(Flow flow, IEffectSpec data)
        {
            flow.SetValue(EffectSpec, data);
        }
    }
}

#endif