# Sincroniza mod/maintenance-bench com a pasta Mods da instalação do RimWorld.
# Apaga o destino e recopia do zero, reportando quanto o mod cresceu ou
# encolheu desde a última sincronização.

$ErrorActionPreference = 'Stop'

# O robocopy usa o exit code como bitmask e devolve 1 quando copia arquivos
# com sucesso. Sem isto, um PowerShell configurado para tratar exit code de
# executável nativo como erro abortaria o script justamente quando dá certo.
$PSNativeCommandUseErrorActionPreference = $false

if (-not $IsWindows) {
    Write-Host '✗ este script requer Windows (robocopy).' -ForegroundColor Red
    exit 1
}

$repoRoot = (Get-Item $PSScriptRoot).Parent.FullName

# RIMWORLD_PATH vem do ambiente ou, na falta dele, do .env: o `bun run` não
# repassa o .env aos scripts. Mesma regra do Directory.Build.props.
$rimWorldPath = $env:RIMWORLD_PATH
$envFile = Join-Path $repoRoot '.env'
if (-not $rimWorldPath -and (Test-Path $envFile)) {
    $match = Select-String -Path $envFile -Pattern '^RIMWORLD_PATH=(.*?)\s*$' | Select-Object -First 1
    if ($match) { $rimWorldPath = $match.Matches[0].Groups[1].Value }
}

if (-not $rimWorldPath) {
    Write-Host '✗ RIMWORLD_PATH não definido. Copie .env.example para .env.' -ForegroundColor Red
    exit 1
}

$modName = 'maintenance-bench'
$source = Join-Path $repoRoot "mod/$modName"
$modsFolder = Join-Path $rimWorldPath 'Mods'
$destination = Join-Path $modsFolder $modName

if (-not (Test-Path $source)) {
    Write-Host "✗ pasta de origem não encontrada: $source" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $modsFolder)) {
    Write-Host "✗ pasta Mods não encontrada: $modsFolder" -ForegroundColor Red
    exit 1
}

# Mede uma pasta numa passada só. Devolve $null se ela não existe — é o que
# distingue "instalação nova" de "nada mudou" na linha final.
function Measure-Folder([string]$path) {
    if (-not (Test-Path $path)) { return $null }
    $measure = Get-ChildItem -Path $path -Recurse -File -Force |
        Measure-Object -Property Length -Sum
    return [pscustomobject]@{
        Files = [int]$measure.Count
        Bytes = [long]($measure.Sum ?? 0)
    }
}

function Format-KB([double]$bytes) {
    return ('{0:N1} KB' -f ($bytes / 1KB))
}

Write-Host "→ sincronizando $modName → $modsFolder"

# Precisa medir antes do Remove-Item: depois de apagar, não há mais com o que comparar.
$before = Measure-Folder $destination

if ($null -ne $before) {
    Remove-Item -Path $destination -Recurse -Force
}

# /E copia subpastas, inclusive vazias. As demais flags silenciam o log por
# arquivo, o cabeçalho e o sumário.
robocopy $source $destination /E /NFL /NDL /NJH /NJS /NP | Out-Null

# 0-7 é sucesso (com ou sem cópia), 8 ou mais é falha real.
if ($LASTEXITCODE -ge 8) {
    Write-Host "✗ robocopy falhou (exit code $LASTEXITCODE) — o mod local pode estar incompleto." -ForegroundColor Red
    exit $LASTEXITCODE
}

$after = Measure-Folder $destination

if ($null -eq $before) {
    $change = 'instalação nova'
} elseif ($before.Bytes -eq $after.Bytes) {
    $change = 'sem variação desde a última sincronização'
} else {
    $delta = $after.Bytes - $before.Bytes
    $sign = if ($delta -gt 0) { '+' } else { '-' }
    $change = "$sign$(Format-KB ([Math]::Abs($delta))) desde a última sincronização"
}

Write-Host "✓ $modName sincronizado — $($after.Files) arquivos, $(Format-KB $after.Bytes) ($change)."
exit 0
