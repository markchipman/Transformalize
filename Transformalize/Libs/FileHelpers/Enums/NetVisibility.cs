#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.FileHelpers.Enums
{
    /// <summary>Indicates the visibility of a member.</summary>
    public enum NetVisibility
    {
        /// <summary>Public visivility.</summary>
        Public = 0,

        /// <summary>Internal visivility. (Friend in VB.NET)</summary>
        Internal,

        /// <summary>Protected visivility.</summary>
        Protected,

        /// <summary>Private visivility.</summary>
        Private
    }
}