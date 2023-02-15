using UnityEngine;

using StudioScor.Utilities;


namespace StudioScor.EffectSystem
{

    public delegate void EffectSpecStateHandler(IEffectSpec effectSpec);
    public delegate void EffectSpecLevelStateHandler(IEffectSpec effectSpec, int currentLevel, int prevLevel);

    public abstract partial class EffectSpec<T> : BaseClass, IEffectSpec where T : GameplayEffect
    {
        protected readonly T _Effect;

        private EffectSystemComponent _Owner;
        private EffectSystemComponent _Instigator;

        private bool _IsActivate = false;
        protected int _Level;

        private float _RemainTime;
        public GameplayEffect Effect => _Effect;
        public EffectSystemComponent Owner => _Owner;
        public EffectSystemComponent Instigator => _Instigator;
        public bool IsActivate => _IsActivate;
        public int Level => _Level;
        public float RemainTime => _RemainTime;

#if UNITY_EDITOR
        public override bool UseDebug => Effect.UseDebug;
        public override Object Context => _Effect;
#endif

        public event EffectSpecStateHandler OnActivateEffect;
        public event EffectSpecStateHandler OnCanceledEffect;
        public event EffectSpecStateHandler OnFinishedEffect;
        public event EffectSpecStateHandler OnEndedEffect;
        
        public event EffectSpecStateHandler OnOverlappedEffect;

        public event EffectSpecLevelStateHandler OnChangedEffectLevel;

        public EffectSpec(T effect)
        {
            _Effect = effect;
        }

        public virtual void SetupSpec(EffectSystemComponent owner, EffectSystemComponent instigator, int level = 0, object data = default)
        {
            _Owner = owner;
            _Instigator = instigator;
            _Level = level;
        }

        public virtual void ForceOverlapEffect(int level) 
        {
            OnOverlapEffect(level);
        }

        public bool CanOverlapEffect(int level)
        {
            return level > Level;
        }

        public virtual bool CanRemoveEffectFromSource(object source)
        {
            return false;
        }

        public bool TryOverlapEffect(int level)
        {
            if(CanOverlapEffect(level))
            {
                ForceOverlapEffect(level);

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
            return true;
        }

        public void ForceTakeEffect()
        {
            Log(" Activate Effect ");

            _IsActivate = true;

            OnEnterEffect();

            if (_Effect.Type.Equals(EEffectType.Instante))
            {
                EndEffect();

                return;
            }

            _RemainTime = Effect.Duration;
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
            if (!IsActivate && Effect.Type.Equals(EEffectType.Instante))
                return;

            OnUpdateEffect(deltaTime);

            if (!Effect.Type.Equals(EEffectType.Duration))
                return;

            _RemainTime -= deltaTime;

            if(_RemainTime <= 0f)
            {
                EndEffect();
            }
        }
        public void EndEffect()
        {
            Log(" End Effect ");

            _IsActivate = false;

            OnFInishEffect();

            OnExitEffect();
        }
        public void ForceRemoveEffect()
        {
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
        protected virtual void OnOverlapEffect(int level) { }

        
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
