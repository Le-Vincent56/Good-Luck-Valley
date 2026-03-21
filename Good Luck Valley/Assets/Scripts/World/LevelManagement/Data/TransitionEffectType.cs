namespace GoodLuckValley.World.LevelManagement.Data
{
    /// <summary>
    /// Categorizes the visual transition effect used between scenes.
    /// Mapped to shader materials in <see cref="TransitionConfig"/>.
    /// </summary>
    public enum TransitionEffectType
    {
        Fade,
        DiamondWipe,
        CircleWipe
    }
}