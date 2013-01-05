# DataRecordExtensions

Extensions to the ADO.NET IDataRecord interface.

## Overview
Even though there are plenty of amazing ORMs out there today, like Entity Framework, a large number of developers are still using the core ADO.NET classes. These include the `DataSet` and `DataTable` classes, as well as the `IDataReader` and `IDataRecord` classes. For those of us still working with the raw power of ADO.NET, DataRecordExtensions is an essential library.

### Feature Summary
Here is a list of the features found in DataRecordExtensions.
* Support operations taking both column index or name.
* Support for nullable types.
* Support for mapping to enumerations.
* Support for automatically converting a column value to specified type.
* Support for returning all row values in an object array.

### DataReaderDecorator and DataRecordDecorator
For those of us still working in .NET 2.0, two decorator classes are provided: `DataReaderDecorator` and `DataRecordDecorator`. Simply pass your DataReader to the constructor and the extension methods will be available to you.

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
For those of us working in newer version of .NET, extension methods are provided for the `IDataRecord` interface. NOTE: `IDataReader` inherits from `IDataRecord`, so it also has access to all the extension methods.

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

## Handling Nulls
The `IDataRecord` classes have a horrible limitation - they don't handle nulls very well. Even `GetString` will throw an error if the value is null, even though `String` is a reference type!!! This can be a surprise the first time around. The solution is to call `IsDBNull` beforehand to make sure the value isn't null.

DataRecordExtensions handles nulls for you automatically whenever you call the `GetNullable*` methods.

    string name = reader.GetNullableString("Name");
    
You can even provide a different default value if you'd like:

    string name = reader.GetNullableString("Name", String.Empty);

### Working with Default Values
When working with primitive types, you might not want to return a nullable (`int?` for instance). If you want to substitute nulls for default values, you can do either:

    DateTime creationDate = reader.GetNullableDate("Created", DateTime.MinValue).Value;
    DateTime creationDate = reader.GetNullableDate("Created") ?? DateTime.MinValue;
    
Personally, I find the second approach slightly more readable.

## Avoid Conversion Errors
Some providers do not make it easy to grab values from an `IDataReader`. For instance, the Oracle data provider is notorious for forcing small NUMBERs to be `byte` or `short` rather than `int`. Things get especially hairy when working with floating point types. There's no risk in increasing precision, so an exception being thrown doesn't really make sense. That's why I created the `GetValue<T>` method. This method accepts the desired type as the generics argument. The method will do its best to convert to the specified type. It can even handle string to int conversion and nullables!
