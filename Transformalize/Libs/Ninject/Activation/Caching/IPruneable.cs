#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.Ninject.Activation.Caching
{
    /// <summary>
    ///     An object that is prunealble.
    /// </summary>
    public interface IPruneable
    {
        /// <summary>
        ///     Removes instances from the cache which should no longer be re-used.
        /// </summary>
        void Prune();
    }
}