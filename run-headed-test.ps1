$ErrorActionPreference = "Stop"

$envFile = Join-Path $PSScriptRoot ".env"
if (-not (Test-Path -LiteralPath $envFile -PathType Leaf)) {
    throw "Missing .env file. Copy .env.example to .env and set the test credentials."
}

$envValues = @{}
foreach ($line in Get-Content -LiteralPath $envFile) {
    $trimmedLine = $line.Trim()
    if ([string]::IsNullOrWhiteSpace($trimmedLine) -or $trimmedLine.StartsWith("#")) {
        continue
    }

    if ($trimmedLine -notmatch '^([^=]+)=(.*)$') {
        throw "Invalid .env entry. Expected KEY=VALUE."
    }

    $envValues[$Matches[1].Trim()] = $Matches[2].Trim()
}

$usernameKey = "Configuration__Username"
$passwordKey = "Configuration__Password"
foreach ($key in @($usernameKey, $passwordKey)) {
    if (-not $envValues.ContainsKey($key) -or [string]::IsNullOrWhiteSpace($envValues[$key])) {
        throw "Missing required value '$key' in .env."
    }
}

$originalUsername = [Environment]::GetEnvironmentVariable($usernameKey, "Process")
$originalPassword = [Environment]::GetEnvironmentVariable($passwordKey, "Process")
$testExitCode = 1

try {
    [Environment]::SetEnvironmentVariable($usernameKey, $envValues[$usernameKey], "Process")
    [Environment]::SetEnvironmentVariable($passwordKey, $envValues[$passwordKey], "Process")

    Push-Location $PSScriptRoot
    try {
        dotnet test --filter "FullyQualifiedName~TestAssetCreation.CreateAsset_VerifyDetailsAndHistory" -- Playwright.LaunchOptions.Headless=false
        $testExitCode = $LASTEXITCODE
    }
    finally {
        Pop-Location
    }
}
finally {
    [Environment]::SetEnvironmentVariable($usernameKey, $originalUsername, "Process")
    [Environment]::SetEnvironmentVariable($passwordKey, $originalPassword, "Process")
}

exit $testExitCode