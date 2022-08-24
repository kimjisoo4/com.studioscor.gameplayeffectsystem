using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace KimScor.GameplayTagSystem.Effect
{
    public class GameplayEffectSystem : MonoBehaviour
    {
        #region Events
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
        public bool ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect, out GameplayEffectSpec spec)
        {
            spec = gameplayEffect.CreateSpec(this);

            if (!spec.TryGameplayEffect())
            {
                return false;
            }

            return true;
        }
        public void ApplyGameplayEffectsToSelf(IReadOnlyCollection<GameplayEffect> gameplayEffects)
        {
            if (gameplayEffects is null)
                return;

            foreach (var spec in gameplayEffects)
            {
                ApplyGameplayEffectToSelf(spec);
            }
        }
        public void ApplyGameplayEffectsToSelf(GameplayEffect[] gameplayEffects)
        {
            if (gameplayEffects is null)
                return;

            foreach (var spec in gameplayEffects)
            {
                ApplyGameplayEffectToSelf(spec);
            }
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
        public void RemoveGameplayEffect(GameplayEffect gameplayEffect)
        {
            if (gameplayEffect is null)
                return;

            if(_GameplayEffects.TryGetValue(gameplayEffect, out GameplayEffectSpec spec))
            {
                if(spec.Activate)
                    spec.EndGameplayEffect();

                RemoveGameplayEffectList(spec);
            }
        }
        public void RemoveGameplayEffects(GameplayEffect[] gameplayEffects)
        {
            if (gameplayEffects is null)
                return;

            foreach (var effect in gameplayEffects)
            {
                RemoveGameplayEffect(effect);
            }
        }
        public void RemoveGameplayEffects(IReadOnlyCollection<GameplayEffect> gameplayEffects)
        {
            if (gameplayEffects is null)
                return;

            foreach (var effect in gameplayEffects)
            {
                RemoveGameplayEffect(effect);
            }
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
            float deltaTime = Time.deltaTime;
            
            for (int i = 0; i < GameplayEffects.Count; i++)
            {
                GameplayEffects.ElementAt(i).Value.OnUpdateEffect(deltaTime);
            }
        }
        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;

            for (int i = 0; i < GameplayEffects.Count; i++)
            {
                GameplayEffects.ElementAt(i).Value.OnFixedUpdateEffect(deltaTime);
            }
        }
    }
}
