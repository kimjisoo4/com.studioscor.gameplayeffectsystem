using UnityEngine;

using StudioScor.Utilities;


namespace StudioScor.GameplayEffectSystem
{

    public delegate void EffectSpecStateHandler(IGameplayEffectSpec effectSpec);
    public delegate void EffectSpecLevelStateHandler(IGameplayEffectSpec effectSpec, int currentLevel, int prevLevel);

    public abstract partial class GameplayEffectSpec<T> : BaseClass, IGameplayEffectSpec where T : GameplayEffect
    {
        protected readonly T _GameplayEffect;

        private IGameplayEffectSystem _Owner;
        private IGameplayEffectSystem _Instigator;

        private bool _IsActivate = false;

        protected int _Level;
        protected float _Strength;
        protected float _RemainTime;

        public GameplayEffect GameplayEffect => _GameplayEffect;
        public IGameplayEffectSystem Owner => _Owner;
        public IGameplayEffectSystem Instigator => _Instigator;
        public bool IsActivate => _IsActivate;
        public int Level => _Level;
        public float Strength => _Strength;
        public float RemainTime => _RemainTime;

#if UNITY_EDITOR
        public override bool UseDebug => GameplayEffect.UseDebug;
        public override Object Context => _GameplayEffect;
#endif

        public event EffectSpecStateHandler OnActivateEffect;
        public event EffectSpecStateHandler OnCanceledEffect;
        public event EffectSpecStateHandler OnFinishedEffect;
        public event EffectSpecStateHandler OnEndedEffect;
        
        public event EffectSpecStateHandler OnOverlappedEffect;

        public event EffectSpecLevelStateHandler OnChangedEffectLevel;

        public GameplayEffectSpec(T gameplayEffect)
        {
            _GameplayEffect = gameplayEffect;
        }

        public virtual void SetupSpec(IGameplayEffectSystem owner, IGameplayEffectSystem instigator, int level = 0, float strength = 0f, object data = default)
        {
            _Owner = owner;
            _Instigator = instigator;
            _Level = level;
            _Strength = strength;
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

        public virtual bool CanRemoveEffectFromSource(object source)
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
            if (_Level == level)
                return;

            var prevLevel = level;

            _Level = level;

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

            _IsActivate = true;

            OnEnterEffect();

            if (_GameplayEffect.Type.Equals(EGameplayEffectType.Instante))
            {
                EndEffect();

                return;
            }

            _RemainTime = GameplayEffect.Duration;
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

            _RemainTime -= deltaTime;

            if(_RemainTime <= 0f)
            {
                EndEffect();
            }
        }
        public void EndEffect()
        {
            if (!_IsActivate)
                return;

            Log(" End Effect ");
            
            _IsActivate = false;

            OnFInishEffect();

            OnExitEffect();
        }
        public void ForceRemoveEffect()
        {
            if (!_IsActivate)
                return;

            Log(" Force Remmove Effect ");

            _IsActivate = false;

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

            OnChangedEffectLevel?.Invoke(this, _Level, prevLevel);
        }

       


        #endregion
    }
}
