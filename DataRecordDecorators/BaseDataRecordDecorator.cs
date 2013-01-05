using System;
using System.Data;
using System.Globalization;

namespace DataRecordDecorators
{
    /// <summary>
    /// Represents a function that maps the given value to another.
    /// </summary>
    /// <typeparam name="TSource">The type of the source item.</typeparam>
    /// <typeparam name="TMapped">The type of the item returned.</typeparam>
    /// <param name="value">The value to map to the mapped type.</param>
    /// <returns>The mapped value.</returns>
    public delegate TMapped Mapper<TSource, TMapped>(TSource value);

    /// <summary>
    /// Provides additional helper methods to the IDataRecord interface.
    /// </summary>
    public abstract class BaseDataRecordDecorator : IDataRecord
    {
        private readonly IDataRecord record;

        /// <summary>
        /// Initializes a new instance of a DataRecordDecorator.
        /// </summary>
        /// <param name="record">The IDataRecord to decorate.</param>
        protected BaseDataRecordDecorator(IDataRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }
            this.record = record;
        }

        #region FieldCount

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public int FieldCount
        {
            get { return record.FieldCount; }
        }

        #endregion

        #region Item

        /// <summary>
        /// Gets the value located at the specific index.
        /// </summary>
        /// <param name="i">The index of the column to get.</param>
        /// <returns>The value located at the specific index.</returns>
        public object this[int i]
        {
            get { return record[i]; }
        }

        /// <summary>
        /// Gets the value with the given name.
        /// </summary>
        /// <param name="name">The name of the column to get.</param>
        /// <returns>The value with the given name.</returns>
        public object this[string name]
        {
            get { return record[name]; }
        }

        #endregion

        #region GetBoolean

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="i">The index of the column.</param>
        /// <returns>The value of the column.</returns>
        public bool GetBoolean(int i)
        {
            return record.GetBoolean(i);
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column.</returns>
        public bool GetBoolean(string name)
        {
            return get(record.GetBoolean, name);
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean -or- null if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public bool? GetNullableBoolean(int i)
        {
            return getNullable(record.GetBoolean, i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean -or- null if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public bool? GetNullableBoolean(string name)
        {
            return getNullable(record.GetBoolean, name, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public bool? GetNullableBoolean(int i, bool? defaultValue)
        {
            return getNullable(record.GetBoolean, i, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public bool? GetNullableBoolean(string name, bool? defaultValue)
        {
            return getNullable(record.GetBoolean, name, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a Boolean and stores it in the out parameter.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a Boolean; otherwise, false.</returns>
        public bool TryGetBoolean(int i, out bool value)
        {
            return tryGet(i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a Boolean and stores it in the out parameter.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a Boolean; otherwise, false.</returns>
        public bool TryGetBoolean(string name, out bool value)
        {
            return tryGet(name, out value);
        }

        #endregion

        #region GetEnum

        /// <summary>
        /// Maps the value of the column to the specified enumeration value.
        /// </summary>
        /// <typeparam name="T">The type of the column.</typeparam>
        /// <typeparam name="TEnum">The type of the value to map to.</typeparam>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column mapped to the enumeration value.</returns>
        /// <remarks>This method attempts to generate the enumeration by its name (case-insensitive) or numeric value.</remarks>
        public TEnum GetEnum<T, TEnum>(int i)
            where TEnum : struct
        {
            return getEnum<T, TEnum>(i, getByRepresentation<T, TEnum>);
        }

        /// <summary>
        /// Maps the value of the column to the specified enumeration value.
        /// </summary>
        /// <typeparam name="T">The type of the column.</typeparam>
        /// <typeparam name="TEnum">The type of the value to map to.</typeparam>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column mapped to the enumeration value.</returns>
        /// <remarks>This method attempts to generate the enumeration by its name (case-insensitive) or numeric value.</remarks>
        public TEnum GetEnum<T, TEnum>(string name)
            where TEnum : struct
        {
            int ordinal = record.GetOrdinal(name);
            return getEnum<T, TEnum>(ordinal, getByRepresentation<T, TEnum>);
        }

        private static TEnum getByRepresentation<T, TEnum>(T value)
            where TEnum : struct
        {
            if (value is String)
            {
                return (TEnum)Enum.Parse(typeof(TEnum), (string)(object)value, true);
            }
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        /// <summary>
        /// Maps the value of the column to an enumeration value.
        /// </summary>
        /// <typeparam name="T">The type of the column.</typeparam>
        /// <typeparam name="TEnum">The type of value to map to.</typeparam>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="mapper">A method that maps from the column's type to the desired type.</param>
        /// <returns>The value of the column mapped to the enumeration value.</returns>
        public TEnum GetEnum<T, TEnum>(int i, Mapper<T, TEnum> mapper)
            where TEnum : struct
        {
            return getEnum<T, TEnum>(i, mapper);
        }

        /// <summary>
        /// Maps the value of the column to an enumeration value.
        /// </summary>
        /// <typeparam name="T">The type of the column.</typeparam>
        /// <typeparam name="TEnum">The type of value to map to.</typeparam>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="mapper">A method that maps from the column's type to the desired type.</param>
        /// <returns>The value of the column mapped to the enumeration value.</returns>
        public TEnum GetEnum<T, TEnum>(string name, Mapper<T, TEnum> mapper)
            where TEnum : struct
        {
            int ordinal = record.GetOrdinal(name);
            return getEnum(ordinal, mapper);
        }

        private TEnum getEnum<T, TEnum>(int i, Mapper<T, TEnum> mapper)
            where TEnum : struct
        {
            T value = getValue<T>(i, CultureInfo.CurrentCulture);
            return mapper(value);
        }

        #endregion

        #region GetByte

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="i">The index of the column.</param>
        /// <returns>The 8-bit unsigned integer value for the specified column.</returns>
        public byte GetByte(int i)
        {
            return record.GetByte(i);
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The 8-bit unsigned integer value of the specified column.</returns>
        public byte GetByte(string name)
        {
            return get(record.GetByte, name);
        }

        /// <summary>
        /// Gets the value of the specified column as a byte -or- null if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public byte? GetNullableByte(int i)
        {
            return getNullable(record.GetByte, i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a byte -or- null if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public byte? GetNullableByte(string name)
        {
            return getNullable(record.GetByte, name, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a byte -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public byte? GetNullableByte(int i, byte? defaultValue)
        {
            return getNullable(record.GetByte, i, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a byte -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public byte? GetNullableByte(string name, byte? defaultValue)
        {
            return getNullable(record.GetByte, name, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a byte and stores it in the out parameter.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a byte; otherwise, false.</returns>
        public bool TryGetByte(int i, out byte value)
        {
            return tryGet(i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a byte and stores it in the out parameter.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a byte; otherwise, false.</returns>
        public bool TryGetByte(string name, out byte value)
        {
            return tryGet(name, out value);
        }

        #endregion

        #region GetBytes

        /// <summary>
        /// Reads a stream of bytes from the specified column offset into the buffer
        /// as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <param name="fieldOffset">The index within the field from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for buffer to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return record.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column offset into the buffer
        /// as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="fieldOffset">The index within the field from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for buffer to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        public long GetBytes(string name, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            int ordinal = record.GetOrdinal(name);
            return record.GetBytes(ordinal, fieldOffset, buffer, bufferoffset, length);
        }

        #endregion

        #region GetChar

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <returns>The character value of the specified column.</returns>
        public char GetChar(int i)
        {
            return record.GetChar(i);
        }

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The character value of the specified column.</returns>
        public char GetChar(string name)
        {
            return get(record.GetChar, name);
        }

        /// <summary>
        /// Gets the value of the specified column as a char -or- null if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public char? GetNullableChar(int i)
        {
            return getNullable(record.GetChar, i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a char -or- null if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public char? GetNullableChar(string name)
        {
            return getNullable(record.GetChar, name, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a char -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public char? GetNullableChar(int i, char? defaultValue)
        {
            return getNullable(record.GetChar, i, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a char -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public char? GetNullableChar(string name, char? defaultValue)
        {
            return getNullable(record.GetChar, name, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a char and stores it in the out parameter.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a char; otherwise, false.</returns>
        public bool TryGetChar(int i, out char value)
        {
            return tryGet(i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a char and stores it in the out parameter.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a char; otherwise, false.</returns>
        public bool TryGetChar(string name, out char value)
        {
            return tryGet(name, out value);
        }

        #endregion

        #region GetChars

        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer
        /// as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <param name="fieldoffset">The index within the row from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for buffer to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The actual number of characters read.</returns>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return record.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer
        /// as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="fieldoffset">The index within the row from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for buffer to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The actual number of characters read.</returns>
        public long GetChars(string name, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            int ordinal = record.GetOrdinal(name);
            return record.GetChars(ordinal, fieldoffset, buffer, bufferoffset, length);
        }

        #endregion

        #region GetData

        /// <summary>
        /// Returns an System.Data.IDataReader for the specified column name.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <returns>An System.Data.IDataReader.</returns>
        public IDataReader GetData(int i)
        {
            return record.GetData(i);
        }

        /// <summary>
        /// Returns an System.Data.IDataReader for the specified column name.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>An System.Data.IDataReader.</returns>
        public IDataReader GetData(string name)
        {
            return get(record.GetData, name);
        }

        #endregion

        #region GetDataTypeName

        /// <summary>
        /// Gets the data type information for the specified field.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <returns>The data type information for the specified field.</returns>
        public string GetDataTypeName(int i)
        {
            return record.GetDataTypeName(i);
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The data type information for the specified field.</returns>
        public string GetDataTypeName(string name)
        {
            return get(record.GetDataTypeName, name);
        }

        #endregion

        #region GetDateTime

        /// <summary>
        /// Gets the DateTime value of the specified column.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <returns>The DateTime value of the specified column.</returns>
        public DateTime GetDateTime(int i)
        {
            return record.GetDateTime(i);
        }

        /// <summary>
        /// Gets the DateTime value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The DateTime value of the specified column.</returns>
        public DateTime GetDateTime(string name)
        {
            return get(record.GetDateTime, name);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- null if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public DateTime? GetNullableDateTime(int i)
        {
            return getNullable(record.GetDateTime, i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- null if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public DateTime? GetNullableDateTime(string name)
        {
            return getNullable(record.GetDateTime, name, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public DateTime? GetNullableDateTime(int i, DateTime? defaultValue)
        {
            return getNullable(record.GetDateTime, i, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public DateTime? GetNullableDateTime(string name, DateTime? defaultValue)
        {
            return getNullable(record.GetDateTime, name, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a DateTime and stores it in the out parameter.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a DateTime; otherwise, false.</returns>
        public bool TryGetDateTime(int i, out DateTime value)
        {
            return tryGet(i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a DateTime and stores it in the out parameter.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a DateTime; otherwise, false.</returns>
        public bool TryGetDateTime(string name, out DateTime value)
        {
            return tryGet(name, out value);
        }

        #endregion

        #region GetDecimal

        /// <summary>
        /// Gets the decimal value of the specified column.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <returns>The decimal value of the specified column.</returns>
        public decimal GetDecimal(int i)
        {
            return record.GetDecimal(i);
        }

        /// <summary>
        /// Gets the decimal value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The decimal value of the specified column.</returns>
        public decimal GetDecimal(string name)
        {
            return get(record.GetDecimal, name);
        }

        /// <summary>
        /// Gets the value of the specified column as a decimal -or- null if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public decimal? GetNullableDecimal(int i)
        {
            return getNullable(record.GetDecimal, i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a decimal -or- null if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public decimal? GetNullableDecimal(string name)
        {
            return getNullable(record.GetDecimal, name, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a decimal -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public decimal? GetNullableDecimal(int i, decimal? defaultValue)
        {
            return getNullable(record.GetDecimal, i, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a decimal -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public decimal? GetNullableDecimal(string name, decimal? defaultValue)
        {
            return getNullable(record.GetDecimal, name, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a decimal and stores it in the out parameter.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a decimal; otherwise, false.</returns>
        public bool TryGetDecimal(int i, out decimal value)
        {
            return tryGet(i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a decimal and stores it in the out parameter.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a decimal; otherwise, false.</returns>
        public bool TryGetDecimal(string name, out decimal value)
        {
            return tryGet(name, out value);
        }

        #endregion

        #region GetDouble

        /// <summary>
        /// Gets the double value of the specified column.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <returns>The double value of the specified column.</returns>
        public double GetDouble(int i)
        {
            return record.GetDouble(i);
        }

        /// <summary>
        /// Gets the double value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The double value of the specified column.</returns>
        public double GetDouble(string name)
        {
            return get(record.GetDouble, name);
        }

        /// <summary>
        /// Gets the value of the specified column as a double -or- null if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public double? GetNullableDouble(int i)
        {
            return getNullable(record.GetDouble, i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a double -or- null if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public double? GetNullableDouble(string name)
        {
            return getNullable(record.GetDouble, name, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a double -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public double? GetNullableDouble(int i, double? defaultValue)
        {
            return getNullable(record.GetDouble, i, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a double -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public double? GetNullableDouble(string name, double? defaultValue)
        {
            return getNullable(record.GetDouble, name, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a double and stores it in the out parameter.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a double; otherwise, false.</returns>
        public bool TryGetDouble(int i, out double value)
        {
            return tryGet(i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a double and stores it in the out parameter.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a double; otherwise, false.</returns>
        public bool TryGetDouble(string name, out double value)
        {
            return tryGet(name, out value);
        }

        #endregion

        #region GetFieldType

        /// <summary>
        /// Gets the System.Type information corresponding to the type of System.Object
        /// that would be returned from System.Data.IDataRecord.GetValue(System.Int32).
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The System.Type information corresponding to the type of System.Object that
        /// would be returned from System.Data.IDataRecord.GetValue(System.Int32).
        /// </returns>
        public Type GetFieldType(int i)
        {
            return record.GetFieldType(i);
        }

        /// <summary>
        /// Gets the System.Type information corresponding to the type of System.Object
        /// that would be returned from System.Data.IDataRecord.GetValue(System.Int32).
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>
        /// The System.Type information corresponding to the type of System.Object that
        /// would be returned from System.Data.IDataRecord.GetValue(System.Int32).
        /// </returns>
        public Type GetFieldType(string name)
        {
            return get(record.GetFieldType, name);
        }

        #endregion

        #region GetFloat

        /// <summary>
        /// Gets the float value of the specified column.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <returns>The float value of the specified column.</returns>
        public float GetFloat(int i)
        {
            return record.GetFloat(i);
        }

        /// <summary>
        /// Gets the float value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The float value of the specified column.</returns>
        public float GetFloat(string name)
        {
            return get(record.GetFloat, name);
        }

        /// <summary>
        /// Gets the value of the specified column as a float -or- null if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public float? GetNullableFloat(int i)
        {
            return getNullable(record.GetFloat, i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a float -or- null if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public float? GetNullableFloat(string name)
        {
            return getNullable(record.GetFloat, name, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a float -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public float? GetNullableFloat(int i, float? defaultValue)
        {
            return getNullable(record.GetFloat, i, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a float -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public float? GetNullableFloat(string name, float? defaultValue)
        {
            return getNullable(record.GetFloat, name, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a float and stores it in the out parameter.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a float; otherwise, false.</returns>
        public bool TryGetFloat(int i, out float value)
        {
            return tryGet(i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a float and stores it in the out parameter.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a float; otherwise, false.</returns>
        public bool TryGetFloat(string name, out float value)
        {
            return tryGet(name, out value);
        }

        #endregion

        #region GetGuid

        /// <summary>
        /// Gets the Guid value of the specified column.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <returns>The Guid value of the specified column.</returns>
        public Guid GetGuid(int i)
        {
            return record.GetGuid(i);
        }

        /// <summary>
        /// Gets the Guid value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The Guid value of the specified column.</returns>
        public Guid GetGuid(string name)
        {
            return get(record.GetGuid, name);
        }

        /// <summary>
        /// Gets the value of the specified column as a Guid -or- null if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public Guid? GetNullableGuid(int i)
        {
            return getNullable(record.GetGuid, i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a Guid -or- null if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public Guid? GetNullableGuid(string name)
        {
            return getNullable(record.GetGuid, name, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a Guid -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public Guid? GetNullableGuid(int i, Guid? defaultValue)
        {
            return getNullable(record.GetGuid, i, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a Guid -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public Guid? GetNullableGuid(string name, Guid? defaultValue)
        {
            return getNullable(record.GetGuid, name, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a Guid and stores it in the out parameter.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a Guid; otherwise, false.</returns>
        public bool TryGetGuid(int i, out Guid value)
        {
            return tryGet(i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a Guid and stores it in the out parameter.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a Guid; otherwise, false.</returns>
        public bool TryGetGuid(string name, out Guid value)
        {
            return tryGet(name, out value);
        }

        #endregion

        #region GetInt16

        /// <summary>
        /// Gets the short value of the specified column.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <returns>The short value of the specified column.</returns>
        public short GetInt16(int i)
        {
            return record.GetInt16(i);
        }

        /// <summary>
        /// Gets the short value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The short value of the specified column.</returns>
        public short GetInt16(string name)
        {
            return get(record.GetInt16, name);
        }

        /// <summary>
        /// Gets the value of the specified column as a short -or- null if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public short? GetNullableInt16(int i)
        {
            return getNullable(record.GetInt16, i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a short -or- null if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public short? GetNullableInt16(string name)
        {
            return getNullable(record.GetInt16, name, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a short -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public short? GetNullableInt16(int i, short? defaultValue)
        {
            return getNullable(record.GetInt16, i, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a short -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public short? GetNullableInt16(string name, short? defaultValue)
        {
            return getNullable(record.GetInt16, name, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a short and stores it in the out parameter.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a short; otherwise, false.</returns>
        public bool TryGetInt16(int i, out short value)
        {
            return tryGet(i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a short and stores it in the out parameter.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a short; otherwise, false.</returns>
        public bool TryGetInt16(string name, out short value)
        {
            return tryGet(name, out value);
        }

        #endregion

        #region GetInt32

        /// <summary>
        /// Gets the int value of the specified column.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <returns>The int value of the specified column.</returns>
        public int GetInt32(int i)
        {
            return record.GetInt32(i);
        }

        /// <summary>
        /// Gets the int value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The int value of the specified column.</returns>
        public int GetInt32(string name)
        {
            return get(record.GetInt32, name);
        }

        /// <summary>
        /// Gets the value of the specified column as a int -or- null if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public int? GetNullableInt32(int i)
        {
            return getNullable(record.GetInt32, i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a int -or- null if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public int? GetNullableInt32(string name)
        {
            return getNullable(record.GetInt32, name, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a int -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public int? GetNullableInt32(int i, int? defaultValue)
        {
            return getNullable(record.GetInt32, i, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a int -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public int? GetNullableInt32(string name, int? defaultValue)
        {
            return getNullable(record.GetInt32, name, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a int and stores it in the out parameter.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a int; otherwise, false.</returns>
        public bool TryGetInt32(int i, out int value)
        {
            return tryGet(i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a int and stores it in the out parameter.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a int; otherwise, false.</returns>
        public bool TryGetInt32(string name, out int value)
        {
            return tryGet(name, out value);
        }

        #endregion

        #region GetInt64

        /// <summary>
        /// Gets the long value of the specified column.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <returns>The int value of the specified column.</returns>
        public long GetInt64(int i)
        {
            return record.GetInt64(i);
        }

        /// <summary>
        /// Gets the long value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The int value of the specified column.</returns>
        public long GetInt64(string name)
        {
            return get(record.GetInt64, name);
        }

        /// <summary>
        /// Gets the value of the specified column as a long -or- null if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public long? GetNullableInt64(int i)
        {
            return getNullable(record.GetInt64, i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a long -or- null if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public long? GetNullableInt64(string name)
        {
            return getNullable(record.GetInt64, name, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a long -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public long? GetNullableInt64(int i, long? defaultValue)
        {
            return getNullable(record.GetInt64, i, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a long -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public long? GetNullableInt64(string name, long? defaultValue)
        {
            return getNullable(record.GetInt64, name, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a long and stores it in the out parameter.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a long; otherwise, false.</returns>
        public bool TryGetInt64(int i, out long value)
        {
            return tryGet(i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a long and stores it in the out parameter.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a long; otherwise, false.</returns>
        public bool TryGetInt64(string name, out long value)
        {
            return tryGet(name, out value);
        }

        #endregion

        #region GetString

        /// <summary>
        /// Gets the string value of the specified column.
        /// </summary>
        /// <param name="i">The index of the column to find.</param>
        /// <returns>The int value of the specified column.</returns>
        public string GetString(int i)
        {
            return record.GetString(i);
        }

        /// <summary>
        /// Gets the string value of the specified column.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The int value of the specified column.</returns>
        public string GetString(string name)
        {
            return get(record.GetString, name);
        }

        /// <summary>
        /// Gets the value of the specified column as a string -or- null if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public string GetNullableString(int i)
        {
            return getNullableString(i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a string -or- null if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public string GetNullableString(string name)
        {
            int ordinal = record.GetOrdinal(name);
            return getNullableString(ordinal, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a string -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public string GetNullableString(int i, string defaultValue)
        {
            return getNullableString(i, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a string -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public string GetNullableString(string name, string defaultValue)
        {
            int ordinal = record.GetOrdinal(name);
            return getNullableString(ordinal, defaultValue);
        }

        private string getNullableString(int i, string defaultValue)
        {
            if (record.IsDBNull(i))
            {
                return defaultValue;
            }
            return record.GetString(i);
        }

        /// <summary>
        /// Tries to get the value of the column as a string and stores it in the out parameter.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a string; otherwise, false.</returns>
        public bool TryGetString(int i, out string value)
        {
            return tryGet(i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a string and stores it in the out parameter.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a string; otherwise, false.</returns>
        public bool TryGetString(string name, out string value)
        {
            return tryGet(name, out value);
        }

        #endregion

        #region GetValue

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The System.Object which will contain the field value upon return.</returns>
        public object GetValue(int i)
        {
            return record.GetValue(i);
        }

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>The System.Object which will contain the field value upon return.</returns>
        public object GetValue(string name)
        {
            return get(record.GetValue, name);
        }

        /// <summary>
        /// Returns the value of the specified field.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The value.</returns>
        public T GetValue<T>(int i)
        {
            return getValue<T>(i, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns the value of the specified field.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>The value.</returns>
        public T GetValue<T>(string name)
        {
            int ordinal = record.GetOrdinal(name);
            return getValue<T>(ordinal, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns the value of the specified field.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="i">The index of the field to find.</param>
        /// <param name="provider">A format provider for converting to the desired type.</param>
        /// <returns>The value.</returns>
        public T GetValue<T>(int i, IFormatProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            return getValue<T>(i, provider);
        }

        /// <summary>
        /// Returns the value of the specified field.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="name">The name of the field to find.</param>
        /// <param name="provider">A format provider for converting to the desired type.</param>
        /// <returns>The value.</returns>
        public T GetValue<T>(string name, IFormatProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            int ordinal = record.GetOrdinal(name);
            return getValue<T>(ordinal, provider);
        }

        private T getValue<T>(int i, IFormatProvider provider)
        {
            Type type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            if (record.IsDBNull(i))
            {
                if (typeof(T).IsValueType && typeof(T) == type)
                {
                    throw new InvalidCastException();
                }
                return default(T);
            }
            object value = record.GetValue(i);
            if (value is IConvertible)
            {
                value = System.Convert.ChangeType(value, type, provider);
            }
            return (T)value;
        }

        #endregion

        #region GetValues

        /// <summary>
        /// Populates the given array with the field values.
        /// </summary>
        /// <param name="values">The array to populate.</param>
        /// <returns>The number of values stored in the array.</returns>
        public int GetValues(object[] values)
        {
            return record.GetValues(values);
        }

        /// <summary>
        /// Creates an array of objects with the column values of the current record.
        /// </summary>
        /// <returns>An array of objects with the column values of the current record.</returns>
        public object[] GetValues()
        {
            object[] values = new object[record.FieldCount];
            record.GetValues(values);
            return values;
        }

        #endregion

        #region IsDBNull

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>true if the specified field is set to null; otherwise, false.</returns>
        public bool IsDBNull(int i)
        {
            return record.IsDBNull(i);
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>true if the specified field is set to null; otherwise, false.</returns>
        public bool IsDBNull(string name)
        {
            return get(record.IsDBNull, name);
        }

        #endregion

        #region GetName

        /// <summary>
        /// Gets the name of the field at the given index.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The name of the field at the given index.</returns>
        public string GetName(int i)
        {
            return record.GetName(i);
        }

        #endregion

        #region GetOrdinal

        /// <summary>
        /// Gets the index of the column with the given name.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The index of the column with the given name.</returns>
        public int GetOrdinal(string name)
        {
            return record.GetOrdinal(name);
        }

        #endregion

        private delegate T Getter<T>(int ordinal);

        private T get<T>(Getter<T> getter, string name)
        {
            int index = record.GetOrdinal(name);
            return getter(index);
        }

        private T? getNullable<T>(Getter<T> getter, int index, T? defaultValue)
            where T : struct
        {
            if (record.IsDBNull(index))
            {
                return defaultValue;
            }
            else
            {
                return getter(index);
            }
        }

        private T? getNullable<T>(Getter<T> getter, string name, T? defaultValue)
            where T : struct
        {
            int index = record.GetOrdinal(name);
            return getNullable<T>(getter, index, defaultValue);
        }

        private bool tryGet<T>(int i, out T value)
        {
            if (record.IsDBNull(i))
            {
                value = default(T);
                return false;
            }
            object result = record.GetValue(i);
            if (result is T)
            {
                value = (T)result;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        private bool tryGet<T>(string name, out T value)
        {
            int index = record.GetOrdinal(name);
            return tryGet<T>(index, out value);
        }
    }
}
