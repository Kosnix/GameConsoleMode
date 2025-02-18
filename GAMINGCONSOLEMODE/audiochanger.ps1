param (
    [string]$nircmdPath,  # Path to NirCmd
    [string]$deviceName   # Name of the audio device
)

# Check if NirCmd exists
if (-Not (Test-Path $nircmdPath)) {
    Write-Host "Error: NirCmd.exe not found at the specified path: $nircmdPath"
    exit
}

# Run NirCmd to set the default playback device
Write-Host "Setting default playback device to: $deviceName"
Start-Process -FilePath $nircmdPath -ArgumentList "setdefaultsounddevice `"$deviceName`" 0" -NoNewWindow -Wait

# Confirm success
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully set the default playback device to: $deviceName"
} else {
    Write-Host "Failed to set the default playback device. Exit code: $LASTEXITCODE"
}
