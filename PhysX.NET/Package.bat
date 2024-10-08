C:

md Package
cd Package
rd Working /S /Q
md Working
cd Working
md "PhysX.NET 5.1.0 for NVIDIA PhysX 4.1.2"
cd "PhysX.NET 5.1.0 for NVIDIA PhysX 4.1.2"
md "Assemblies"
md "Assemblies\x64"
md "Assemblies\x64\Debug"
md "Assemblies\x64\Release"
cd ..\..\..

msbuild PhysX.NET.sln /p:Configuration=Debug;Platform=x64 /t:Clean,Build
msbuild PhysX.NET.sln /p:Configuration=Release;Platform=x64 /t:Clean,Build

Rem Samples
md "Package\Working\PhysX.NET 5.1.0 for NVIDIA PhysX 4.1.2\Samples"
robocopy Samples "Package\Working\PhysX.NET 5.1.0 for NVIDIA PhysX 4.1.2\Samples" /E /XF *.suo *.vssscc *.cachefile *.user *.vspscc *.pdb *.vshost.exe* /XD obj Release
md "Package\Working\PhysX.NET 5.1.0 for NVIDIA PhysX 4.1.2\Assemblies"
robocopy Assemblies "Package\Working\PhysX.NET 5.1.0 for NVIDIA PhysX 4.1.2\Assemblies" /E

xcopy "PhysX.NET\bin\x64\Debug\PhysX.NET.dll" "Package\Working\PhysX.NET 5.1.0 for NVIDIA PhysX 4.1.2\Assemblies\x64\Debug"
xcopy "PhysX.NET\bin\x64\Debug\PhysX.NET.pdb" "Package\Working\PhysX.NET 5.1.0 for NVIDIA PhysX 4.1.2\Assemblies\x64\Debug"
xcopy "PhysX.NET\bin\x64\Release\PhysX.NET.dll" "Package\Working\PhysX.NET 5.1.0 for NVIDIA PhysX 4.1.2\Assemblies\x64\Release"

cd "Package\Working"

attrib * -R /S /D

..\..\..\Tools\7-Zip\7z.exe a "PhysX.NET 5.0.1 for NVIDIA PhysX 4.1.2.zip" .

cd ..\..
