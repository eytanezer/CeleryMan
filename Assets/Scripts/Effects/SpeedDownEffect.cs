namespace Effects
{
    /// <summary>
    /// Decreases the player's movement speed for a duration.
    /// </summary>
    public class SpeedDownEffect : Effect
    {
        private float _originalSpeed;
        private const float SpeedMultiplier = 0.5f; // 50% slower

        public SpeedDownEffect(float duration) : base(duration) { }

        public override void Apply(PlayerMovement player)
        {
            _originalSpeed = player.Speed;
            player.Speed = _originalSpeed * SpeedMultiplier;
        }

        public override void Remove(PlayerMovement player)
        {
            player.Speed = _originalSpeed;
        }
    }
}