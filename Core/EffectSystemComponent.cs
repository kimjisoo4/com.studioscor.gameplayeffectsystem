using System.Collections.Generic;
using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.EffectSystem
{
    [AddComponentMenu("StudioScor/EffectSystem/Effect System Component", order: 0)]
    public class EffectSystemComponent : BaseMonoBehaviour
    {
        #region Event
        public delegate void ChangeGameplayEffectHandler(EffectSystemComponent effectSystem, IEffectSpec effectSpec);
        #endregion

        [Header(" [ Effect System ] ")]
        private List<IEffectSpec> _Effects;
        public IReadOnlyList<IEffectSpec> Effects => _Effects;

        public event ChangeGameplayEffectHandler OnGrantedEffect;
        public event ChangeGameplayEffectHandler OnRemovedEffect;

        private void Awake()
        {
            Setup();
        }

        protected void Setup()
        {
            Log("Setup");

            _Effects = new();
        }
        protected virtual void OnSetup() { }

        public void ResetEffectSystem()
        {
            Log("Reset");

            RemoveAllEffect();

            _Effects.Clear();

            OnReset();
        }
        protected virtual void OnReset() { }



        private void Update()
        {
            float deltaTime = Time.deltaTime;

            for(int i = Effects.Count - 1; i >= 0; i --)
            {
                Effects[i].UpdateEffect(deltaTime);

                if (!Effects[i].IsActivate)
                {
                    RemoveEffectSpec(_Effects[i]);
                }
            }
        }

        public bool ContainEffect(GameplayEffect containEffect)
        {
            foreach (var effect in Effects)
            {
                if (effect.Effect == containEffect)
                {
                    return true;
                }
            }

            return false;
        }
        public bool TryGetEffectSpec(GameplayEffect containEffect, out IEffectSpec spec)
        {
            foreach (var effect in Effects)
            {
                if (effect.Effect == containEffect)
                {
                    spec = effect;

                    return true;
                }
            }

            spec = null;

            return false;
        }

        public void TryTakeEffectToSelf(GameplayEffect effect, int level = 0, object data = null)
        {
            TryTakeEffect(this, effect, level, data);
        }
        public void TryTakeEffectToOther(EffectSystemComponent target, GameplayEffect effect, int level = 0, object data = null)
        {
            target.TryTakeEffect(this, effect, level, data);
        }

        protected void TryTakeEffect(EffectSystemComponent instigator, GameplayEffect effect, int level = 0, object data = null)
        {
            if (TryGetEffectSpec(effect, out IEffectSpec containSpec))
            {
                if (containSpec.TryOverlapEffect(level))
                {
                    Log("Override Effect - " + effect.name);

                    return;
                }
            }

            var spec = effect.CreateSpec(this, instigator, level, data);

            if (spec is not null && spec.TryTakeEffect())
            {
                if (spec.IsActivate)
                    _Effects.Add(spec);
            }
        }
        


#region Cancel Effect
        public void RemoveEffect(GameplayEffect effect)
        {
            if (effect is null)
                return;

            if (TryGetEffectSpec(effect, out IEffectSpec spec))
            {
                RemoveEffect(spec);
            }
        }
        
        public void RemoveAllEffect()
        {
            foreach (var spec in Effects)
            {
                RemoveEffect(spec);
            }

            _Effects.Clear();
        }

        public void RemoveEffectFromSource(object source)
        {
            foreach (var spec in Effects)
            {
                if (spec.CanRemoveEffectFromSource(source))
                {
                    RemoveEffect(spec);
                }
            }
        }

        public void RemoveEffect(IEffectSpec effectSpec)
        {
            if (effectSpec is null)
                return;

            effectSpec.ForceRemoveEffect();
        }


        private void RemoveEffectSpec(IEffectSpec effectSpec)
        {
            _Effects.Remove(effectSpec);

            Callback_OnRemovedEffect(effectSpec);
        }

        #endregion

        #region Callback
        protected virtual void Callback_OnGrantedEffect(IEffectSpec addedSpec)
        {
            Log("On Granted Effect - " + addedSpec);

            OnGrantedEffect?.Invoke(this, addedSpec);
        }
        protected virtual void Callback_OnRemovedEffect(IEffectSpec removedSpec)
        {
            Log("On Removed Effect - " + removedSpec);

            OnRemovedEffect?.Invoke(this, removedSpec);
        }
#endregion

    }
}
