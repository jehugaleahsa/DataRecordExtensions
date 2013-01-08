nuget pack ../DataRecordDecorators/DataRecordDecorators.csproj -Prop Configuration=Release -Build
nuget push *.nupkg
del *.nupkg