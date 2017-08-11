using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyCompany("TheCodeKing")]
[assembly: AssemblyCopyright("Copyright © 2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("5.0.6")]
[assembly: AssemblyFileVersion("5.0.6")]

#if SIGNED
[assembly: InternalsVisibleTo("XDMessaging.Test, PublicKey=0024000004800000940000000602000000240000525341310004000001000100f1061ea0d5d0eb099e3796f495bcfa7b50b9c48085233e18013cadfe86cb8a841547d0a26b0f1ddea1c5759d510032e031caf0925cac3346955127c2e52304bd825186bfb39b7048030549c006fa06070ca8b708c83a77d41a16cff0e43d7b72d2c61739766c048bec598f6ef7d9b0ad19aeaac8854133c0603a740a5f67e9b8")]
[assembly: AssemblyKeyFile(@"..\..\..\..\..\thecodeking.snk")]
#else
[assembly: InternalsVisibleTo("XDMessaging.Test")]
#endif