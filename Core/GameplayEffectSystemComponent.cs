using System.Collections.Generic;
using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.GameplayEffectSystem
{
    public interface IGameplayEffectSystem
    {
        public Transform transform { get; }
        public bool TryGetGameplayEffectSpec(GameplayEffect gameplayEffect, out IGameplayEffectSpec spec);

        public bool TryTakeEffectToOther(GameplayEffect effect, int level = 0, object data = null);
        public bool TryTakeEffectToSelf(GameplayEffect effect, int level = 0, object data = null);

        public bool TryTakeEffectToOther(GameplayEffect effect, int level, object data, out IGameplayEffectSpec spec);
        public bool TryTakeEffectToSelf(GameplayEffect effect, int level, object data, out IGameplayEffectSpec spec);
        public void RemoveEffectFromSource(object source);
    }

    [AddComponentMenu("StudioScor/GameplayEffectSystem/GameplayEffect System Component", order: 0)]
    public class GameplayEffectSystemComponent : BaseMonoBehaviour, IGameplayEffectSystem
    {
        #region Event
        public delegate void ChangeGameplayEffectHandler(GameplayEffectSystemComponent effectSystem, IGameplayEffectSpec effectSpec);
        #endregion

        [Header(" [ Effect System ] ")]
        private List<IGameplayEffectSpec> _GameplayEffects;
        public IReadOnlyList<IGameplayEffectSpec> GameplayEffects => _GameplayEffects;

        public event ChangeGameplayEffectHandler OnGrantedEffect;
        public event ChangeGameplayEffectHandler OnRemovedEffect;

        private void Awake()
        {
            Setup();
        }

        protected void Setup()
        {
            Log("Setup");

            _GameplayEffects = new();
        }
        protected virtual void OnSetup() { }

        public void ResetEffectSystem()
        {
            Log("Reset");

            RemoveAllEffect();

            _GameplayEffects.Clear();

            OnReset();
        }
        protected virtual void OnReset() { }



        private void Update()
        {
            float deltaTime = Time.deltaTime;

            for(int i = GameplayEffects.Count - 1; i >= 0; i --)
            {
                GameplayEffects[i].UpdateEffect(deltaTime);

                if (!GameplayEffects[i].IsActivate)
                {
                    RemoveEffectSpec(_GameplayEffects[i]);
                }
            }
        }

        public bool ContainEffect(GameplayEffect containEffect)
        {
            foreach (var effect in GameplayEffects)
            {
                if (effect.GameplayEffect == containEffect)
                {
                    return true;
                }
            }

            return false;
        }
        public bool TryGetGameplayEffectSpec(GameplayEffect containEffect, out IGameplayEffectSpec spec)
        {
            foreach (var effect in GameplayEffects)
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

        public bool TryTakeEffectToOther(GameplayEffect effect, int level = 0, object data = null)
        {
            return TryTakeEffect(effect, level, data, out IGameplayEffectSpec _);
        }

        public bool TryTakeEffectToSelf(GameplayEffect effect, int level = 0, object data = null)
        {
            return TryTakeEffect(effect, level, data, out IGameplayEffectSpec _);
        }

        public bool TryTakeEffectToSelf(GameplayEffect effect, int level, object data, out IGameplayEffectSpec spec)
        {
            return TryTakeEffect(effect, level, data, out spec);
        }
        public bool TryTakeEffectToOther(GameplayEffect effect, int level, object data, out IGameplayEffectSpec spec)
        {
            return TryTakeEffect(effect, level, data, out spec);
        }

        public bool TryTakeEffect(GameplayEffect effect, int level, object data, out IGameplayEffectSpec spec)
        {
            spec = effect.CreateSpec(this, level, data);

            if (TryGetGameplayEffectSpec(effect, out IGameplayEffectSpec containSpec))
            {
                if (containSpec.TryOverlapEffect(spec))
                {
                    Log("Override Effect - " + effect.name);

                    return true;
                }
            }

            if (spec.TryTakeEffect())
            {
                if (spec.IsActivate)
                    _GameplayEffects.Add(spec);

                return true;
            }

            return false;
        }
        


#region Cancel Effect
        public void RemoveEffect(GameplayEffect effect)
        {
            if (effect is null)
                return;

            if (TryGetGameplayEffectSpec(effect, out IGameplayEffectSpec spec))
            {
                RemoveEffect(spec);
            }
        }
        
        public void RemoveAllEffect()
        {
            foreach (var spec in GameplayEffects)
            {
                RemoveEffect(spec);
            }
        }

        public void RemoveEffectFromSource(object source)
        {
            foreach (var spec in GameplayEffects)
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
            _GameplayEffects.Remove(effectSpec);

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
