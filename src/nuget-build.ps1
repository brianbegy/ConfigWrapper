
## delete local nuget packages
get-childItem ConfigWrapper/packages -recurse | remove-item -recurse -force -ErrorAction silentlyContinue
get-childItem ConfigWrapper.Json/packages -recurse | remove-item -recurse -force -ErrorAction silentlyContinue

## restore all nuget packages
nuget restore ConfigWrapper.sln

rm .\ConfigWrapper.*.*.*.nupkg

## clean and build the soluion
msbuild ConfigWrapper.sln  /t:Clean
msbuild ConfigWrapper/ConfigWrapper.csproj  /p:Configuration=Release /p:TargetFramework=v4.0 
msbuild ConfigWrapper/ConfigWrapperJson.csproj  /p:Configuration=Release /p:TargetFramework=v4.0 
 
nuget pack .\ConfigWrapper\ConfigWrapper.nuspec
nuget pack .\ConfigWrapper.Json\ConfigWrapper.json.nuspec