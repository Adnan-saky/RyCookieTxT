Add-Type -AssemblyName System.Drawing

$resourceDir = "d:\Code\rycookietext\Resources"
$pngPath = Join-Path $resourceDir "cookie_logo.png"
$icoPath = Join-Path $resourceDir "icon.ico"

Write-Host "Converting $pngPath to $icoPath..."

if (-not (Test-Path $pngPath)) {
    Write-Error "PNG file not found at $pngPath"
    exit 1
}

try {
    # Load the PNG
    $bitmap = [System.Drawing.Bitmap]::FromFile($pngPath)
    
    # Resize to standard icon size (256x256)
    $resized = new-object System.Drawing.Bitmap 256, 256
    $g = [System.Drawing.Graphics]::FromImage($resized)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.DrawImage($bitmap, 0, 0, 256, 256)
    $g.Dispose()
    
    # Convert to Icon
    $handle = $resized.GetHicon()
    $icon = [System.Drawing.Icon]::FromHandle($handle)
    
    # Save to file
    $fileStream = New-Object System.IO.FileStream $icoPath, 'Create'
    $icon.Save($fileStream)
    $fileStream.Close()
    
    # Cleanup
    $icon.Dispose()
    $resized.Dispose()
    $bitmap.Dispose()
    
    Write-Host "Successfully created icon.ico" -ForegroundColor Green
}
catch {
    Write-Error "Failed to convert icon: $_"
    exit 1
}
