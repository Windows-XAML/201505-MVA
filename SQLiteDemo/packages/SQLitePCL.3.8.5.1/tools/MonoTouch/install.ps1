param($installPath, $toolsPath, $package, $project)

Write-Host "SQLite is natively supported on iOS."
Write-Host "You should make an explicit call to SQLitePCL.CurrentPlatform.Init() static method from your Xamarin iOS application, in order to have the platform specific SQLitePCL.Ext.dll library loaded into the package."