# Global360 Playwright Test Framework

UI automation framework for the Snipe-IT demo application, built with .NET, NUnit, Microsoft Playwright, and Allure.

## Prerequisites

- Windows with PowerShell
- .NET 10 SDK
- Node.js and npm (required for the Allure CLI)
- Access to the Snipe-IT demo application and a valid test account

## Setup

Restore and build the project:

```powershell
dotnet restore
dotnet build
```

Install the Playwright Chromium browser after building:

```powershell
.\bin\Debug\net10.0\playwright.ps1 install chromium
```

Create a local credentials file from the template:

```powershell
Copy-Item .env.example .env
```

Set `Configuration__Username` and `Configuration__Password` in `.env` to the test account credentials. `.env` is ignored by Git; do not commit credentials. The application URL is configured in `settings.json` under `Configuration:BaseUrl`.

## Run the Test

Run the focused test in headed mode:

```powershell
.\run-headed-test.ps1
```

The script validates the `.env` values, exposes them to the test process as environment variables, runs `CreateAsset_VerifyDetailsAndHistory`, and restores the prior environment values afterward. The test project reads those variables through `ConfigUtils`.

To run tests directly with `dotnet test`, set `Configuration__Username` and `Configuration__Password` in the current process environment first. Direct `dotnet test` does not load `.env` itself.

## Allure Reports

The NUnit adapter writes raw test results to `allure-results` at the project root. Failure screenshots are attached to their individual Allure test results. Generated results and reports are ignored by Git.

Install the Allure CLI if it is not already available:

```powershell
npm install --global allure
```

Generate and open a report from the project root:

```powershell
allure serve .\allure-results
```

`allure serve` builds a temporary report and opens it in a browser. Allure CLI requires Node.js.

## Framework Layout

- `Tests/` contains NUnit test fixtures and `BaseTest`, which loads settings and attaches a screenshot when a test fails.
- `PageObjects/` contains page-specific locators and UI actions for login, dashboard, asset creation, asset listing, and asset details/history.
- `TestData/` contains the asset-creation data record and per-run unique tag/serial generation.
- `Utils/` contains configuration, path, and screenshot helpers.
- `settings.json` contains non-secret application settings, including the base URL.
- `.env.example` documents the required local credential keys; `.env` holds local-only values.
- `allureConfig.json` configures the Allure result directory.

## Test Scenario

The asset scenario logs in, selects form data, submits an asset, verifies the success notification, searches for the generated Asset Tag, checks the asset details, and validates the creation and checkout entries in History.

