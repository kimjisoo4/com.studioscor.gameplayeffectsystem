namespace StudioScor.GameplayEffectSystem
{
    public interface IGameplayEffectSpecEvent
    {
        public event EffectSpecStateHandler OnActivateEffect;
        public event EffectSpecStateHandler OnCanceledEffect;
        public event EffectSpecStateHandler OnFinishedEffect;
        public event EffectSpecStateHandler OnEndedEffect;
        public event EffectSpecStateHandler OnOverlappedEffect;

        public event EffectSpecLevelStateHandler OnChangedEffectLevel;
    }
    public interface IGameplayEffectSpec
    {
        public GameplayEffect GameplayEffect { get; }
        public IGameplayEffectSystem GameplayEffectSystem { get; }

        public bool IsActivate { get; }
        public int Level { get; }
        public float RemainTime { get; }
        public object Data { get; }

        public void SetupSpec(GameplayEffect gameplayEffect, IGameplayEffectSystem gameplayEffectSystem, int level = 0, object data = default);

        public void Copy(IGameplayEffectSpec effectSpec);
        public bool CanTakeEffect();
        public bool TryTakeEffect();
        public void ForceTakeEffect();

        public void UpdateEffect(float deltaTime);

        public void EndEffect();

        public void ForceCancelEffect();
        public bool CancelEffectFromSource(object source);
        public void ChangeLevel(int level);

        public void ForceOverlapEffect(IGameplayEffectSpec effectSpec);
        public bool CanOverlapEffect(IGameplayEffectSpec effectSpec);
        public bool TryOverlapEffect(IGameplayEffectSpec effectSpec);
    }
}
