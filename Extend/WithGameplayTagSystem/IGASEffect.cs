#if SCOR_ENABLE_GAMEPLAYTAGSYSTEM
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using StudioScor.GameplayTagSystem;

namespace StudioScor.GameplayEffectSystem
{
    public interface IGASEffect
    {
        public GameplayTagSO EffectTag { get; }
        public IReadOnlyCollection<GameplayTagSO> AttributeTags { get; }
        public FConditionTags ConditionTags { get; }
        public FGameplayTags GrantTags { get; }
        public IReadOnlyCollection<GameplayTagSO> CancelEffectTags { get; }
    }
    public interface IGASGameplayEffect
    {
        public GameplayTagSO EffectTag { get; }
        public IReadOnlyCollection<GameplayTagSO> AttributeTags { get; }
        public FConditionTags ConditionTags { get; }
        public FGameplayTags GrantTags { get; }
        public IReadOnlyCollection<GameplayTagSO> CancelEffectTags { get; }
    }

    public abstract class GASGameplayEffect : GameplayEffect, IGASGameplayEffect
    {
        [Header(" [ Character Effect ] ")]
        [SerializeField] protected GameplayTagSO _effectTag;
        [SerializeField] private GameplayTagSO[] _attributeTags;
        [SerializeField] private FConditionTags _conditionTags;
        [SerializeField] private FGameplayTags _grantTags;
        [SerializeField] private GameplayTagSO[] _cancelEffectTags;
        public GameplayTagSO EffectTag => _effectTag;
        public IReadOnlyCollection<GameplayTagSO> AttributeTags => _attributeTags;
        public FConditionTags ConditionTags => _conditionTags;
        public FGameplayTags GrantTags => _grantTags;
        public IReadOnlyCollection<GameplayTagSO> CancelEffectTags => _cancelEffectTags;
    }

    public abstract class GASGameplayEffectSpec : GameplayEffectSpec
    {
        protected new GASGameplayEffect _gameplayEffect;
        protected IGameplayTagSystem _gameplayTagSystem;

        public override void SetupSpec(GameplayEffect gameplayEffect, IGameplayEffectSystem gameplayEffectSystem, GameObject instigator, int level = 0, object data = null)
        {
            base.SetupSpec(gameplayEffect, gameplayEffectSystem, instigator, level, data);

            this._gameplayEffect = gameplayEffect as GASGameplayEffect;
            _gameplayTagSystem = gameplayEffectSystem.gameObject.GetGameplayTagSystem();
        }

        public override bool CanTakeEffect()
        {
            if (!base.CanTakeEffect())
                return false;

            if (_gameplayTagSystem.ContainBlockTag(_gameplayEffect.EffectTag))
            {
                Log($"Block Effect Tag");

                return false;
            }
            if (_gameplayTagSystem.ContainAnyTagsInBlock(_gameplayEffect.AttributeTags))
            {
                Log($"Block Atrribute Tags");

                return false;
            }

            if(!_gameplayTagSystem.ContainConditionTags(_gameplayEffect.ConditionTags))
            {
                Log($"Contain Condition Tags is False");

                return false;
            }

            return true;
        }

        public override bool CancelEffectFromSource(object source)
        {
            Log(" Can Remove Effect From Source ? ");

            IReadOnlyCollection<GameplayTagSO> gameplayTags = source as IReadOnlyCollection<GameplayTagSO>;

            if (gameplayTags is null)
                return false;

            if (gameplayTags.Contains(_gameplayEffect.EffectTag))
            {
                Log($"Remove Effect From Source Contain EffectTag [{_gameplayEffect.EffectTag.name}]");

                return true;
            }

            foreach (var tag in gameplayTags)
            {
                if (_gameplayEffect.AttributeTags.Contains(tag))
                {
                    Log($"Remove Effect From Source [{tag.name}]");

                    return true;
                }
            }

            return false;
        }

        protected override void OnEnterEffect()
        {
            GameplayEffectSystem.CancelEffectFromSource(_gameplayEffect.CancelEffectTags);

            _gameplayTagSystem.AddGameplayTags(_gameplayEffect.GrantTags);

        }
        protected override void OnExitEffect()
        {
            _gameplayTagSystem.RemoveGameplayTags(_gameplayEffect.GrantTags);
        }
    }
}

#endif