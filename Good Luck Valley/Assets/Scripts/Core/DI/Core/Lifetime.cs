namespace GoodLuckValley.Core.DI.Core
{
    /// <summary>
    /// Defines the lifetime management strategy for a dependency registration.
    /// </summary> 
    internal enum Lifetime
    {
        /// <summary>
        /// A single instance is created and reused for all resolutions within the owning container.
        /// </summary>
        Singleton,
        
        /// <summary>
        /// A new instance is created for each resolution request.
        /// </summary>
        Transient
    }
}