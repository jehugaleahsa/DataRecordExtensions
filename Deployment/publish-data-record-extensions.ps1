&dotnet pack "..\DataRecordExtensions\DataRecordExtensions.csproj" --configuration Release --output $PWD

.\NuGet.exe push DataRecordExtensions.*.nupkg -Source https://www.nuget.org/api/v2/package

Remove-Item DataRecordExtensions.*.nupkg