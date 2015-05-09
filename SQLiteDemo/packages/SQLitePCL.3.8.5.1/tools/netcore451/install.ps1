param($installPath, $toolsPath, $package, $project)

$sqliteReference = $project.Object.References.AddSDK("SQLite for Windows Runtime (Windows 8.1)", "SQLite.WinRT81, version=3.8.5")

Write-Host "Successfully added a reference to the extension SDK SQLite for Windows Runtime (Windows 8.1)."
Write-Host "Please, verify that the extension SDK SQLite for Windows Runtime (Windows 8.1) v3.8.5, from the SQLite.org site (http://www.sqlite.org/2014/sqlite-winrt81-3080500.vsix), has been properly installed."
