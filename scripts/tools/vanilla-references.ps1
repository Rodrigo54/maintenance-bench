# Extrai do Core os valores de referência para a tabela provisória do Maintenance Bench:
# custo, energia, textura e pesquisa das bancadas de produção, e tempo/skill das receitas comparáveis.
param([string]$RimWorldPath = 'D:\SteamLibrary\steamapps\common\RimWorld')

$ErrorActionPreference = 'Stop'

$defsByName = @{}
$thingDefs = @{}
$recipeDefs = @{}

foreach ($file in Get-ChildItem (Join-Path $RimWorldPath 'Data\Core\Defs') -Recurse -Filter *.xml) {
    $document = [xml](Get-Content $file.FullName -Raw)
    foreach ($node in $document.SelectNodes('/Defs/*')) {
        $name = $node.GetAttribute('Name')
        if ($name) { $defsByName[$name] = $node }
        $defName = $node.SelectSingleNode('defName')
        if (-not $defName) { continue }
        if ($node.LocalName -eq 'ThingDef') { $thingDefs[$defName.InnerText] = $node }
        if ($node.LocalName -eq 'RecipeDef') { $recipeDefs[$defName.InnerText] = $node }
    }
}

# Primeiro nó encontrado pelo XPath subindo a herança por ParentName.
function Get-Inherited($node, [string]$xpath) {
    while ($node) {
        $found = $node.SelectSingleNode($xpath)
        if ($found) { return $found }
        $parentName = $node.GetAttribute('ParentName')
        $node = if ($parentName) { $defsByName[$parentName] } else { $null }
    }
    return $null
}

function Format-CostList($node) {
    $costList = Get-Inherited $node 'costList'
    if (-not $costList) { return '' }
    ($costList.ChildNodes | Where-Object NodeType -eq 'Element' | ForEach-Object { "$($_.LocalName) $($_.InnerText)" }) -join ', '
}

'=== Bancadas de produção'
$tables = $thingDefs.Values | Where-Object {
    (Get-Inherited $_ 'thingClass').InnerText -match 'WorkTable' -and $_.GetAttribute('Abstract') -ne 'True'
}
$tables | ForEach-Object {
    [pscustomobject]@{
        DefName   = $_.SelectSingleNode('defName').InnerText
        Cost      = Format-CostList $_
        Stuff     = (Get-Inherited $_ 'costStuffCount').InnerText
        Power     = (Get-Inherited $_ 'comps/li[@Class="CompProperties_Power"]/basePowerConsumption').InnerText
        Unpowered = (Get-Inherited $_ 'building/unpoweredWorkTableWorkSpeedFactor').InnerText
        Work      = (Get-Inherited $_ 'statBases/WorkToBuild').InnerText
        Size      = (Get-Inherited $_ 'size').InnerText
        Research  = ((Get-Inherited $_ 'researchPrerequisites').ChildNodes | ForEach-Object InnerText) -join ','
        Texture   = (Get-Inherited $_ 'graphicData/texPath').InnerText
    }
} | Sort-Object DefName | Format-Table -AutoSize | Out-String -Width 250

'=== Receitas comparáveis'
foreach ($defName in 'SmeltWeapon', 'SmeltApparel', 'SmeltOrDestroyThing', 'DestroyWeapon', 'Make_ComponentIndustrial', 'Make_Apparel_FlakVest', 'Make_Gun_Revolver', 'Make_Gun_AssaultRifle', 'Make_MeleeWeapon_LongSword') {
    $recipe = $recipeDefs[$defName]
    if (-not $recipe) { "$defName (não encontrado)"; continue }
    $skills = Get-Inherited $recipe 'skillRequirements'
    $skillText = if ($skills) { ($skills.ChildNodes | Where-Object NodeType -eq 'Element' | ForEach-Object { "$($_.LocalName) $($_.InnerText)" }) -join ', ' } else { '' }
    '{0,-28} work={1,-6} skills={2}' -f $defName, (Get-Inherited $recipe 'workAmount').InnerText, $skillText
}

'=== WorkToMake de itens comparáveis'
foreach ($defName in 'Gun_Revolver', 'Gun_AssaultRifle', 'MeleeWeapon_LongSword', 'Apparel_FlakVest', 'Apparel_Parka', 'Apparel_PowerArmor') {
    $thing = $thingDefs[$defName]
    if (-not $thing) { "$defName (não encontrado)"; continue }
    '{0,-24} WorkToMake={1,-6} skill={2}' -f $defName, (Get-Inherited $thing 'statBases/WorkToMake').InnerText, ((Get-Inherited $thing 'recipeMaker/skillRequirements').ChildNodes | ForEach-Object { "$($_.LocalName) $($_.InnerText)" }) -join ', '
}
