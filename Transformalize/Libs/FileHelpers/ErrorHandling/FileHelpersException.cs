#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.FileHelpers.ErrorHandling
{
    /// <summary>Base class for all the library Exceptions.</summary>
    public class FileHelpersException : Exception
    {
        /// <summary>Basic constructor of the exception.</summary>
        /// <param name="message">Message of the exception.</param>
        public FileHelpersException(string message) : base(message)
        {
        }

        /// <summary>Basic constructor of the exception.</summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerEx">The inner Exception.</param>
        public FileHelpersException(string message, Exception innerEx) : base(message, innerEx)
        {
        }
    }
}