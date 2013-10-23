#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.Ninject.Activation.Caching;

namespace Transformalize.Libs.Ninject.Activation.Strategies
{
    /// <summary>
    ///     Adds all activated instances to the activation cache.
    /// </summary>
    public class ActivationCacheStrategy : IActivationStrategy
    {
        /// <summary>
        ///     The activation cache.
        /// </summary>
        private readonly IActivationCache activationCache;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivationCacheStrategy" /> class.
        /// </summary>
        /// <param name="activationCache">The activation cache.</param>
        public ActivationCacheStrategy(IActivationCache activationCache)
        {
            this.activationCache = activationCache;
        }

        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        /// <value>The ninject settings.</value>
        public INinjectSettings Settings { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        ///     Contributes to the activation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public void Activate(IContext context, InstanceReference reference)
        {
            activationCache.AddActivatedInstance(reference.Instance);
        }

        /// <summary>
        ///     Contributes to the deactivation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public void Deactivate(IContext context, InstanceReference reference)
        {
            activationCache.AddDeactivatedInstance(reference.Instance);
        }
    }
}