using StudioScor.Utilities;
using UnityEngine;

namespace StudioScor.GameplayEffectSystem.Extend.TaskSystem
{
    public class TakeGameplayEffectsTaskAction : TaskAction
    {
        [Header(" [ Take Gameplay Effect Action ] ")]
        [SerializeField] private GameplayEffect[] _gameplayEffects;
        [SerializeReference]
#if SCOR_ENABLE_SERIALIZEREFERENCE
        [SerializeReferenceDropdown]
#endif
         private IIntegerVariable _level = new DefaultIntegerVariable();

        private TakeGameplayEffectsTaskAction _original;

        public override void Setup(GameObject owner)
        {
            base.Setup(owner);

            _level.Setup(Owner);
        }
        public override ITaskAction Clone()
        {
            var clone = new TakeGameplayEffectsTaskAction();

            clone._original = this;
            clone._level = _level.Clone();

            return clone;
        }

        public override void Action(GameObject target)
        {
            if(target.TryGetGameplayEffectSystem(out IGameplayEffectSystem gameplayEffectSystem))
            {
                var effects = _original is null ? _gameplayEffects : _original._gameplayEffects;

                foreach (var effect in effects)
                {
                    gameplayEffectSystem.TryTakeEffect(effect, Owner, _level.GetValue());
                }
                
            }
        }
    }
}
