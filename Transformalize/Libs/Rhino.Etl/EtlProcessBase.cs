#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Libs.Rhino.Etl {
    /// <summary>
    ///     Base class for etl processes, provider registration and management
    ///     services for the pipeline
    /// </summary>
    /// <typeparam name="TDerived">The type of the derived.</typeparam>
    public class EtlProcessBase<TDerived> : WithLoggingMixin
        where TDerived : EtlProcessBase<TDerived> {
        private readonly Process _process;

        protected Process Process { get { return _process; } }

        public EtlProcessBase(Process process) {
            _process = process;
            Logger = _process.Logger;
        }

        /// <summary>
        ///     Ordered list of the operations in this process that will be added to the
        ///     operations list after the initialization is completed.
        /// </summary>
        private readonly List<IOperation> _lastOperations = new List<IOperation>();

        /// <summary>
        ///     Ordered list of the operations in this process
        /// </summary>
        protected readonly List<IOperation> Operations = new List<IOperation>();

        /// <summary>
        ///     Gets the name of this instance
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name {
            get { return GetType().Name; }
        }

        /// <summary>
        ///     Gets or sets whether we are using a transaction
        /// </summary>
        /// <value>True or value.</value>
        public bool UseTransaction { get; set; }

        /// <summary>
        ///     Registers the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public TDerived Register(IOperation operation) {
            operation.UseTransaction = UseTransaction;
            operation.LogRows = _process.LogRows;
            operation.ProcessName = _process.Name;
            operation.Logger = Logger;
            Operations.Add(operation);
            Logger.Debug("Register {0}", operation.Name ?? operation.GetType().Name);
            return (TDerived)this;
        }

        /// <summary>
        ///     Merges the last operations to the operations list.
        /// </summary>
        protected void MergeLastOperationsToOperations() {
            Operations.AddRange(_lastOperations);
        }
    }
}