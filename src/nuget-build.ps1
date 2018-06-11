
## delete local nuget packages
get-childItem ConfigWrapper/packages -recurse | remove-item -recurse -force -ErrorAction silentlyContinue
get-childItem ConfigWrapper.Json/packages -recurse | remove-item -recurse -force -ErrorAction silentlyContinue

## restore all nuget packages
nuget restore ConfigWrapper.sln

## clean and build the soluion
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe ConfigWrapper.sln  /t:Clean
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe ConfigWrapper.sln  /t:build /p:Configuration=Release /p:TargetFramework=v4.0

 rm .\ConfigWrapper.*.*.*.nupkg
 
 nuget pack .\ConfigWrapper\ConfigWrapper.nuspec
 nuget pack .\ConfigWrapper.Json\ConfigWrapper.json.nuspec