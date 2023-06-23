using UnityEngine;
using UnityEngine.Pool;
using StudioScor.Utilities;

namespace StudioScor.GameplayEffectSystem
{
    public abstract partial class GameplayEffect : BaseScriptableObject
    {
        [Header(" [ Gameplay Effect ]")]
        [SerializeField] protected EGameplayEffectType effectType;
        [SerializeField][SEnumCondition(nameof(effectType), (int)EGameplayEffectType.Duration)] protected float duration = 5f;
        [SerializeField] protected bool useSpeed = true;

        public EGameplayEffectType Type => effectType;
        public float Duration => duration;

        public bool UseSpeed => useSpeed;

        public abstract IGameplayEffectSpec CreateSpec(IGameplayEffectSystem gameplayEffectSystem, int level = 0, object data = default);
    }
}

