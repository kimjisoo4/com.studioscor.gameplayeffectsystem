using System.Collections.Generic;
using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.GameplayEffectSystem
{

    public static class GameplayEffectSystemUtility
    {
        #region Get GameplayEffectSystem
        public static IGameplayEffectSystem GetGameplayEffectSystem(this GameObject gameObject)
        {
            return gameObject.GetComponent<IGameplayEffectSystem>();
        }
        public static IGameplayEffectSystem GetGameplayEffectSystem(this Component component)
        {
            if (component is IGameplayEffectSystem gameplayEffecSystem)
                return gameplayEffecSystem;

            return component.GetComponent<IGameplayEffectSystem>();
        }
        public static bool TryGetGameplayEffectSystem(this GameObject gameObject, out IGameplayEffectSystem gameplayEffectSystem)
        {
            return gameObject.TryGetComponent(out gameplayEffectSystem);
        }
        public static bool TryGetGameplayEffectSystem(this Component component, out IGameplayEffectSystem gameplayEffectSystem)
        {
            gameplayEffectSystem = component as IGameplayEffectSystem;

            if (gameplayEffectSystem is not null)
                return true;

            return component.TryGetComponent(out gameplayEffectSystem);
        }
        #endregion

        public static bool HasEffect(this IGameplayEffectSystem gameplayEffectSytstem, GameplayEffect containEffect)
        {
            foreach (var effect in gameplayEffectSytstem.GameplayEffects)
            {
                if (effect.GameplayEffect == containEffect)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool TryGetGameplayEffectSpec(this IGameplayEffectSystem gameplayEffectSytstem, GameplayEffect containEffect, out IGameplayEffectSpec spec)
        {
            foreach (var effect in gameplayEffectSytstem.GameplayEffects)
            {
                if (effect.GameplayEffect == containEffect)
                {
                    spec = effect;

                    return true;
                }
            }

            spec = null;

            return false;
        }
    }

    public interface IGameplayEffectSystem
    {
        public delegate void ChangeGameplayEffectHandler(IGameplayEffectSystem effectSystem, IGameplayEffectSpec effectSpec);
        public Transform transform { get; }
        public GameObject gameObject { get; }

        public IReadOnlyList<IGameplayEffectSpec> GameplayEffects { get; }

        public void Tick(float deltaTime);
        public bool TryApplyGameplayEffect(GameplayEffect effect, GameObject instigator = null, int level = 0, object data = default);
        public bool TryApplyGameplayEffect(GameplayEffect effect, GameObject instigator, int level, object data, out IGameplayEffectSpec spec);
        public void CancelEffect(GameplayEffect effect);
        public void CancelEffectFromSource(object source);
        public void CancelAllEffect();

        public event ChangeGameplayEffectHandler OnGrantedEffect;
        public event ChangeGameplayEffectHandler OnRemovedEffect;
    }

    [System.Serializable]
    public struct FGameplayEffect
    {
        public string EffectName;
        public GameplayEffect GameplayEffect;
        public int Level;
    }

    [AddComponentMenu("StudioScor/GameplayEffectSystem/GameplayEffect System Component", order: 0)]
    public class GameplayEffectSystemComponent : BaseMonoBehaviour, IGameplayEffectSystem
    {
        [Header(" [ Gameplay Effect System ] ")]
        [SerializeField] private FGameplayEffect[] initGameplayEffects;
        private List<IGameplayEffectSpec> gameplayEffects;
        public IReadOnlyList<IGameplayEffectSpec> GameplayEffects => gameplayEffects;

        public event IGameplayEffectSystem.ChangeGameplayEffectHandler OnGrantedEffect;
        public event IGameplayEffectSystem.ChangeGameplayEffectHandler OnRemovedEffect;

        private void OnValidate()
        {
#if UNITY_EDITOR
            if(initGameplayEffects is not null && initGameplayEffects.Length > 0)
            {
                for(int i = 0; i < initGameplayEffects.Length; i++)
                {
                    if(initGameplayEffects[i].GameplayEffect)
                    {
                        initGameplayEffects[i].EffectName = initGameplayEffects[i].GameplayEffect.name;
                    }
                }
            }
#endif
        }
        
        private void Awake()
        {
            Setup();
        }

        void Start()
        {
            foreach(var initGameplayEffect in initGameplayEffects)
            {
                TryApplyGameplayEffect(initGameplayEffect.GameplayEffect, gameObject, initGameplayEffect.Level, null);
            }
        }

        protected void Setup()
        {
            Log("Setup");

            gameplayEffects = new();
        }
        protected virtual void OnSetup() { }

        public void ResetEffectSystem()
        {
            Log("Reset");

            CancelAllEffect();

            gameplayEffects.Clear();

            OnReset();
        }
        protected virtual void OnReset() { }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            for (int i = GameplayEffects.LastIndex(); i >= 0; i--)
            {
                var gameplayEffect = GameplayEffects[i];

                gameplayEffect.UpdateEffect(deltaTime);

                if (!gameplayEffect.IsActivate)
                    RemoveEffectSpec(gameplayEffect);
            }
        }

        public bool TryApplyGameplayEffect(GameplayEffect effect, GameObject instigator = null, int level = 0, object data = default)
        {
            if (!effect)
                return false;

            var spec = effect.CreateSpec(this, instigator, level, data);

            if (spec.TryTakeEffect())
            {
                Callback_OnGrantedEffect(spec);

                if (spec.IsActivate)
                    gameplayEffects.Add(spec);

                return true;
            }
            else
            {
                spec.ReleaseSpec();
            }

            return false;
        }
        public bool TryApplyGameplayEffect(GameplayEffect effect, GameObject instigator, int level, object data, out IGameplayEffectSpec spec)
        {
            spec = null;

            if (!effect)
                return false;

            spec = effect.CreateSpec(this, instigator, level, data);

            if (spec.TryTakeEffect())
            {
                Callback_OnGrantedEffect(spec);

                if (spec.IsActivate)
                    gameplayEffects.Add(spec);

                return true;
            }
            else
            {
                spec.ReleaseSpec();
                spec = null;
            }

            return false;
        }



        #region Remove Effect
        public void CancelEffect(GameplayEffect effect)
        {
            if (effect is null)
                return;

            if (this.TryGetGameplayEffectSpec(effect, out IGameplayEffectSpec spec))
            {
                spec.ForceCancelEffect();
            }
        }
        
        public void CancelAllEffect()
        {
            foreach (var spec in GameplayEffects)
            {
                spec.ForceCancelEffect();
            }
        }

        public void CancelEffectFromSource(object source)
        {
            foreach (var spec in GameplayEffects)
            {
                if (spec.CancelEffectFromSource(source))
                {
                    spec.ForceCancelEffect();
                }
            }
        }

        private void RemoveEffectSpec(IGameplayEffectSpec effectSpec)
        {
            gameplayEffects.Remove(effectSpec);

            Callback_OnRemovedEffect(effectSpec);
        }

        #endregion

        #region Callback
        protected virtual void Callback_OnGrantedEffect(IGameplayEffectSpec addedSpec)
        {
            Log("On Granted Effect - " + addedSpec);

            OnGrantedEffect?.Invoke(this, addedSpec);
        }
        protected virtual void Callback_OnRemovedEffect(IGameplayEffectSpec removedSpec)
        {
            Log("On Removed Effect - " + removedSpec);

            OnRemovedEffect?.Invoke(this, removedSpec);
        }

        #endregion

    }
}
