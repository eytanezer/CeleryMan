namespace Effects
{
    public class StunEffect : Effect
    {
        private float _originalSpeed;

        public StunEffect(float duration) : base(duration) { }

        public override void Apply(PlayerMovement player)
        {
            _originalSpeed = player.Speed;
            player.Speed = 0f;
        }

        public override void Remove(PlayerMovement player)
        {
            player.Speed = _originalSpeed;
        }
    }
}