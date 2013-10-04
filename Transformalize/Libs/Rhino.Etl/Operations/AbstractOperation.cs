#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Represent a single operation that can occure during the ETL process
    /// </summary>
    public abstract class AbstractOperation : WithLoggingMixin, IOperation
    {
        private readonly OperationStatistics _statistics = new OperationStatistics();

        /// <summary>
        ///     Gets the pipeline executer.
        /// </summary>
        /// <value>The pipeline executer.</value>
        protected IPipelineExecuter PipelineExecuter { get; private set; }

        /// <summary>
        ///     Gets the name of this instance
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        ///     Gets or sets whether we are using a transaction
        /// </summary>
        /// <value>True or false.</value>
        public bool UseTransaction { get; set; }

        /// <summary>
        ///     Gets the statistics for this operation
        /// </summary>
        /// <value>The statistics.</value>
        public OperationStatistics Statistics
        {
            get { return _statistics; }
        }

        /// <summary>
        ///     Occurs when a row is processed.
        /// </summary>
        public virtual event Action<IOperation, Row> OnRowProcessed = delegate { };

        /// <summary>
        ///     Occurs when all the rows has finished processing.
        /// </summary>
        public virtual event Action<IOperation> OnFinishedProcessing = delegate { };

        /// <summary>
        ///     Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        public virtual void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            PipelineExecuter = pipelineExecuter;
            Statistics.MarkStarted();
        }

        /// <summary>
        ///     Raises the row processed event
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        void IOperation.RaiseRowProcessed(Row dictionary)
        {
            Statistics.MarkRowProcessed();
            OnRowProcessed(this, dictionary);
        }

        /// <summary>
        ///     Raises the finished processing event
        /// </summary>
        void IOperation.RaiseFinishedProcessing()
        {
            Statistics.MarkFinished();
            OnFinishedProcessing(this);
        }

        /// <summary>
        ///     Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Exception> GetAllErrors()
        {
            return Errors;
        }

        /// <summary>
        ///     Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public abstract IEnumerable<Row> Execute(IEnumerable<Row> rows);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
        }
    }
}