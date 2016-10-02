pushd ..\
set msbuild="C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe"
%msbuild% src\XDMessaging.sln -P:Configuration=SignedRelease
NuGet pack Nuget/XDMessaging.Lite.nuspec -Prop Configuration=SignedRelease -o Nuget
NuGet pack Nuget/XDMessaging.nuspec -Prop Configuration=SignedRelease -o Nuget
popd