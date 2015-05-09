param($installPath, $toolsPath, $package, $project)

Write-Host "Please, download the SQLite precompiled binary for Windows v3.8.5 from the SQLite.org site (http://www.sqlite.org/2014/sqlite-dll-win32-x86-3080500.zip), extract the sqlite3.dll file, and add it as Copy-Always Content to the root folder of the project."
Write-Host "When referencing the package from a runnable project, such as a Console Application or Windows Application, you should target x86 platform architecture, or Any CPU with Prefer 32-bit option marked."
Write-Host "When referencing the package from a Class Library project, you should verify that the runnable project referencing the Class Library is targeting x86 platform architecture, or Any CPU with Prefer 32-bit option marked."
Write-Host "Targeting x64 platform architecture is not supported."
