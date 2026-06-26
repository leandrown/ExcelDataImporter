<#
.SYNOPSIS
    Backs up the ExcelImporter SQL Server database and removes backups older than N days.

.PARAMETER ServerInstance
    SQL Server instance name. Default: localhost.

.PARAMETER DatabaseName
    Database to back up. Default: ExcelImporter.

.PARAMETER BackupDir
    Directory where backup files will be saved.

.PARAMETER RetentionDays
    Number of days to keep backup files. Default: 7.

.EXAMPLE
    .\backup-database.ps1 -BackupDir "C:\Backups"
#>

param(
    [string]$ServerInstance = "localhost",
    [string]$DatabaseName = "ExcelImporter",
    [string]$BackupDir = "$PSScriptRoot\Backups\Database",
    [int]$RetentionDays = 7
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Ensure backup directory exists
if (-not (Test-Path -Path $BackupDir)) {
    New-Item -ItemType Directory -Path $BackupDir | Out-Null
    Write-Host "Created backup directory: $BackupDir"
}

# Generate backup filename with timestamp
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupFile = Join-Path -Path $BackupDir -ChildPath "$DatabaseName-$timestamp.bak"

Write-Host "Starting backup of '$DatabaseName' on '$ServerInstance'..."

$sql = @"
BACKUP DATABASE [$DatabaseName]
TO DISK = N'$backupFile'
WITH FORMAT, INIT, COMPRESSION,
    NAME = N'$DatabaseName-Full Database Backup',
    STATS = 10;
"@

try {
    Invoke-Sqlcmd -ServerInstance $ServerInstance -Query $sql
    Write-Host "Backup completed successfully: $backupFile"
}
catch {
    Write-Error "Backup failed: $_"
    exit 1
}

# Remove backups older than RetentionDays
Write-Host "Removing backup files older than $RetentionDays days..."
$cutoff = (Get-Date).AddDays(-$RetentionDays)

Get-ChildItem -Path $BackupDir -Filter "*.bak" |
    Where-Object { $_.LastWriteTime -lt $cutoff } |
    ForEach-Object {
        Remove-Item -Path $_.FullName -Force
        Write-Host "Deleted: $($_.Name)"
    }

Write-Host "Backup cleanup completed."
