using UnityEngine;
using UnityEngine.Pool;
using StudioScor.Utilities;

namespace StudioScor.GameplayEffectSystem
{
    [System.Serializable]
    public struct FGameplayEffect
    {
        public GameplayEffect Effect;
        public int Level;
        public float Strength;
        public object Data;

        public FGameplayEffect(GameplayEffect effect, int level, float strength, object data)
        {
            Effect = effect;
            Level = level;
            Data = data;
            Strength = strength;
        }
    }

    public abstract partial class GameplayEffect : BaseScriptableObject
    {
        [Header(" [ Gameplay Effect ]")]
        [SerializeField] protected EGameplayEffectType _EffectType;
        [SerializeField][SEnumCondition(nameof(_EffectType), (int)EGameplayEffectType.Duration)] protected float _Duration = 5f;
        
        public EGameplayEffectType Type => _EffectType;
        public float Duration => _Duration;

        private ObjectPool<IGameplayEffectSpec> _Pool;

        public IGameplayEffectSpec CreateSpec(IGameplayEffectSystem gameplayEffectSystem, int level = 0, object data = null)
        {
            if (_Pool is null)
                _Pool = new(OnCreateSpec);

            var spec = _Pool.Get();

            spec.SetupSpec(gameplayEffectSystem, level, data);

            return spec;
        }

        public void ReleaseSpec(IGameplayEffectSpec spec)
        {
            _Pool.Release(spec);
        }

        protected abstract IGameplayEffectSpec OnCreateSpec();

    }
}

