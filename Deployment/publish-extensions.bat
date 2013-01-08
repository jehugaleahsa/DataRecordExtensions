nuget pack ../DataRecordExtensions/DataRecordExtensions.csproj -Prop Configuration=Release -Build
nuget push *.nupkg
del *.nupkg