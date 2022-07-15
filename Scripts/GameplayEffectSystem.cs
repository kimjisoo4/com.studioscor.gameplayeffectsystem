using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace KimScor.GameplayTagSystem.Effect
{
    public class GameplayEffectSystem : MonoBehaviour
    {
        #region Events
        public delegate void UpdateEffectHandler(GameplayEffectSystem effectSystem, float deltaTime);
        #endregion
        [SerializeField] private GameplayTagSystem _GameplayTagSystem;

        private Dictionary<GameplayEffect, GameplayEffectSpec> _GameplayEffects;
        public IReadOnlyDictionary<GameplayEffect, GameplayEffectSpec> GameplayEffects
        {
            get
            {
                if (_GameplayEffects == null)
                {
                    SetupEffectSystem();
                }

                return _GameplayEffects;
            }
        }

        public GameplayTagSystem GameplayTagSystem { get => _GameplayTagSystem; }

        [SerializeField] private GameplayEffect[] InitializationEffects;

        public event UpdateEffectHandler OnUpdatedEffect;
        public event UpdateEffectHandler OnFixedUpdatedEffect;

#if UNITY_EDITOR
        private void Reset()
        {
            TryGetComponent(out _GameplayTagSystem);
        }
#endif
        private void Awake()
        {
            if (GameplayEffects == null)
                SetupEffectSystem();
        }
        void SetupEffectSystem()
        {
            _GameplayEffects = new Dictionary<GameplayEffect, GameplayEffectSpec>();

            foreach (var effect in InitializationEffects)
            {
                ApplyGameplayEffectToSelf(effect);
            }
        }

        public bool ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect)
        {
            var spec = gameplayEffect.CreateSpec(this);

            if (!spec.TryGameplayEffect())
            {
                return false;
            }

            return true;
        }
        public bool ApplyGameplayEffectToOther(GameplayEffect gameplayEffect, object data)
        {
            var spec = gameplayEffect.CreateSpec(this);

            spec.SetData(data);

            if (!spec.TryGameplayEffect())
            {
                return false;
            }

            return true;
        }


        public void RemoveGameplayEffectWithTags(GameplayTag[] removeTags)
        {
            if (removeTags == null)
            {
                return;
            }

            foreach (GameplayEffectSpec spec in GameplayEffects.Values.ToArray())
            {
                foreach (GameplayTag tag in removeTags)
                {
                    if (spec.EffectTags.AssetTag == tag || spec.EffectTags.AttributeTags.Contains(tag))
                    {
                        RemoveGameplayEffectWithSpec(spec);

                        break;
                    }
                }
            }
        }
        public void RemoveGameplayEffectWithSpecs(List<GameplayEffectSpec> gameplayEffectSpecs)
        {
            if (gameplayEffectSpecs == null)
            {
                return;
            }

            foreach (GameplayEffectSpec spec in GameplayEffects.Values.ToArray())
            {
                foreach (GameplayEffectSpec removeSpec in gameplayEffectSpecs)
                {
                    if (spec == removeSpec)
                    {
                        RemoveGameplayEffectWithSpec(spec);
                    }
                }
            }
        }

        public void RemoveGameplayEffectWithSpec(GameplayEffectSpec gameplayEffectSpec)
        {
            if (gameplayEffectSpec == null)
            {
                return;
            }

            if(gameplayEffectSpec.Activate)
                gameplayEffectSpec.EndGameplayEffect();

            RemoveGameplayEffectList(gameplayEffectSpec);
        }


        public void AddGameplayEffectList(GameplayEffectSpec gameplayEffectSpec)
        {
            _GameplayEffects.TryAdd(gameplayEffectSpec.GameplayEffect, gameplayEffectSpec);
        }
        public void RemoveGameplayEffectList(GameplayEffect gameplayEffect)
        {
            _GameplayEffects.Remove(gameplayEffect);
        }
        public void RemoveGameplayEffectList(GameplayEffectSpec gameplayEffectSpec)
        {
            _GameplayEffects.Remove(gameplayEffectSpec.GameplayEffect);
        }

        private void Update()
        {
            OnUpdatedEffect?.Invoke(this, Time.deltaTime);
        }
        private void FixedUpdate()
        {
            OnFixedUpdatedEffect?.Invoke(this, Time.fixedDeltaTime);
        }
    }
}
