using System;
using System.Data;

namespace DataRecordDecorators
{
    /// <summary>
    /// Provides additional helper methods to the IDataRecord interface.
    /// </summary>
    public sealed class DataRecordDecorator : BaseDataRecordDecorator
    {
        private readonly IDataRecord record;

        /// <summary>
        /// Initializes a new instance of a DataRecordDecorator.
        /// </summary>
        /// <param name="record">The IDataRecord to decorate.</param>
        public DataRecordDecorator(IDataRecord record)
            : base(record)
        {
            this.record = record;
        }

        /// <summary>
        /// Gets the underlying data record.
        /// </summary>
        public IDataRecord DataRecord
        {
            get { return record; }
        }
    }
}
