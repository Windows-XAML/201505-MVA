param($installPath, $toolsPath, $package, $project)

$sqliteReference = $project.Object.References.Find("SQLite.WinRT81, version=3.8.5")

if ($sqliteReference -eq $null) {
    Write-Host "Unable to find a reference to the extension SDK SQLite for Windows Runtime (Windows 8.1)."
    Write-Host "Verify that the reference to the extension SDK SQLite for Windows Runtime (Windows 8.1) has already been removed."
} else {
    $sqliteReference.Remove()
    Write-Host "Successfully removed the reference to the extension SDK SQLite for Windows Runtime (Windows 8.1)."
}
