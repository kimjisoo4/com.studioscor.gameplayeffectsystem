using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Diagnostics;


namespace StudioScor.EffectSystem
{

    public partial class GameplayEffectSystem : MonoBehaviour
    {
#region Event
        public delegate void ChangeGameplayEffectHandler(GameplayEffectSystem gameplayEffectSystem, GameplayEffectSpec gameplayEffectSpec);
#endregion

        private bool _WasSetup = false;
        private List<GameplayEffectSpec> _GameplayEffects;

        [Header(" [ Use Debug ] ")]
        [SerializeField] private bool _UseDebug;

        public IReadOnlyList<GameplayEffectSpec> GameplayEffects
        {
            get
            {
                if (!_WasSetup)
                    Setup();

                return _GameplayEffects;
            }
        }


        public bool UseDebug => _UseDebug;

        public event ChangeGameplayEffectHandler OnAddedGameplayEffect;
        public event ChangeGameplayEffectHandler OnRemovedGameplayEffect;

#region EDITOR ONLY

#if UNITY_EDITOR
        private void Reset()
        {
            SetupGameplayTag();
        }
#endif

        [Conditional("UNITY_EDITOR")]
        protected void Log(object content, bool isError = false)
        {
#if UNITY_EDITOR
            if (isError)
            {
                UnityEngine.Debug.LogError("Effect Sytstem [ " + transform.name + " ] : " + content, this);

                return;
            }

            if (_UseDebug)
                UnityEngine.Debug.Log("Effect Sytstem [ " + transform.name + " ] : " + content, this);
#endif
        }
#endregion


        private void Awake()
        {
            if (!_WasSetup)
                Setup();
        }
        protected virtual void Setup()
        {
            if (_WasSetup)
                return;

            _WasSetup = true;

            Log("Setup");

            SetupGameplayTag();

            _GameplayEffects = new();
        }

        public void ResetEffectSystem()
        {
            CancelAllGameplayEffect();

            _GameplayEffects.Clear();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            for (int i = GameplayEffects.Count - 1; i >= 0; i--)
            {
                GameplayEffects[i].OnUpdateEffect(deltaTime);
            }
        }

        public bool ContainGameplayEffect(GameplayEffect containEffect)
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
        public bool TryGetGameplayEffectSpec(GameplayEffect containEffect, out GameplayEffectSpec spec)
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


        public void ForceTakeGameplayEffect(GameplayEffectSpec effectSpec)
        {
            if (TryGetGameplayEffectSpec(effectSpec.GameplayEffect, out GameplayEffectSpec spec))
            {
                if (spec.TryOverrideEffect(effectSpec))
                {
                    Log("Override Effect");

                    return;
                }
            }

            effectSpec.ActivateEffect();

            _GameplayEffects.Add(effectSpec);

            OnAddEffect(effectSpec);
        }

        public void RemoveGameplayEffectSpec(GameplayEffectSpec spec)
        {
            if (spec.IsActivate)
            {
                spec.EndGameplayEffect();
            }

            _GameplayEffects.Remove(spec);
        }

#region Cancel Effect
        public void CancelGameplayEffect(GameplayEffect effect)
        {
            if (effect is null)
                return;

            if (TryGetGameplayEffectSpec(effect, out GameplayEffectSpec spec))
            {
                CancelGamepalyEffect(spec);
            }
        }
        public void CancelGameplayEffect(GameplayEffectSpec effectSpec)
        {
            if (effectSpec is null)
                return;

            if (GameplayEffects.Contains(effectSpec))
            {
                CancelGamepalyEffect(effectSpec);
            }
        }
        public void CancelAllGameplayEffect()
        {
            for(int i = GameplayEffects.Count - 1; i >= 0; i--)
            {
                CancelGameplayEffect(GameplayEffects[i]);
            }
        }
        private void CancelGamepalyEffect(GameplayEffectSpec effectSpec)
        {
            effectSpec.EndGameplayEffect();
        }
        
        

#endregion

#region Callback
        protected virtual void OnAddEffect(GameplayEffectSpec addedSpec)
        {
            Log("Add Effect - " + addedSpec);

            OnAddedGameplayEffect?.Invoke(this, addedSpec);
        }
        protected virtual void OnRemoveEffect(GameplayEffectSpec removedSpec)
        {
            Log("Add Effect - " + removedSpec);

            OnRemovedGameplayEffect?.Invoke(this, removedSpec);
        }
#endregion

    }
}
