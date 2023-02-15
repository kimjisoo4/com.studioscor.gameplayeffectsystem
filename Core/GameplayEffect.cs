using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.EffectSystem
{
    public abstract partial class GameplayEffect : BaseScriptableObject
    {
        [Header(" [ Gameplay Effect ]")]
        [SerializeField] protected EEffectType _EffectType;
        [SerializeField][SEnumCondition(nameof(_EffectType), (int)EEffectType.Duration)] protected float _Duration = 5f;
        
        public EEffectType Type => _EffectType;
        public float Duration => _Duration;
        public abstract IEffectSpec CreateSpec(EffectSystemComponent owner, EffectSystemComponent instigator, int level = 0, object data = null);
    }
}

