namespace GoodLuckValley.Core.SceneManagement.Enums
{
    /// <summary>
    /// Represents the current state of a scene loading operation.
    /// </summary>
    public enum SceneLoadState
    {
        Idle,
        Loading,
        Installing,
        Ready,
        Error
    }
}