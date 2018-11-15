$vsixmanifestPath = 'src\CopyFunctionBreakpointName\source.extension.vsixmanifest'
$assemblyInfoPath = 'src\CopyFunctionBreakpointName\Properties\AssemblyInfo.cs'

function XmlPeek(
    [Parameter(Mandatory=$true)] [string] $FilePath,
    [Parameter(Mandatory=$true)] [string] $XPath,
    [HashTable] $NamespaceUrisByPrefix
) {
    $document = [xml](Get-Content $FilePath)
    $namespaceManager = [System.Xml.XmlNamespaceManager]::new($document.NameTable)

    if ($null -ne $NamespaceUrisByPrefix) {
        foreach ($prefix in $NamespaceUrisByPrefix.Keys) {
            $namespaceManager.AddNamespace($prefix, $NamespaceUrisByPrefix[$prefix]);
        }
    }

    return $document.SelectSingleNode($XPath, $namespaceManager).Value
}

function XmlPoke(
    [Parameter(Mandatory=$true)] [string] $FilePath,
    [Parameter(Mandatory=$true)] [string] $XPath,
    [Parameter(Mandatory=$true)] [string] $Value,
    [HashTable] $NamespaceUrisByPrefix
) {
    $document = [System.Xml.XmlDocument]::new()
    $document.PreserveWhitespace = $true
    $document.Load((Resolve-Path $FilePath))

    $namespaceManager = [System.Xml.XmlNamespaceManager]::new($document.NameTable)

    if ($null -ne $NamespaceUrisByPrefix) {
        foreach ($prefix in $NamespaceUrisByPrefix.Keys) {
            $namespaceManager.AddNamespace($prefix, $NamespaceUrisByPrefix[$prefix]);
        }
    }

    $document.SelectSingleNode($XPath, $namespaceManager).Value = $Value
    $document.Save((Resolve-Path $FilePath))
}

function Get-AutomaticCiVersion {
    function Get-VersionPrefix([Parameter(Mandatory=$true)] [string] $Tag) {
        # Start the search at index 6, skipping 1 for the `v` and 5 because no valid semantic version can have a suffix sooner than `N.N.N`.
        $suffixStart = $Tag.IndexOfAny(('-', '+'), 6)

        return [version] $(
            if ($suffixStart -eq -1) {
                $Tag.Substring(1)
            } else {
                $Tag.Substring(1, $suffixStart - 1)
            })
    }

    $currentTags = $(git tag --list v* --points-at head --sort=-v:refname)
    if ($currentTags.Count -gt 0) {
        # Head is tagged, so the tag is the intended CI version for this build.
        return Get-VersionPrefix $currentTags[0]
    }

    $previousTags = $(git tag --list v* --sort=-v:refname)
    if ($previousTags.Count -gt 0) {
        # Head is not tagged, so it would be greater than the most recent tagged version.
        $previousVersion = Get-VersionPrefix $previousTags[0]
        return [version]::new($previousVersion.Major, $previousVersion.Minor, $previousVersion.Build + 1)
    }

    # No release has been tagged, so the initial version should be whatever the source files currently contain.
}

function Update-Version([string] $BuildNumber, [string] $CommitHash, [ScriptBlock] $UpdateBuildNumber) {
    $vsixManifestVersionPath = '/vsx:PackageManifest/vsx:Metadata/vsx:Identity/@Version'
    $vsixNamespaces = @{ vsx = 'http://schemas.microsoft.com/developer/vsx-schema/2011' }

    $extensionVersion = [version](XmlPeek $vsixmanifestPath $vsixManifestVersionPath $vsixNamespaces)

    $automaticCiVersion = Get-AutomaticCiVersion
    if ($extensionVersion -lt $automaticCiVersion) { $extensionVersion = $automaticCiVersion }

    $assemblyProductVersion = [string] $extensionVersion;
    $suffix = @()
    if ($BuildNumber) { $suffix += "build.$BuildNumber" }
    if ($CommitHash) { $suffix += "commit.$CommitHash" }
    if ($suffix) { $assemblyProductVersion += '+' + ($suffix -Join '.') }

    if ($UpdateBuildNumber) { $UpdateBuildNumber.Invoke($assemblyProductVersion) }

    XmlPoke $vsixmanifestPath $vsixManifestVersionPath -Value $extensionVersion $vsixNamespaces

    $assemblyInfo = [System.IO.File]::ReadAllText((Resolve-Path $assemblyInfoPath))

    $assemblyInfo = $assemblyInfo `
        -replace '(?<=\[assembly: AssemblyVersion\(")[^"]*(?="\)\])', $extensionVersion `
        -replace '(?<=\[assembly: AssemblyInformationalVersion\(")[^"]*(?="\)\])', $assemblyProductVersion

    [System.IO.File]::WriteAllText((Resolve-Path $assemblyInfoPath), $assemblyInfo)
}
