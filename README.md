# DataRecordExtensions

Powerful extensions to the ADO.NET IDataRecord interface.

## Overview
Even though there are plenty of amazing ORMs out there today, like Entity Framework, a large number of developers are still using the core ADO.NET classes. These include the DataSet and DataTable classes, as well as the IDataReader and IDataRecord classes. For those of us still working with the raw power of ADO.NET, DataRecordExtensions is an essential library.

### Feature Summary
Here is a list of the features found in DataRecordExtensions.
* Support for all operations by column index or name.
* Support for nullable types (or a default value).
* Support for mapping to enumerations.
* Support for automatically converting a column value to specified type.
* Support for returning all row values in an object array.

### DataReaderDecorator and DataRecordDecorator
For those of us still working in .NET 2.0, two decorator classes are provided: DataReaderDecorator and DataRecordDecorator. Simply pass your DataReader to the constructor and the extension methods will be available to you.

	using (SqlConnection connection = new SqlConnection(connectionString))
	{
		connection.Open();
		using (SqlCommand command = connection.CreateCommand())
		{
			command.CommandText = "SELECT * FROM TABLE";
			using (IDataReader reader = new DataReaderDecorator(command.ExecuteReader()))
			{
				while (reader.Read())
				{
					// ... process record
				}
			}
		}
	}


### DataRecordExtensions
For those of us working in newer version of .NET, extension methods are provided for the IDataRecord interface. NOTE: IDataReader inherits from IDataRecord.

	using (SqlConnection connection = new SqlConnection(connectionString))
	{
		connection.Open();
		using (SqlCommand command = connection.CreateCommand())
		{
			command.CommandText = "SELECT * FROM TABLE";
			using (SqlDataReader reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					// ... process record
				}
			}
		}
	}

You'll notice in this example that the code is exactly the same. Just remember to put your `using DataRecordExtensions` statement at the top of the file!
