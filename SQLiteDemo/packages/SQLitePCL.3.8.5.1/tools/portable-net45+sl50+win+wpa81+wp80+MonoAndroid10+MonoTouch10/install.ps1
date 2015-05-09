param($installPath, $toolsPath, $package, $project)

Write-Host "If your PCL Project is part of another NuGet Package, you should add this NuGet package as a dependency."
Write-Host "If your PCL Project is referenced from other platform specific projects, you should install this NuGet package on each of these projects."