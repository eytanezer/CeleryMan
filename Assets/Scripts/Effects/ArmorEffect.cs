namespace Effects
{
    /// <summary>
    /// Grants the player armor, managed by the PlayerEffectManager.
    /// </summary>
    public class ArmorEffect : Effect
    {
        public ArmorEffect(float duration) : base(duration) { }

        public override void Apply(PlayerMovement player)
        {
            // Fetch the manager and turn the armor ON
            PlayerEffectManager manager = player.GetComponent<PlayerEffectManager>();
            if (manager)
            {
                manager.HasArmor = true;
            }
        }

        public override void Remove(PlayerMovement player)
        {
            // Fetch the manager and turn the armor OFF
            PlayerEffectManager manager = player.GetComponent<PlayerEffectManager>();
            if (manager)
            {
                manager.HasArmor = false;
            }
        }
    }
}