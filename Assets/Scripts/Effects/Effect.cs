namespace Effects
{
    /// <summary>
    /// Base abstract class for all status effects.
    /// </summary>
    public abstract class Effect
    {
        public float Duration { get; }
        protected Effect(float duration)
        {
            Duration = duration;
        }
        public abstract void Apply(PlayerMovement player);
        public abstract void Remove(PlayerMovement player);
    }
}