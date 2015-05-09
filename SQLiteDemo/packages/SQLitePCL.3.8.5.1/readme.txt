Portable Class Library for SQLite. v3.8.5.1

Supports .NET Framework 4.5, Windows Store, Windows Phone 8 applications, Xamarin Android, Xamarin iOS, and PCL Projects targeting this platforms. Even though we include SilverLight 5 target on the PCL, this is only to enable certain fixed profiles, and is not supported by our current package (there is no built in support for SQLite).

ADDITIONAL INSTRUCTIONS

.NET Framework 4.5
Please, download the SQLite precompiled binary for Windows v3.8.5 from the SQLite.org site (http://www.sqlite.org/2014/sqlite-dll-win32-x86-3080500.zip), extract the sqlite3.dll file, and add it as Copy-Always Content to the root folder of the project.
When referencing the package from a runnable project, such as a Console Application or Windows Application, you should target x86 platform architecture, or Any CPU with Prefer 32-bit option marked.
When referencing the package from a Class Library project, you should verify that the runnable project referencing the Class Library is targeting x86 platform architecture, or Any CPU with Prefer 32-bit option marked.
Targeting x64 platform architecture is not supported.

Windows Store 8.0
Please, verify that the extension SDK SQLite for Windows Runtime v3.8.5, from the SQLite.org site (http://www.sqlite.org/2014/sqlite-winrt-3080500.vsix), has been properly installed.

Windows Store 8.1
Please, verify that the extension SDK SQLite for Windows Runtime (Windows 8.1) v3.8.5, from the SQLite.org site (http://www.sqlite.org/2014/sqlite-winrt81-3080500.vsix), has been properly installed.

Windows Phone 8
Please, verify that the extension SDK SQLite for Windows Phone v3.8.5, from the SQLite.org site (http://www.sqlite.org/2014/sqlite-wp80-winrt-3080500.vsix), has been properly installed.

Windows Phone 8.1
Please, verify that the extension SDK SQLite for Windows Phone 8.1 v3.8.5, from the SQLite.org site (http://www.sqlite.org/2014/sqlite-wp81-winrt-3080500.vsix), has been properly installed.

Xamarin Android
SQLite is natively supported on Android.

Xamarin iOS
SQLite is natively supported on iOS.
You should make an explicit call to SQLitePCL.CurrentPlatform.Init() static method from your Xamarin iOS application, in order to have the platform specific SQLitePCL.Ext.dll library loaded into the package.

PCL Project
If your PCL Project is part of another NuGet Package, you should add this NuGet package as a dependency.
If your PCL Project is referenced from other platform specific host projects, you should install this NuGet package on each of these projects.
