namespace StudioScor.GameplayEffectSystem
{
    public interface IGameplayEffectSpec
    {
        public GameplayEffect GameplayEffect { get; }
        public GameplayEffectSystemComponent Owner { get; }
        public GameplayEffectSystemComponent Instigator { get; }

        public bool IsActivate { get; }
        public int Level { get; }
        public float RemainTime { get; }

        public void SetupSpec(GameplayEffectSystemComponent owner, GameplayEffectSystemComponent instigator, int level = 0, object data = default);
        public bool CanTakeEffect();
        public bool TryTakeEffect();
        public void ForceTakeEffect();

        public void UpdateEffect(float deltaTime);

        public void EndEffect();

        public void ForceRemoveEffect();
        public bool CanRemoveEffectFromSource(object source);
        public void ChangeLevel(int level);

        public void ForceOverlapEffect(int level);
        public bool CanOverlapEffect(int level);
        public bool TryOverlapEffect(int level);


        public event EffectSpecStateHandler OnActivateEffect;
        public event EffectSpecStateHandler OnCanceledEffect;
        public event EffectSpecStateHandler OnFinishedEffect;
        public event EffectSpecStateHandler OnEndedEffect;
        public event EffectSpecStateHandler OnOverlappedEffect;

        public event EffectSpecLevelStateHandler OnChangedEffectLevel;
    }
}
