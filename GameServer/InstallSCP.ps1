# 관리자 권한으로 실행할것
Add-WindowsCapability -Online -Name OpenSSH.Client~~~~0.0.1.0

# 1. OpenSSH 경로를 환경 변수에 추가
$targetPath = "C:\Windows\System32\OpenSSH\"
$oldPath = [Environment]::GetEnvironmentVariable("Path", "Machine")

if ($oldPath -notlike "*$targetPath*") {
    $newPath = $oldPath + ";" + $targetPath
    [Environment]::SetEnvironmentVariable("Path", $newPath, "Machine")
    Write-Host "ok: added Path" -ForegroundColor Green
    Write-Host "please, enter 'ssh' in new terminal" -ForegroundColor Yellow
} else {
    Write-Host "already added Path." -ForegroundColor Cyan
}
