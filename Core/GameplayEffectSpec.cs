using UnityEngine;
using StudioScor.Utilities;


namespace StudioScor.GameplayEffectSystem
{
    public delegate void EffectSpecStateHandler(IGameplayEffectSpec effectSpec);
    public delegate void EffectSpecLevelStateHandler(IGameplayEffectSpec effectSpec, int currentLevel, int prevLevel);

    public abstract partial class GameplayEffectSpec : BaseClass, IGameplayEffectSpec
    {
        protected GameplayEffect gameplayEffect;
        protected IGameplayEffectSystem gameplayEffectSystem;

        private bool isActivate = false;

        protected int level;
        protected object data;
        protected float remainTime;

        public GameplayEffect GameplayEffect => gameplayEffect;
        public IGameplayEffectSystem GameplayEffectSystem => gameplayEffectSystem;

        public bool IsActivate => isActivate;
        public int Level => level;
        public float RemainTime => remainTime;

        public object Data => data;

#if UNITY_EDITOR
        public override bool UseDebug => GameplayEffect.UseDebug;
        public override Object Context => gameplayEffect;
#endif

        public event EffectSpecStateHandler OnActivateEffect;
        public event EffectSpecStateHandler OnCanceledEffect;
        public event EffectSpecStateHandler OnFinishedEffect;
        public event EffectSpecStateHandler OnEndedEffect;
        
        public event EffectSpecStateHandler OnOverlappedEffect;

        public event EffectSpecLevelStateHandler OnChangedEffectLevel;

        public GameplayEffectSpec() { }
        public GameplayEffectSpec(GameplayEffect gameplayEffect, IGameplayEffectSystem gameplayEffectSystem, int level = 0, object data = default)
        {
            SetupSpec(gameplayEffect, gameplayEffectSystem, level, data);
        }

        public virtual void SetupSpec(GameplayEffect gameplayEffect, IGameplayEffectSystem gameplayEffectSystem, int level = 0, object data = default)
        {
            this.gameplayEffect = gameplayEffect;
            this.gameplayEffectSystem = gameplayEffectSystem;
            this.level = level;
            this.data = data;
        }

        public virtual void Copy(IGameplayEffectSpec effectSpec)
        {

        }

        public void ForceOverlapEffect(IGameplayEffectSpec spec) 
        {
            OnOverlapEffect(spec);
        }

        public virtual bool CanOverlapEffect(IGameplayEffectSpec spec)
        {
            return false;
        }

        public virtual bool CancelEffectFromSource(object source)
        {
            return false;
        }

        public bool TryOverlapEffect(IGameplayEffectSpec spec)
        {
            if(CanOverlapEffect(spec))
            {
                ForceOverlapEffect(spec);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void ChangeLevel(int level) 
        {
            if (this.level == level)
                return;

            var prevLevel = level;

            this.level = level;

            OnChangeLevel(prevLevel);

            Callback_OnChangedEffectLevel(prevLevel);
        }


        public virtual bool CanTakeEffect()
        {
            return !IsActivate;
        }

        public void ForceTakeEffect()
        {
            Log(" Activate Effect ");

            isActivate = true;

            OnEnterEffect();

            if (gameplayEffect.Type.Equals(EGameplayEffectType.Instante))
            {
                EndEffect();

                return;
            }

            remainTime = GameplayEffect.Duration;
        }

        public bool TryTakeEffect()
        {
            if (!CanTakeEffect())
            {
                return false;
            }

            ForceTakeEffect();

            return true;
        }

        
        public void UpdateEffect(float deltaTime)
        {
            if (!IsActivate && GameplayEffect.Type.Equals(EGameplayEffectType.Instante))
                return;

            OnUpdateEffect(deltaTime);

            if (!GameplayEffect.Type.Equals(EGameplayEffectType.Duration))
                return;

            remainTime -= deltaTime;

            if(remainTime <= 0f)
            {
                EndEffect();
            }
        }
        public void EndEffect()
        {
            if (!isActivate)
                return;

            Log(" End Effect ");
            
            isActivate = false;

            OnFInishEffect();

            OnExitEffect();
        }
        public void ForceCancelEffect()
        {
            if (!isActivate)
                return;

            Log(" Force Cancel Effect ");

            isActivate = false;

            OnCancelEffect();

            OnExitEffect();
        }

        protected abstract void OnEnterEffect();
        protected virtual void OnUpdateEffect(float deltaTime) { }
        protected virtual void OnExitEffect() { }
        protected virtual void OnFInishEffect() { }
        protected virtual void OnCancelEffect() { }
        protected virtual void OnChangeLevel(int prevLevel) { }
        protected virtual void OnOverlapEffect(IGameplayEffectSpec spec) { }

        
        #region Callback
        protected void Callback_OnActivateEffect()
        {
            Log("On Activate Effect");

            OnActivateEffect?.Invoke(this);
        }
        protected void Callback_OnCanceledEffect()
        {
            Log("On Canceled Effect");

            OnCanceledEffect?.Invoke(this);
        }
        protected void Callback_OnFinishedEffect()
        {
            Log("On Finished Effect");

            OnFinishedEffect?.Invoke(this);
        }
        protected void Callback_OnEndedEffect()
        {
            Log("On Ended Effect");

            OnEndedEffect?.Invoke(this);
        }
        protected void Callback_OnOverlappedEffect()
        {
            Log("On Overlapped Effect");

            OnOverlappedEffect?.Invoke(this);
        }
        protected void Callback_OnChangedEffectLevel(int prevLevel)
        {
            Log("On Changed Effect Level - Current Level : " + Level + " Prev Level : " + prevLevel);

            OnChangedEffectLevel?.Invoke(this, level, prevLevel);
        }

       


        #endregion
    }
}
