#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.Rhino.Etl.Pipelines
{
    /// <summary>
    ///     Exception class for when a pipeline throws an execution error.
    /// </summary>
    public class PipelineExecutionException : Exception
    {
        internal PipelineExecutionException(string Message, Exception InnerException)
            : base(Message, InnerException)
        {
        }
    }
}