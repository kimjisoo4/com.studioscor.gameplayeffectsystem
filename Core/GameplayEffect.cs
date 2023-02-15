using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.GameplayEffectSystem
{
    public abstract partial class GameplayEffect : BaseScriptableObject
    {
        [Header(" [ Gameplay Effect ]")]
        [SerializeField] protected EGameplayEffectType _EffectType;
        [SerializeField][SEnumCondition(nameof(_EffectType), (int)EGameplayEffectType.Duration)] protected float _Duration = 5f;
        
        public EGameplayEffectType Type => _EffectType;
        public float Duration => _Duration;
        public abstract IGameplayEffectSpec CreateSpec(GameplayEffectSystemComponent owner, GameplayEffectSystemComponent instigator, int level = 0, object data = null);
    }
}

