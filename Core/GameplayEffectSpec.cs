using UnityEngine;
using StudioScor.Utilities;


namespace StudioScor.GameplayEffectSystem
{
    public abstract class GameplayEffectSpec : BaseClass, IGameplayEffectSpec
    {
        protected GameplayEffect _gameplayEffect;
        protected IGameplayEffectSystem _gameplayEffectSystem;
        protected GameObject _instigator;

        private bool _isActivate = false;

        protected int _level;
        protected object _data;
        protected float _remainTime;

        public GameObject gameObject => _gameplayEffectSystem is null ? null : _gameplayEffectSystem.gameObject;
        public GameplayEffect GameplayEffect => _gameplayEffect;
        public IGameplayEffectSystem GameplayEffectSystem => _gameplayEffectSystem;
        public GameObject Instigator => _instigator;
        public bool IsActivate => _isActivate;
        public int Level => _level;
        public float RemainTime => _remainTime;
        public object Data => _data;

#if UNITY_EDITOR
        public override bool UseDebug => GameplayEffect.UseDebug;
        public override Object Context => _gameplayEffect;
#endif

        public event IGameplayEffectSpec.EffectSpecStateHandler OnActivateEffect;
        public event IGameplayEffectSpec.EffectSpecStateHandler OnCanceledEffect;
        public event IGameplayEffectSpec.EffectSpecStateHandler OnFinishedEffect;
        public event IGameplayEffectSpec.EffectSpecStateHandler OnEndedEffect;
                     
        public event IGameplayEffectSpec.EffectSpecStateHandler OnOverlappedEffect;

        public event IGameplayEffectSpec.EffectSpecLevelStateHandler OnChangedEffectLevel;

        public GameplayEffectSpec() { }
        public GameplayEffectSpec(GameplayEffect gameplayEffect, IGameplayEffectSystem gameplayEffectSystem, GameObject instigator = null, int level = 0, object data = default)
        {
            SetupSpec(gameplayEffect, gameplayEffectSystem, instigator, level, data);
        }

        public virtual void SetupSpec(GameplayEffect gameplayEffect, IGameplayEffectSystem gameplayEffectSystem, GameObject instigator, int level = 0, object data = default)
        {
            _gameplayEffect = gameplayEffect;
            _gameplayEffectSystem = gameplayEffectSystem;
            _level = level;
            _data = data;
            _instigator = instigator;
        }

        public virtual void Copy(IGameplayEffectSpec effectSpec)
        {

        }

        public void ForceOverlapEffect(IGameplayEffectSpec spec) 
        {
            OnOverlapEffect(spec);

            Invoke_OnOverlappedEffect();
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

            Invoke_OnChangedEffectLevel(prevLevel);
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

            Invoke_OnActivateEffect();

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
                _remainTime -= _gameplayEffect.UnscaledTime ? Time.unscaledDeltaTime : deltaTime;

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

            Invoke_OnFinishedEffect();

            OnExitEffect();

            Invoke_OnEndedEffect();
        }
        public void ForceCancelEffect()
        {
            if (!_isActivate)
                return;

            Log(" Force Cancel Effect ");

            _isActivate = false;

            OnCancelEffect();

            Invoke_OnCanceledEffect();

            OnExitEffect();

            Invoke_OnEndedEffect();
        }

        protected abstract void OnEnterEffect();
        protected virtual void OnUpdateEffect(float deltaTime) { }
        protected virtual void OnExitEffect() { }
        protected virtual void OnFInishEffect() { }
        protected virtual void OnCancelEffect() { }
        protected virtual void OnChangeLevel(int prevLevel) { }
        protected virtual void OnOverlapEffect(IGameplayEffectSpec spec) { }

        
        #region Callback
        protected void Invoke_OnActivateEffect()
        {
            Log($"{nameof(OnActivateEffect)}");

            OnActivateEffect?.Invoke(this);
        }
        protected void Invoke_OnCanceledEffect()
        {
            Log($"{nameof(OnCanceledEffect)}");

            OnCanceledEffect?.Invoke(this);
        }
        protected void Invoke_OnFinishedEffect()
        {
            Log($"{nameof(OnFinishedEffect)}");

            OnFinishedEffect?.Invoke(this);
        }
        protected void Invoke_OnEndedEffect()
        {
            Log($"{nameof(OnEndedEffect)}");

            OnEndedEffect?.Invoke(this);
        }
        protected void Invoke_OnOverlappedEffect()
        {
            Log($"{nameof(OnOverlappedEffect)}");

            OnOverlappedEffect?.Invoke(this);
        }
        protected void Invoke_OnChangedEffectLevel(int prevLevel)
        {
            Log($"{nameof(OnChangedEffectLevel)}- Current Level : " + Level + " Prev Level : " + prevLevel);

            OnChangedEffectLevel?.Invoke(this, _level, prevLevel);
        }

       


        #endregion
    }
}
