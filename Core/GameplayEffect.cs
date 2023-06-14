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
        [SerializeField] protected EGameplayEffectType effectType;
        [SerializeField][SEnumCondition(nameof(effectType), (int)EGameplayEffectType.Duration)] protected float duration = 5f;

        public EGameplayEffectType Type => effectType;
        public float Duration => duration;

        public abstract IGameplayEffectSpec CreateSpec(IGameplayEffectSystem gameplayEffectSystem, int level = 0, object data = default);
    }
}

