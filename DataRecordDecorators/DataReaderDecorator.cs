using System;
using System.Data;

namespace DataRecordDecorators
{
    /// <summary>
    /// Provides additional helper methods to the IDataReader interface.
    /// </summary>
    public sealed class DataReaderDecorator : BaseDataRecordDecorator, IDataReader
    {
        private readonly IDataReader reader;

        /// <summary>
        /// Initializes a new instance of a DataReaderDecorator.
        /// </summary>
        /// <param name="reader">The IDataReader to decorate.</param>
        public DataReaderDecorator(IDataReader reader)
            : base(reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Gets the underlying data reader.
        /// </summary>
        public IDataReader DataReader
        {
            get { return reader; }
        }

        /// <summary>
        /// Closes the reader.
        /// </summary>
        public void Close()
        {
            reader.Close();
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        public int Depth
        {
            get { return reader.Depth; }
        }

        /// <summary>
        /// Returns a DataTable that describes the column metadata of the reader.
        /// </summary>
        /// <returns>A DataTable that describes the column metadata.</returns>
        public DataTable GetSchemaTable()
        {
            return reader.GetSchemaTable();
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        public bool IsClosed
        {
            get { return reader.IsClosed; }
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>True if there are more results; otherwise, false.</returns>
        public bool NextResult()
        {
            return reader.NextResult();
        }

        /// <summary>
        /// Advances the System.Data.IDataReader to the next record.
        /// </summary>
        /// <returns>True if there are more rows; otherwise, false.</returns>
        public bool Read()
        {
            return reader.Read();
        }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        public int RecordsAffected
        {
            get { return reader.RecordsAffected; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
