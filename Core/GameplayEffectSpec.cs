using UnityEngine;
using StudioScor.Utilities;


namespace StudioScor.GameplayEffectSystem
{
    public delegate void EffectSpecStateHandler(IGameplayEffectSpec effectSpec);
    public delegate void EffectSpecLevelStateHandler(IGameplayEffectSpec effectSpec, int currentLevel, int prevLevel);

    public abstract class GameplayEffectSpec : BaseClass, IGameplayEffectSpec
    {
        protected GameplayEffect _gameplayEffect;
        protected IGameplayEffectSystem _gameplayEffectSystem;

        private bool _isActivate = false;

        protected int _level;
        protected object _data;
        protected float _remainTime;

        public GameplayEffect GameplayEffect => _gameplayEffect;
        public IGameplayEffectSystem GameplayEffectSystem => _gameplayEffectSystem;
        public bool IsActivate => _isActivate;
        public int Level => _level;
        public float RemainTime => _remainTime;
        public object Data => _data;

#if UNITY_EDITOR
        public override bool UseDebug => GameplayEffect.UseDebug;
        public override Object Context => _gameplayEffect;
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
            this._gameplayEffect = gameplayEffect;
            this._gameplayEffectSystem = gameplayEffectSystem;
            this._level = level;
            this._data = data;
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
            if (this._level == level)
                return;

            var prevLevel = level;

            this._level = level;

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

            _isActivate = true;

            OnEnterEffect();

            if (_gameplayEffect.Type.Equals(EGameplayEffectType.Instante))
            {
                EndEffect();

                return;
            }

            _remainTime = GameplayEffect.Duration;
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
                _remainTime -= _gameplayEffect.UnscaledTime ? Time.deltaTime : deltaTime;

                OnUpdateEffect(deltaTime);

                if (_remainTime <= 0f)
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
            if (!_isActivate)
                return;

            Log(" End Effect ");
            
            _isActivate = false;

            OnFInishEffect();

            OnExitEffect();
        }
        public void ForceCancelEffect()
        {
            if (!_isActivate)
                return;

            Log(" Force Cancel Effect ");

            _isActivate = false;

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

            OnChangedEffectLevel?.Invoke(this, _level, prevLevel);
        }

       


        #endregion
    }
}
