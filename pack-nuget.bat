copy LICENSE bin
pushd source 
..\..\..\..\NuGet\NuGet.exe pack XDMessaging.csproj -Prop Configuration=SignedRelease -o ..\bin
popd