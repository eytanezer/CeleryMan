namespace Effects
{
    public class ReverseEffect : Effect
    {
        public ReverseEffect(float duration) : base(duration) { }
        public override void Apply(PlayerMovement player)
        {
            player.Speed = -player.Speed;
        }

        public override void Remove(PlayerMovement player)
        {
            player.Speed = -player.Speed;
        }
    }
}