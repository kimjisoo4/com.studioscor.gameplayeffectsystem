using StudioScor.Utilities;
using UnityEngine;

namespace StudioScor.GameplayEffectSystem.Extend.TaskSystem
{
    public class TakeGameplayEffectsTaskAction : TaskAction
    {
        [Header(" [ Take Gameplay Effect Action ] ")]
#if SCOR_ENABLE_SERIALIZEREFERENCE
        [SerializeReference, SerializeReferenceDropdown]
#endif
        private IGameObjectVariable _instigator = new SelfGameObjectVariable();

        [SerializeField] private GameplayEffect[] _gameplayEffects;
#if SCOR_ENABLE_SERIALIZEREFERENCE
        [SerializeReference, SerializeReferenceDropdown]
#endif
         private IIntegerVariable _level = new DefaultIntegerVariable();

        private TakeGameplayEffectsTaskAction _original;

        public override void Setup(GameObject owner)
        {
            base.Setup(owner);

            _instigator.Setup(Owner);
            _level.Setup(Owner);
        }
        public override ITaskAction Clone()
        {
            var clone = new TakeGameplayEffectsTaskAction();

            clone._original = this;
            clone._instigator = _instigator.Clone();
            clone._level = _level.Clone();

            return clone;
        }

        public override void Action(GameObject target)
        {
            if(target.TryGetGameplayEffectSystem(out IGameplayEffectSystem gameplayEffectSystem))
            {
                var effects = _original is null ? _gameplayEffects : _original._gameplayEffects;
                var owner = _instigator.GetValue();

                foreach (var effect in effects)
                {
                    gameplayEffectSystem.TryTakeEffect(effect, owner, _level.GetValue());
                }
                
            }
        }
    }
}
