using UnityEngine;
using System.Diagnostics;
using System.Linq;

#if SCOR_ENABLE_GAMEPLAYTAG
using StudioScor.GameplayTagSystem;
#endif

namespace StudioScor.EffectSystem
{
    public partial class GameplayEffectSystem : MonoBehaviour
    {
#if SCOR_ENABLE_GAMEPLAYTAG
        [Header(" [ Use GameplayTagSystem ] ")]
        [SerializeField] private GameplayTagSystemComponent _GameplayTagSystemComponent;

        public GameplayTagSystemComponent GameplayTagSystemComponent => _GameplayTagSystemComponent;
#endif

        [Conditional("SCOR_ENABLE_GAMEPLAYTAG")]
        private void SetupGameplayTag()
        {
#if SCOR_ENABLE_GAMEPLAYTAG
            if(!_GameplayTagSystemComponent)
            {
                if (!TryGetComponent(out _GameplayTagSystemComponent))
                {
                    Log("GameplayTag System Is Null", true);
                }
            }
#endif
        }

#if SCOR_ENABLE_GAMEPLAYTAG
        public void CancelGameplayEffectWithTags(GameplayTag[] removeTags)
        {
            if (removeTags is null)
                return;

            for (int i = GameplayEffects.Count - 1; i >= 0; i--)
            {
                foreach (var removeTag in removeTags)
                {
                    if (GameplayEffects[i].GameplayEffect.EffectTags.AssetTag == removeTag
                        || GameplayEffects[i].GameplayEffect.EffectTags.AttributeTags.Contains(removeTag)
                        || GameplayEffects[i].GameplayEffect.BaseEffectTags.Tags.AssetTag == removeTag
                        || GameplayEffects[i].GameplayEffect.BaseEffectTags.Tags.AttributeTags.Contains(removeTag))
                    {
                        CancelGamepalyEffect(GameplayEffects[i]);
                    }
                }
            }
        }
#endif
    }
}
