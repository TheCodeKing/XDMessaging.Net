copy LICENSE bin
pushd source 
..\..\..\..\NuGet\NuGet.exe pack XDMessaging.Core\XDMessaging.Core.csproj -Prop Configuration=SignedRelease -o ..\bin
popd