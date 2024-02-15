using UnityEngine;
using UnityEngine.Pool;
using StudioScor.Utilities;

namespace StudioScor.GameplayEffectSystem
{
    public abstract partial class GameplayEffect : BaseScriptableObject
    {
        [Header(" [ Gameplay Effect ]")]
        [SerializeField] protected EGameplayEffectType _EffectType;
        [SerializeField][SEnumCondition(nameof(_EffectType), (int)EGameplayEffectType.Duration)] protected float _Duration = 5f;
        [SerializeField] protected bool _UnscaledTime = false;

        public EGameplayEffectType Type => _EffectType;
        public float Duration => _Duration;


        public bool UnscaledTime => _UnscaledTime;

        public abstract IGameplayEffectSpec CreateSpec(IGameplayEffectSystem gameplayEffectSystem, GameObject instigator = null, int level = 0, object data = default);
    }
}

