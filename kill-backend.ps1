Write-Host "Dang dung backend..." -ForegroundColor Yellow
Stop-Process -Name "backend" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1
Write-Host "Backend da dung!" -ForegroundColor Green
