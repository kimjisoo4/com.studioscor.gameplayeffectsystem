﻿namespace StudioScor.GameplayEffectSystem
{
    public interface IGameplayEffectSpec
    {
        public GameplayEffect GameplayEffect { get; }
        public IGameplayEffectSystem Owner { get; }
        public IGameplayEffectSystem Instigator { get; }

        public bool IsActivate { get; }
        public int Level { get; }
        public float Strength { get; }
        public float RemainTime { get; }

        public void SetupSpec(IGameplayEffectSystem owner, IGameplayEffectSystem instigator, int level = 0, float strength = 0f, object data = default);

        public void Copy(IGameplayEffectSpec effectSpec);
        public bool CanTakeEffect();
        public bool TryTakeEffect();
        public void ForceTakeEffect();

        public void UpdateEffect(float deltaTime);

        public void EndEffect();

        public void ForceRemoveEffect();
        public bool CanRemoveEffectFromSource(object source);
        public void ChangeLevel(int level);

        public void ForceOverlapEffect(IGameplayEffectSpec effectSpec);
        public bool CanOverlapEffect(IGameplayEffectSpec effectSpec);
        public bool TryOverlapEffect(IGameplayEffectSpec effectSpec);


        public event EffectSpecStateHandler OnActivateEffect;
        public event EffectSpecStateHandler OnCanceledEffect;
        public event EffectSpecStateHandler OnFinishedEffect;
        public event EffectSpecStateHandler OnEndedEffect;
        public event EffectSpecStateHandler OnOverlappedEffect;

        public event EffectSpecLevelStateHandler OnChangedEffectLevel;
    }
}