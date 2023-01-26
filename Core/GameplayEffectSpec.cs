using UnityEngine;
using System.Linq;
using System.Diagnostics;
using UnityEngine.Events;


namespace StudioScor.EffectSystem
{

    [System.Serializable]
    public abstract partial class GameplayEffectSpec
    {
#region Events
        public delegate void GameplayEffectSpecState(GameplayEffectSpec gameplayEffectSpec);
#endregion
        private readonly GameplayEffect _GameplayEffect;
        private readonly GameplayEffectSystem _GameplayEffectSystem;

        private bool _Activate = false;
        private bool _IsApply = false;
        protected int _Level;
        protected object _Data; 

        public GameplayEffect GameplayEffect => _GameplayEffect;
        public GameplayEffectSystem GameplayEffectSystem => _GameplayEffectSystem;
        public bool IsActivate => _Activate;
        public bool IsApply => _IsApply;
        public int Level => _Level;
        public object Data => _Data;

        public GameplayEffectSpec(GameplayEffect gameplayEffect, GameplayEffectSystem gameplayEffectSystem, int level = 0, object data = default)
        {
            _GameplayEffect = gameplayEffect;
            _GameplayEffectSystem = gameplayEffectSystem;
            _Level = level;
            _Data = data;
        }

#region EDITOR ONLY
        [Conditional("UNITY_EDITOR")]
        protected void Log(object massage)
        {
#if UNITY_EDITOR
            if (GameplayEffect.UseDebug)
                UnityEngine.Debug.Log(GameplayEffectSystem.gameObject.name + " [ " + GetType().Name + " ]" + massage, GameplayEffect);
#endif
        }
#endregion


#region OverrideEffect
        public bool TryOverrideEffect(GameplayEffectSpec gameplayEffectSpec)
        {
            if (!CanOverrideEffect(gameplayEffectSpec))
                return false;

            Log("Override Effect");

            OnOverrideEffect(gameplayEffectSpec);

            return true;
        }
        public virtual bool CanOverrideEffect(GameplayEffectSpec gameplayEffectSpec)
        {
            return false;
        }
        public virtual void OnOverrideEffect(GameplayEffectSpec gameplayEffectSpec) { }

#endregion


        public void ActivateEffect()
        {
            if (IsActivate) 
                return;

            _Activate = true;

            ActivateEffectWithGameplayTag();

            EnterEffect();

            Log("Activate Effect");

            if (CanApplyGameplayEffect())
                OnApplyEffect();
        }

        public void OnUpdateEffect(float deltaTime)
        {
            if (!IsActivate)
            {
                GameplayEffectSystem.RemoveGameplayEffectSpec(this);

                return;
            }

            UpdateEffect(deltaTime);

            if (IsApply)
                UpdateApplyEffect(deltaTime);
        }

        

        public void EndGameplayEffect()
        {
            if (!IsActivate)
            {
                return;
            }

            _Activate = false;

            OnIgnoreEffect();

            EndGameplayEffectWithGameplayTag();

            ExitEffect();

            Log("End Effect");
        }

        protected bool TryApplyEffect()
        {
            if (CanApplyGameplayEffect())
            {
                OnApplyEffect();

                return true;
            }
            else
            {
                OnIgnoreEffect();

                return false;
            }
        }

        protected void OnApplyEffect()
        {
            if (IsApply)
                return;

            _IsApply = true;

            ApplyEffectWithGameplayTag();

            ApplyEffect();

            Log("Apply Effect");
        }
        protected void OnIgnoreEffect()
        {
            if (!IsApply)
                return;
            
            _IsApply = false;

            IgnoreEffectWithGameplayTag();

            IgnoreEffect();

            Log("Ignore Effect");
        }

        protected virtual bool CanApplyGameplayEffect()
        {
            if (!CanApplyConditionTags())
            {
                return false;
            }

            return true;
        }

       

        protected abstract void EnterEffect();
        protected abstract void UpdateEffect(float deltaTime);
        protected virtual void UpdateApplyEffect(float deltaTime) { }
        protected virtual void ExitEffect() { }
        protected virtual void ApplyEffect() { }
        protected virtual void IgnoreEffect() { }
        
    }
}
