# CLAUDE.md

Guia para agentes de IA que trabalham neste repositório. Como o mod funciona hoje (regras, implementação, valores e o
porquê de cada escolha) está no [`MEMORY.md`](MEMORY.md): leia antes de mexer no mod. Ele descreve só o estado atual;
ao mudar o comportamento, reescreva o trecho correspondente em vez de acrescentar relato, e deixe o histórico para a
mensagem de commit. O [`README.md`](README.md) descreve o mod para quem joga.

## O que é este projeto

Mod para RimWorld 1.7 (**Maintenance Bench**, em PT-BR **Bancada de manutenção**): uma bancada que conserta, melhora,
recicla e remove a biocodificação de armas e roupas, alterando o próprio item em vez de recriá-lo. Independente de
outros mods de reparo; depende só do Harmony (`brrainz.harmony`).

## Comandos

| Comando | O que faz |
|---|---|
| `bun install` | instala o commitlint e, pelo `prepare`, o hook `commit-msg` |
| `bun run build` | `dotnet build` em Release; a DLL vai para `mod/maintenance-bench/Assemblies` |
| `bun run test` | xUnit da lógica pura |
| `bun run copy` | apaga e recopia o mod em `RimWorld\Mods\maintenance-bench` (robocopy) |
| `bun run hooks` | reinstala os hooks de `scripts/git-hooks/` |

O `bun run copy` precisa do RimWorld **fechado**: com o jogo aberto o DLL fica travado e a cópia falha no meio,
deixando a pasta do mod no jogo incompleta.

`.env` (não versionado; modelo em `.env.example`): `RIMWORLD_PATH` (instalação do jogo, usada pelo build e pelo copy)
e `HARMONY_PATH` (pasta com o `0Harmony.dll` do 1.7, usada só pelo build, sem copiar o DLL para `Assemblies`).

## Estrutura

```
mod/maintenance-bench/        o entregável: About, Defs, Languages, Assemblies (gerado, fora do git)
src/MaintenanceBench/         projeto do mod (net10.0), compila para mod/maintenance-bench/Assemblies
tests/MaintenanceBench.Tests/ xUnit da lógica pura
scripts/copy.ps1              instala o mod no jogo
scripts/tools/                apoio à análise, rodados à mão com `pwsh -NoProfile -File`
scripts/git-hooks/            hooks versionados (commit-msg com commitlint)
scripts/install-git-hooks.ts  copia os hooks para .git/hooks
commitlint.config.js          regras dos commits
```

- **Alvo:** só RimWorld 1.7, nativo em `net10.0`, compilado contra o `Assembly-CSharp.dll` da instalação. Requer o
  SDK do .NET 10.
- **Identidade:** `packageId` `rodrigo54.maintenancebench`, autor Rodrigo54.
- **Idioma:** defs em inglês; tradução PT-BR em `Languages/PortugueseBrazilian/DefInjected/`. O mod não tem textos
  `Keyed`; mensagens de erro do C#, comentários e documentação em português. Termos da tradução oficial do jogo: bill
  é "tarefa", HP é "pontos de vida", biocode é "biocodificação", a inspiração de criatividade é "artista".
- **Textura:** a bancada usa a da mesa de usinagem (`TableMachining`).

## Como trabalhar

- **Ciclo:** `bun run build`, `bun run test`, `bun run copy`; então o Rodrigo reinicia o RimWorld e valida em jogo.
  DLL e defs só são lidos ao abrir o jogo.
- **Validação em jogo é do Rodrigo.** Uma mudança só conta como pronta depois do OK dele em jogo.
- **Testado vs. lido:** ao relatar, separe o que foi executado (comando, teste, jogo) do que foi só lido (código
  descompilado, XML) e do que não foi verificado. Conclusão de leitura de código não é "confirmado".
- **Testes automatizados** cobrem só lógica pura, em classes sem tipos do jogo (`RepairMath`, `RecycleMath`,
  `RangeLimit`). O que toca o jogo é validado em jogo; o mod não leva código de debug.
- **Harmony só onde já foi decidido** (os dois patches listados no `MEMORY.md`). Novo patch é decisão do Rodrigo; o
  padrão é usar os pontos de extensão do jogo (`RecipeWorker`, filtros especiais, defs).

## Armadilhas do jogo

- **Toda receita precisa de `workAmount`:** sem ele, `WorkAmountForStuff` lê o `WorkToMake` de `products[0]`, e as
  receitas do mod não têm produtos.
- **Proibir filtro especial com `specialFiltersToDisallow`, nunca `disallowedSpecialFilters`:** o segundo é desfeito
  para filtros configuráveis quando a categoria é liberada (`ThingFilter.ResolveReferences`).
- **Filtro de qualidade deixa passar item sem qualidade:** `ThingFilter.Allows` só checa quem tem `CompQuality`; por
  isso existe o `SpecialThingFilterWorker_WithoutQuality`.
- **O filtro arredonda o HP para centésimos:** item com 99,5% ou mais conta como 100%.
- **Filtro padrão só vale para bill nova:** bill já criada num save mantém o filtro que tinha; para testar mudança de
  `defaultIngredientFilter`, crie a bill de novo.
- **Arma persona:** `CompBladelinkWeapon` herda de `CompBiocodable`, e `UnCode()` desfaz o vínculo com o pawn.
- **Skill 20 decai mesmo trabalhando:** para validar ganho de XP, use colono com skill abaixo de 10 (ver `MEMORY.md`).

## Ambiente de teste

- **Mods:** gerenciados pelo RimPy (`D:\RimPy_Windows`). O perfil `maintenance-bench` (Harmony, Core, DLCs e o mod)
  está em `%USERPROFILE%\AppData\LocalLow\RimPy Mod Manager\ModLists\maintenance-bench.xml`. O Rodrigo também testa
  em saves próprios com a lista completa de mods.
- **Log do jogo:** `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Logs\Player.log` (a
  sessão anterior fica em `Player-prev.log`). `scripts/tools/summarize-log.ps1` agrupa os erros e avisos com a
  origem de cada um.
- **Ações de debug usadas na validação** (menu em inglês): Spawning → `Spawn thing`; General → `10 damage`,
  `300 damage`, `Set quality`, `Finish all research`; Pawns → `Set skill`, `Inspiration` → `Inspired_Creativity`.
  God mode fica na barra do modo dev.
- **Código do jogo:** `ilspycmd -p -o <pasta> "D:\SteamLibrary\steamapps\common\RimWorld\Assembly-CSharp.dll"`
  descompila o jogo para consulta (ou `-t <Tipo>` para um tipo só); a pasta não faz parte do projeto.
- **Outros scripts:** `find-costless-items.ps1` lista as armas e roupas sem custo de fabricação (base do filtro da
  reciclagem); `vanilla-references.ps1` extrai custo, energia e tempo de bancadas e receitas do jogo base.

## Git e commits

- **Repositório:** `github.com/Rodrigo54/maintenance-bench`, público, licença MIT. GitFlow com `main` e `develop`;
  branches de trabalho em `feature/<descricao-em-ingles>`.
- **Commits:** Conventional Commits com emoji obrigatório por tipo, a mesma convenção do sagittarius-species
  (`✨ feat(escopo): descrição em português`). Tipos e emojis em `commitlint.config.js`: ✨ feat, 🐛 fix, 📚 docs,
  💄 style, ♻️ refactor, 🚀 perf, 🧪 test, 📦 build, 🚧 ci, 🛠️ chore, ⏪ revert.
- **Sem trailer `Co-Authored-By` de IA:** o hook recusa o commit.
- **A DLL compilada fica fora do git:** `bun run build` gera de novo.
