namespace Effects
{
    /// <summary>
    /// Increases the player's movement speed for a duration.
    /// </summary>
    public class SpeedUpEffect : Effect
    {
        private float _originalSpeed;
        private const float SpeedMultiplier = 1.5f; // 50% faster

        public SpeedUpEffect(float duration) : base(duration) { }

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