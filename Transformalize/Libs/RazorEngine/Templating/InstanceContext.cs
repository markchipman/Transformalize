﻿#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Diagnostics.Contracts;

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Defines contextual information for a template instance.
    /// </summary>
    public class InstanceContext
    {
        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="InstanceContext" />.
        /// </summary>
        /// <param name="loader">The type loader.</param>
        /// <param name="templateType">The template type.</param>
        internal InstanceContext(TypeLoader loader, Type templateType)
        {
            Contract.Requires(loader != null);
            Contract.Requires(templateType != null);

            Loader = loader;
            TemplateType = templateType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the type loader.
        /// </summary>
        public TypeLoader Loader { get; private set; }

        /// <summary>
        ///     Gets the template type.
        /// </summary>
        public Type TemplateType { get; private set; }

        #endregion
    }
}