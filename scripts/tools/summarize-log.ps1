# Resume um Player.log do RimWorld 1.7: agrupa erros e avisos pela primeira linha da mensagem.
# No 1.7 cada mensagem começa com "data hora NÍVEL [categoria]" e é seguida do stack de quem a gerou.
param([string]$LogPath = "$env:USERPROFILE\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Logs\Player.log")

$ErrorActionPreference = 'Stop'
$lines = Get-Content $LogPath

$entries = [System.Collections.Generic.List[object]]::new()
for ($i = 0; $i -lt $lines.Count; $i++) {
    # Detecta pelo cabeçalho, e não pelo stack: o Log.ErrorOnce não passa pelo LogCategory.Error.
    $match = [regex]::Match($lines[$i], '^\S+ \S+\s+(ERROR|WARN) ')
    if (-not $match.Success) { continue }
    # Sem data e hora, para que a mesma mensagem se agrupe.
    $message = $lines[$i].Substring($match.Length).Trim()

    # A primeira linha do stack que não é do logger aponta quem gerou a mensagem.
    $origin = ''
    for ($k = $i + 1; $k -lt [Math]::Min($i + 12, $lines.Count); $k++) {
        if ($lines[$k] -match '^\s+at (?!Verse\.Log\.|Brutal\.)(.+?) \[') { $origin = $Matches[1]; break }
    }

    $entries.Add([pscustomobject]@{
        Level   = if ($match.Groups[1].Value -eq 'ERROR') { 'Error' } else { 'Warning' }
        Line    = $i + 1
        Message = $message.Substring(0, [Math]::Min(120, $message.Length))
        Origin  = $origin.Substring(0, [Math]::Min(90, $origin.Length))
    })
}

"Linhas no log: $($lines.Count)"
"Erros/avisos encontrados: $($entries.Count)"
''
$entries | Group-Object Level, Message, Origin | ForEach-Object {
    [pscustomobject]@{
        Count     = $_.Count
        Level     = $_.Group[0].Level
        FirstLine = $_.Group[0].Line
        Message   = $_.Group[0].Message
        Origin    = $_.Group[0].Origin
    }
} | Sort-Object FirstLine | Format-List | Out-String -Width 250
