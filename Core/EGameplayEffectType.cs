using UnityEngine;

namespace StudioScor.GameplayEffectSystem
{
    public enum EGameplayEffectType
    {
        [Tooltip("Applies Immediately.")]Instante,
        [Tooltip("Applies for Duration.")]Duration,
        [Tooltip("Applies Forever.")]Infinity,
    }
}

