# Lista armas e roupas do jogo (Core + DLCs) sem custo de fabricação,
# resolvendo a herança por ParentName entre todos os ThingDefs.
param([string]$RimWorldPath = 'D:\SteamLibrary\steamapps\common\RimWorld')

$ErrorActionPreference = 'Stop'

$defsByName = @{}
$concreteDefs = [System.Collections.Generic.List[object]]::new()

foreach ($expansion in Get-ChildItem (Join-Path $RimWorldPath 'Data') -Directory) {
    $defsPath = Join-Path $expansion.FullName 'Defs'
    if (-not (Test-Path $defsPath)) { continue }
    foreach ($file in Get-ChildItem $defsPath -Recurse -Filter *.xml) {
        $document = [xml](Get-Content $file.FullName -Raw)
        foreach ($node in $document.SelectNodes('/Defs/ThingDef')) {
            $entry = [pscustomobject]@{ Node = $node; Expansion = $expansion.Name }
            $name = $node.GetAttribute('Name')
            if ($name) { $defsByName[$name] = $entry }
            if ($node.GetAttribute('Abstract') -ne 'True') { $concreteDefs.Add($entry) }
        }
    }
}

# Devolve o primeiro nó filho com esse nome subindo a cadeia de herança.
function Get-InheritedNode($entry, [string]$childName) {
    $current = $entry
    while ($current) {
        $child = $current.Node.SelectSingleNode($childName)
        if ($child) { return $child }
        $parentName = $current.Node.GetAttribute('ParentName')
        $current = if ($parentName) { $defsByName[$parentName] } else { $null }
    }
    return $null
}

$results = foreach ($entry in $concreteDefs) {
    $isApparel = $null -ne (Get-InheritedNode $entry 'apparel')
    $equipmentType = Get-InheritedNode $entry 'equipmentType'
    $isWeapon = $equipmentType -and $equipmentType.InnerText -eq 'Primary'
    if (-not ($isApparel -or $isWeapon)) { continue }

    $useHitPoints = Get-InheritedNode $entry 'useHitPoints'
    if ($useHitPoints -and $useHitPoints.InnerText -match 'false') { continue }

    $hasCostList = $null -ne (Get-InheritedNode $entry 'costList')
    $hasStuffCost = $null -ne (Get-InheritedNode $entry 'costStuffCount')
    if ($hasCostList -or $hasStuffCost) { continue }

    $label = Get-InheritedNode $entry 'label'
    [pscustomobject]@{
        Expansion = $entry.Expansion
        Kind      = if ($isApparel) { 'apparel' } else { 'weapon' }
        DefName   = $entry.Node.SelectSingleNode('defName').InnerText
        Label     = if ($label) { $label.InnerText } else { '' }
    }
}

"Total de armas/roupas concretas sem custo: $(@($results).Count)"
$results | Sort-Object Expansion, Kind, DefName | Format-Table -AutoSize | Out-String -Width 200
