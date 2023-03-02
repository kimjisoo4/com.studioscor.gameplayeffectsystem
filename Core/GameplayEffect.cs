using UnityEngine;
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
        public abstract IGameplayEffectSpec CreateSpec(IGameplayEffectSystem owner, IGameplayEffectSystem instigator, int level = 0, float strength = 0f, object data = null);
    }
}

