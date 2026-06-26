<#
.SYNOPSIS
    Sends an Excel file to the Excel Data Importer API.

.PARAMETER FilePath
    Path to the .xlsx file to import.

.PARAMETER ApiUrl
    Base URL of the running API. Default: https://localhost:5001.

.EXAMPLE
    .\trigger-import.ps1 -FilePath "C:\data\contacts.xlsx"
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$FilePath,
    [string]$ApiUrl = "https://localhost:5001"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if (-not (Test-Path -Path $FilePath)) {
    Write-Error "File not found: $FilePath"
    exit 1
}

if (-not $FilePath.EndsWith(".xlsx")) {
    Write-Error "Invalid file type. Only .xlsx files are supported."
    exit 1
}

$endpoint = "$ApiUrl/api/import"
Write-Host "Importing '$FilePath' via '$endpoint' ..."

try {
    $response = Invoke-RestMethod `
        -Uri $endpoint `
        -Method Post `
        -Form @{ file = Get-Item -Path $FilePath } `
        -SkipCertificateCheck # safe for local development; remove in production

    Write-Host "Import completed:"
    Write-Host "Operation ID: $($response.operationId)"
    Write-Host "File: $($response.fileName)"
    Write-Host "Total rows: $($response.totalRows)"
    Write-Host "Success: $($response.successRows)"
    Write-Host "Errors: $($response.errorRows)"
    Write-Host "Status: $($response.status)"

    if ($response.errors.Count -gt 0) {
        Write-Host "`nRows with errors:"
        $response.errors | ForEach-Object {
            Write-Host " Row $($_.rowNumber): $($_.errorMessage)"
        }
    }
}
catch {
    Write-Error "Import failed: $_"
    exit 1
}
