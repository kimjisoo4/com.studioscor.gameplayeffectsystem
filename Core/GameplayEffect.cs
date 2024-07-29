using UnityEngine;
using UnityEngine.Pool;
using StudioScor.Utilities;

namespace StudioScor.GameplayEffectSystem
{
    public abstract partial class GameplayEffect : BaseScriptableObject
    {
        [Header(" [ Gameplay Effect ]")]
        [SerializeField] protected EGameplayEffectType _effectType;
        [SerializeField][SEnumCondition(nameof(_effectType), (int)EGameplayEffectType.Duration)] protected float _duration = 5f;
        [SerializeField] protected bool _unscaledTime = false;

        public EGameplayEffectType Type => _effectType;
        public float Duration => _duration;
        public bool UnscaledTime => _unscaledTime;

        public abstract IGameplayEffectSpec CreateSpec(IGameplayEffectSystem gameplayEffectSystem, GameObject instigator = null, int level = 0, object data = default);
    }
}

