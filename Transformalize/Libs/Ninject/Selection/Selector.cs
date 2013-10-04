#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Language;
using Transformalize.Libs.Ninject.Selection.Heuristics;

#endregion

namespace Transformalize.Libs.Ninject.Selection
{
    /// <summary>
    ///     Selects members for injection.
    /// </summary>
    public class Selector : NinjectComponent, ISelector
    {
        private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Selector" /> class.
        /// </summary>
        /// <param name="constructorScorer">The constructor scorer.</param>
        /// <param name="injectionHeuristics">The injection heuristics.</param>
        public Selector(IConstructorScorer constructorScorer, IEnumerable<IInjectionHeuristic> injectionHeuristics)
        {
            Ensure.ArgumentNotNull(constructorScorer, "constructorScorer");
            Ensure.ArgumentNotNull(injectionHeuristics, "injectionHeuristics");

            ConstructorScorer = constructorScorer;
            InjectionHeuristics = injectionHeuristics.ToList();
        }

        /// <summary>
        ///     Gets the default binding flags.
        /// </summary>
        protected virtual BindingFlags Flags
        {
            get
            {
#if !NO_LCG && !SILVERLIGHT
                return Settings.InjectNonPublic ? (DefaultFlags | BindingFlags.NonPublic) : DefaultFlags;
#else
                return DefaultFlags;
                #endif
            }
        }

        /// <summary>
        ///     Gets or sets the constructor scorer.
        /// </summary>
        public IConstructorScorer ConstructorScorer { get; set; }

        /// <summary>
        ///     Gets the property injection heuristics.
        /// </summary>
        public ICollection<IInjectionHeuristic> InjectionHeuristics { get; private set; }

        /// <summary>
        ///     Selects the constructor to call on the specified type, by using the constructor scorer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     The selected constructor, or <see langword="null" /> if none were available.
        /// </returns>
        public virtual IEnumerable<ConstructorInfo> SelectConstructorsForInjection(Type type)
        {
            Ensure.ArgumentNotNull(type, "type");

            var constructors = type.GetConstructors(Flags);
            return constructors.Length == 0 ? null : constructors;
        }

        /// <summary>
        ///     Selects properties that should be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected properties.</returns>
        public virtual IEnumerable<PropertyInfo> SelectPropertiesForInjection(Type type)
        {
            Ensure.ArgumentNotNull(type, "type");
            var properties = new List<PropertyInfo>();
            properties.AddRange(
                type.GetProperties(Flags)
                    .Select(p => p.GetPropertyFromDeclaredType(p, Flags))
                    .Where(p => InjectionHeuristics.Any(h => h.ShouldInject(p))));
#if !SILVERLIGHT
            if (Settings.InjectParentPrivateProperties)
            {
                for (var parentType = type.BaseType; parentType != null; parentType = parentType.BaseType)
                {
                    properties.AddRange(GetPrivateProperties(type.BaseType));
                }
            }
#endif

            return properties;
        }

        /// <summary>
        ///     Selects methods that should be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected methods.</returns>
        public virtual IEnumerable<MethodInfo> SelectMethodsForInjection(Type type)
        {
            Ensure.ArgumentNotNull(type, "type");
            return type.GetMethods(Flags).Where(m => InjectionHeuristics.Any(h => h.ShouldInject(m)));
        }

        private IEnumerable<PropertyInfo> GetPrivateProperties(Type type)
        {
            return type.GetProperties(Flags).Where(p => p.DeclaringType == type && p.IsPrivate())
                       .Where(p => InjectionHeuristics.Any(h => h.ShouldInject(p)));
        }
    }
}