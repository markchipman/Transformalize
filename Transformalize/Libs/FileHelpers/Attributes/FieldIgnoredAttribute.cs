#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.FileHelpers.Attributes
{
    /// <summary>Indicates that the target field is completely ignored by the Engine (i.e. This field don�t exists for the library).</summary>
    /// <remarks>
    ///     See the <a href="attributes.html">Complete Attributes List</a> for more clear info and examples of each one.
    /// </remarks>
    /// <seealso href="attributes.html">Attributes List</seealso>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldIgnoredAttribute : FieldAttribute
    {
    }
}