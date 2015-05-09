param($installPath, $toolsPath, $package, $project)

$sqliteReference = $project.Object.References.Find("SQLite.WinRT, version=3.8.5")

if ($sqliteReference -eq $null) {
    Write-Host "Unable to find a reference to the extension SDK SQLite for Windows Runtime."
    Write-Host "Verify that the reference to the extension SDK SQLite for Windows Runtime has already been removed."
} else {
    $sqliteReference.Remove()
    Write-Host "Successfully removed the reference to the extension SDK SQLite for Windows Runtime."
}
