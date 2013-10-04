#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.Ninject.Infrastructure
{
    /// <summary>
    ///     Inheritable weak reference base class for Silverlight
    /// </summary>
    public abstract class BaseWeakReference
    {
        private readonly WeakReference innerWeakReference;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReferenceEqualWeakReference" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        protected BaseWeakReference(object target)
        {
            innerWeakReference = new WeakReference(target);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReferenceEqualWeakReference" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trackResurrection">
        ///     if set to <c>true</c> [track resurrection].
        /// </param>
        protected BaseWeakReference(object target, bool trackResurrection)
        {
            innerWeakReference = new WeakReference(target, trackResurrection);
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        public bool IsAlive
        {
            get { return innerWeakReference.IsAlive; }
        }

        /// <summary>
        ///     Gets or sets the target of this weak reference.
        /// </summary>
        /// <value>The target of this weak reference.</value>
        public object Target
        {
            get { return innerWeakReference.Target; }

            set { innerWeakReference.Target = value; }
        }
    }
}