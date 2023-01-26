using UnityEngine;
using System.Diagnostics;
using System;
using UnityEditor;

#if SCOR_ENABLE_GAMEPLAYTAG
using StudioScor.GameplayTagSystem;
#endif

namespace StudioScor.EffectSystem
{

    public abstract partial class GameplayEffect : ScriptableObject
    {
#if UNITY_EDITOR
        public bool AutoReName = false;
        public string FileName;

        private void OnValidate()
        {
            if (AutoReName && name != FileName)
            {
                name = FileName;
            }
        }

        
#endif

        [field: Header(" [ Gameplay Effect ] ")]
        


        [field: Header(" [ Debug ] ")]
        [field: SerializeField] public bool UseDebug { get; private set; } = false;

        public bool TryTakeEffect(GameplayEffectSystem target, int level = 0, object data = null)
        {
            if (!CanTakeEffect(target, level, data))
                return false;

            OnTakeEffect(target, level, data);

            return true;
        }
        public virtual bool CanTakeEffect(GameplayEffectSystem target, int level = 0, object data = null)
        {
            if (target is null)
                return false;

            if (!CheckGameplayTags(target)) 
                return false;

            return true;
        }
        public abstract void OnTakeEffect(GameplayEffectSystem target, int level = 0, object data = null);

    }
}

