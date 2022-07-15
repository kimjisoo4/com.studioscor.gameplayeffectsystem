using UnityEngine;
using System.Linq;

namespace KimScor.GameplayTagSystem.Effect
{
    [System.Serializable]
    public abstract class GameplayEffectSpec
    {
        private GameplayEffect _GameplayEffect;
        private GameplayEffectSystem _Owner;
        protected float _ElapsedTime;
        private object _Data;
        private bool _Activate = false;
        private bool _Apply = false;

        public GameplayEffect GameplayEffect => _GameplayEffect;
        public GameplayEffectSystem Owner => _Owner;
        public GameplayTagSystem GameplayTagSystem => Owner.GameplayTagSystem;
        public float Duration => _GameplayEffect.Duration;
        public float ElapsedTime => _ElapsedTime;
        public object Data => _Data;
        public bool Activate => _Activate;
        public bool Apply => _Apply;

        public FGameplayEffectTags EffectTags => GameplayEffect.EffectTags;

        public bool IsInstant => GameplayEffect.DurationPolicy.Equals(EDurationPolicy.Instant);
        public bool IsDuration => GameplayEffect.DurationPolicy.Equals(EDurationPolicy.Duration);
        public bool IsInfinite => GameplayEffect.DurationPolicy.Equals(EDurationPolicy.Infinite);

        public EUpdateType UpdateType => GameplayEffect.UpdateType;

        public bool CanIgnoreUpdated => GameplayEffect.CanIgnoreUpdated;

        public GameplayEffectSpec(GameplayEffect effect, GameplayEffectSystem owner)
        {
            _GameplayEffect = effect;
            _Owner = owner;
        }
        public void SetData(object data)
        {
            _Data = data;
        }

        public bool TryGameplayEffect()
        {
            if (CanActivateGameplayEffect())
            {
                OnGameplayEffect();

                return true;
            }

            return false;
        }
        public void OnGameplayEffect()
        {
            if (Activate)
            {
                return;
            }

            if (GameplayEffect.DebugMode)
                Debug.Log("Enter Effect : " + GameplayEffect.name);


            _Activate = true;
            _ElapsedTime = 0f;

            Owner.RemoveGameplayEffectWithTags(GameplayEffect.EffectTags.RemoveGameplayEffectsWithTags);

            Owner.AddGameplayEffectList(this);

            Owner.GameplayTagSystem.AddOwnedTags(GameplayEffect.EffectTags.ActivateGrantedTags);

            if (GameplayEffect.EffectTags.ApplyEffectRequiredTags.Length > 0 || GameplayEffect.EffectTags.ApplyEffectIgnoreTags.Length > 0)
            {
                Owner.GameplayTagSystem.OnNewAddOwnedTag += GameplayTagSystem_OnUpdateOwnedTag;
                Owner.GameplayTagSystem.OnRemoveOwnedTag += GameplayTagSystem_OnUpdateOwnedTag;
            }

            if (IsDuration || !UpdateType.Equals(EUpdateType.None))
            {
                switch (UpdateType)
                {
                    case EUpdateType.None:
                        break;
                    case EUpdateType.Update:
                        Owner.OnUpdatedEffect += Owner_OnUpdatedEffect;
                        break;
                    case EUpdateType.Fixed:
                        Owner.OnFixedUpdatedEffect += Owner_OnUpdatedEffect;
                        break;
                    default:
                        break;
                }
            }

            EnterEffect();

            if (!Activate)
                return;
                
            _Apply = CanApplyGameplayEffect();

            if (Apply)
                OnApplyEffect();


            if (IsInstant)
            {
                EndGameplayEffect();
            }
        }

        private void Owner_OnUpdatedEffect(GameplayEffectSystem effectSystem, float deltaTime)
        {
            if (GameplayEffect.DebugMode)
                Debug.Log("Update Effect : " + GameplayEffect.name);

            if (_Apply)
            {
                OnUpdateEffect(deltaTime);

                if (GameplayEffect.DurationPolicy.Equals(EDurationPolicy.Duration))
                {
                    _ElapsedTime += deltaTime;

                    if (_ElapsedTime >= Duration)
                    {
                        EndGameplayEffect();

                        return;
                    }
                }
            }
            else
            {
                if(CanIgnoreUpdated)
                {
                    if (GameplayEffect.DurationPolicy.Equals(EDurationPolicy.Duration))
                    {
                        _ElapsedTime += deltaTime;

                        if (_ElapsedTime >= Duration)
                        {
                            EndGameplayEffect();

                            return;
                        }
                    }
                }
            }
        }

        private void GameplayTagSystem_OnUpdateOwnedTag(GameplayTagSystem gameplayTagSystem, GameplayTag changedTag)
        {
            if (EffectTags.ApplyEffectIgnoreTags.Contains(changedTag) 
                || EffectTags.ApplyEffectRequiredTags.Contains(changedTag))
            {
                TryApplyEffect();
            }
        }

        public void EndGameplayEffect()
        {
            if (!Activate)
            {
                return;
            }
            _Activate = false;

            if (Apply)
                OnIgnoreEffect();

            if (GameplayEffect.DebugMode)
                Debug.Log("Exit Effect : " + GameplayEffect.name);


            if (GameplayEffect.EffectTags.ApplyEffectRequiredTags.Length > 0 || GameplayEffect.EffectTags.ApplyEffectIgnoreTags.Length > 0)
            {
                Owner.GameplayTagSystem.OnNewAddOwnedTag -= GameplayTagSystem_OnUpdateOwnedTag;
                Owner.GameplayTagSystem.OnRemoveOwnedTag -= GameplayTagSystem_OnUpdateOwnedTag;
            }

            if (IsDuration || !UpdateType.Equals(EUpdateType.None))
            {
                switch (UpdateType)
                {
                    case EUpdateType.None:
                        Owner.OnUpdatedEffect -= Owner_OnUpdatedEffect;
                        break;
                    case EUpdateType.Update:
                        Owner.OnUpdatedEffect -= Owner_OnUpdatedEffect;
                        break;
                    case EUpdateType.Fixed:
                        Owner.OnFixedUpdatedEffect -= Owner_OnUpdatedEffect;
                        break;
                    default:
                        break;
                }
            }

            Owner.GameplayTagSystem.RemoveOwnedTags(GameplayEffect.EffectTags.ActivateGrantedTags);

            ExitEffect();

            Owner.RemoveGameplayEffectList(this);
        }
        /// <summary>
        /// 이펙트 발동 시작시 효과
        /// </summary>
        protected abstract void EnterEffect();

        /// <summary>
        /// 이펙트 발동 종료시 효과
        /// </summary>
        protected abstract void ExitEffect();
        
        /// <summary>
        /// 지속, 영속 효과의 매 틱 효과
        /// </summary>
        protected abstract void OnUpdateEffect(float deltaTime);

        /// <summary>
        /// 이펙트 적용 시작시 효과
        /// </summary>
        protected abstract void ApplyEffect();

        /// <summary>
        /// 이펙트 적용 무시시 효과
        /// </summary>
        protected abstract void IgnoreEffect();


        protected void TryApplyEffect()
        {
            bool canApplyEffect = CanApplyGameplayEffect();

            if (Apply != canApplyEffect)
            {
                _Apply = canApplyEffect;

                if (Apply)
                {
                    OnApplyEffect();
                }
                else
                {
                    OnIgnoreEffect();
                }
            }
        }
        protected void OnApplyEffect()
        {
            if (!Activate)
                return;

            if (GameplayEffect.DebugMode)
                Debug.Log("Apply Effect : " + GameplayEffect.name);

            GameplayTagSystem.AddOwnedTags(EffectTags.ApplyGrantedTags);

            ApplyEffect();
        }
        protected void OnIgnoreEffect()
        {
            if (GameplayEffect.DebugMode)
                Debug.Log("Ignore Effect : " + GameplayEffect.name);

            GameplayTagSystem.RemoveOwnedTags(EffectTags.ApplyGrantedTags);

            IgnoreEffect();
        }

        public virtual bool CanActivateGameplayEffect()
        {
            return Owner.GameplayTagSystem.ContainAllOwnedTags(GameplayEffect.EffectTags.ActivateEffectRequiredTags)
                && Owner.GameplayTagSystem.ContainNotAllOwnedTags(GameplayEffect.EffectTags.ActivateEffectIgnoreTags);
        }

        protected virtual bool CanApplyGameplayEffect()
        {
            return Owner.GameplayTagSystem.ContainAllOwnedTags(GameplayEffect.EffectTags.ApplyEffectRequiredTags)
                && Owner.GameplayTagSystem.ContainNotAllOwnedTags(GameplayEffect.EffectTags.ApplyEffectIgnoreTags);
        }
    }
}
