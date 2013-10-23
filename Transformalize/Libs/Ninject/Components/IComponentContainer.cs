#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using System.Collections.Generic;

#endregion

namespace Transformalize.Libs.Ninject.Components
{
    /// <summary>
    ///     An internal container that manages and resolves components that contribute to Ninject.
    /// </summary>
    public interface IComponentContainer : IDisposable
    {
        /// <summary>
        ///     Gets or sets the kernel that owns the component container.
        /// </summary>
        IKernel Kernel { get; set; }

        /// <summary>
        ///     Registers a component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        void Add<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent;

        /// <summary>
        ///     Removes all registrations for the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        void RemoveAll<T>() where T : INinjectComponent;

        /// <summary>
        ///     Removes all registrations for the specified component.
        /// </summary>
        /// <param name="component">The component's type.</param>
        void RemoveAll(Type component);

        /// <summary>
        ///     Gets one instance of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The instance of the component.</returns>
        T Get<T>() where T : INinjectComponent;

        /// <summary>
        ///     Gets all available instances of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>A series of instances of the specified component.</returns>
        IEnumerable<T> GetAll<T>() where T : INinjectComponent;

        /// <summary>
        ///     Gets one instance of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>The instance of the component.</returns>
        object Get(Type component);

        /// <summary>
        ///     Gets all available instances of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>A series of instances of the specified component.</returns>
        IEnumerable<object> GetAll(Type component);

        /// <summary>
        ///     Registers a transient component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        void AddTransient<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent;
    }
}