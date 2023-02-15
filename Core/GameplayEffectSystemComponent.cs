using System.Collections.Generic;
using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.GameplayEffectSystem
{
    [AddComponentMenu("StudioScor/GameplayEffectSystem/GameplayEffect System Component", order: 0)]
    public class GameplayEffectSystemComponent : BaseMonoBehaviour
    {
        #region Event
        public delegate void ChangeGameplayEffectHandler(GameplayEffectSystemComponent effectSystem, IGameplayEffectSpec effectSpec);
        #endregion

        [Header(" [ Effect System ] ")]
        private List<IGameplayEffectSpec> _Effects;
        public IReadOnlyList<IGameplayEffectSpec> Effects => _Effects;

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
                if (effect.GameplayEffect == containEffect)
                {
                    return true;
                }
            }

            return false;
        }
        public bool TryGetEffectSpec(GameplayEffect containEffect, out IGameplayEffectSpec spec)
        {
            foreach (var effect in Effects)
            {
                if (effect.GameplayEffect == containEffect)
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
        public void TryTakeEffectToOther(GameplayEffectSystemComponent target, GameplayEffect effect, int level = 0, object data = null)
        {
            target.TryTakeEffect(this, effect, level, data);
        }

        protected void TryTakeEffect(GameplayEffectSystemComponent instigator, GameplayEffect effect, int level = 0, object data = null)
        {
            if (TryGetEffectSpec(effect, out IGameplayEffectSpec containSpec))
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

            if (TryGetEffectSpec(effect, out IGameplayEffectSpec spec))
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

        public void RemoveEffect(IGameplayEffectSpec effectSpec)
        {
            if (effectSpec is null)
                return;

            effectSpec.ForceRemoveEffect();
        }


        private void RemoveEffectSpec(IGameplayEffectSpec effectSpec)
        {
            _Effects.Remove(effectSpec);

            Callback_OnRemovedEffect(effectSpec);
        }

        #endregion

        #region Callback
        protected virtual void Callback_OnGrantedEffect(IGameplayEffectSpec addedSpec)
        {
            Log("On Granted Effect - " + addedSpec);

            OnGrantedEffect?.Invoke(this, addedSpec);
        }
        protected virtual void Callback_OnRemovedEffect(IGameplayEffectSpec removedSpec)
        {
            Log("On Removed Effect - " + removedSpec);

            OnRemovedEffect?.Invoke(this, removedSpec);
        }
#endregion

    }
}
