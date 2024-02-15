using UnityEngine;
using StudioScor.Utilities;


namespace StudioScor.GameplayEffectSystem
{
    public delegate void EffectSpecStateHandler(IGameplayEffectSpec effectSpec);
    public delegate void EffectSpecLevelStateHandler(IGameplayEffectSpec effectSpec, int currentLevel, int prevLevel);

    public abstract class GameplayEffectSpec : BaseClass, IGameplayEffectSpec
    {
        protected GameplayEffect _GameplayEffect;
        protected IGameplayEffectSystem _GameplayEffectSystem;
        protected GameObject _Instigator;

        private bool _IsActivate = false;

        protected int _Level;
        protected object _Data;
        protected float _RemainTime;

        public GameObject gameObject => _GameplayEffectSystem is null ? null : _GameplayEffectSystem.gameObject;
        public GameplayEffect GameplayEffect => _GameplayEffect;
        public IGameplayEffectSystem GameplayEffectSystem => _GameplayEffectSystem;
        public GameObject Instigator => _Instigator;
        public bool IsActivate => _IsActivate;
        public int Level => _Level;
        public float RemainTime => _RemainTime;
        public object Data => _Data;

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

        public GameplayEffectSpec() { }
        public GameplayEffectSpec(GameplayEffect gameplayEffect, IGameplayEffectSystem gameplayEffectSystem, GameObject instigator = null, int level = 0, object data = default)
        {
            SetupSpec(gameplayEffect, gameplayEffectSystem, instigator, level, data);
        }

        public virtual void SetupSpec(GameplayEffect gameplayEffect, IGameplayEffectSystem gameplayEffectSystem, GameObject instigator, int level = 0, object data = default)
        {
            _GameplayEffect = gameplayEffect;
            _GameplayEffectSystem = gameplayEffectSystem;
            _Level = level;
            _Data = data;
            _Instigator = instigator;
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
            if (this._Level == level)
                return;

            var prevLevel = level;

            this._Level = level;

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

            if (GameplayEffect.Type.Equals(EGameplayEffectType.Duration))
            {
                _RemainTime -= _GameplayEffect.UnscaledTime ? Time.deltaTime : deltaTime;

                OnUpdateEffect(deltaTime);

                if (_RemainTime <= 0f)
                {
                    EndEffect();
                }
            }
            else
            {
                OnUpdateEffect(deltaTime);
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
        public void ForceCancelEffect()
        {
            if (!_IsActivate)
                return;

            Log(" Force Cancel Effect ");

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
