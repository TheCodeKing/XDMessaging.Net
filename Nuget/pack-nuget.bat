pushd ..\
copy LICENSE bin
for /D %%D in (%SYSTEMROOT%\Microsoft.NET\Framework\v4*) do set msbuild=%%D\MSBuild.exe
%msbuild% source\XDMessaging.sln -P:Configuration=SignedRelease
popd
..\..\..\..\NuGet\NuGet.exe pack XDMessaging.nuspec -Prop Configuration=SignedRelease -o ..\Bin
