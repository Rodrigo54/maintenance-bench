# MEMORY.md — Maintenance Bench

Como o mod funciona hoje: regras de jogo, implementação, valores e o porquê de cada escolha. Comandos, ambiente de
teste e convenções estão no [`CLAUDE.md`](CLAUDE.md). Mantenha este arquivo descrevendo o estado atual; histórico vai
nas mensagens de commit.

## Estado

Todas as receitas estão implementadas e validadas em jogo, com tradução PT-BR.

Nas seções abaixo, o que só foi verificado por leitura do código do jogo está marcado como *leitura de código*; o
resto foi validado em jogo.

## Conceito

Uma única bancada que cuida do equipamento da colônia do começo ao fim da vida útil: conserta armas e roupas, pode
melhorar a qualidade delas, recicla o que não vale mais consertar e remove o biocode de equipamento saqueado.

Nenhum mod para as versões recentes junta essas funções: o *Tinkering Bench and Biohacking Station* re-rola
qualidade e remove biocode, mas não repara; os mods de reparo (MendAndRecycle, Repair Workbench, [Og] Repair Your
Gear) não mexem em qualidade nem em biocode. O Maintenance Bench não depende de nenhum deles nem reaproveita código.

## Regras de jogo

### A bancada

- **Pesquisa:** Metalurgia (`Smithing`). O mod não cria pesquisas; todas as receitas usam pesquisas do jogo base.
- **Energia opcional:** 50% da velocidade sem energia, 100% ligada.
- **Itens aceitos:** armas e roupas (incluindo armaduras) com HP, pelas categorias `Weapons` e `Apparel`. Recursos que
  o jogo trata como arma equipável (cerveja, madeira, presas, chifres) ficam em `Drugs`, `ResourcesRaw` e `ItemsMisc`
  e não entram.
- **Trabalho:** Artesanato (`Crafting`).

### Receitas

| Receita | Pesquisa | Custo por execução | Item aceito | Efeito |
|---|---|---|---|---|
| Reparo simples | Metalurgia (`Smithing`) | só tempo | danificado | devolve até 25% do HP máximo; chance de upgrade 0,5x |
| Reparo avançado | Usinagem (`Machining`) | 1 componente industrial | danificado | devolve até 75% do HP máximo; chance de upgrade 1x |
| Melhoria de item | Usinagem (`Machining`) | 2 componentes industriais | 100% de HP | chance de upgrade 4x |
| Reciclagem | Metalurgia (`Smithing`) | só tempo | danificado | devolve parte dos materiais de fabricação |
| Remoção de biocode | Microeletrônica (`MicroelectronicsBasics`) | 3 componentes industriais | biocodificado, exceto arma persona | remove o biocode |

Microeletrônica exige Eletricidade, mas não Usinagem, então a remoção de biocode pode sair antes do reparo avançado.
Isso é aceito.

### Reparo

- **Fatia por execução:** 25% do HP máximo no simples, 75% no avançado. A última execução só completa o que falta.
- **Repetição:** a bill continua pegando o item enquanto ele estiver danificado. Um item com 10% de HP precisa de
  quatro execuções simples ou duas avançadas.
- **Papel de cada receita:** o simples é gratuito e lento, para manutenção barata; o avançado consome componente e é
  mais rápido, para equipamento valioso ou pressa.
- **Faixa padrão da bill:** o reparo simples nasce aceitando todo item danificado; o avançado nasce com 0 a 25% de HP,
  e o jogador pode ampliar até 99%.

### Chance de upgrade

Toda execução de reparo ou de melhoria pode subir a qualidade em **um nível**. A qualidade nunca piora.

- **Chance base *p*:** a chance de uma rolagem normal de fabricação daquele pawn dar qualidade acima da atual. Usa a
  curva de qualidade do jogo base.
- **Reparo:** chance = *p* × multiplicador × fração do HP máximo devolvida na execução. Para um item de 0% a 100%, o
  simples soma 0,5 *p* (quatro execuções de 25%) e o avançado 1 *p* (75% + 25%): o multiplicador é a "chance por barra
  de HP restaurada". A fração impede o abuso de reparar todo dia o desgaste mínimo de uma roupa para ganhar uma
  tentativa cheia.
- **Melhoria:** chance = *p* × 4, sem fração, porque não restaura HP.
- **Tetos:** o reparo sobe no máximo até obra-prima. A melhoria sobe até obra-prima para qualquer crafter; o passo
  para lendário só acontece com a inspiração de criatividade (consumida) ou o papel de especialista em produção.

**Por que +1 nível, e não salto.** O Tinkering Bench fica com a maior entre a qualidade atual e a rolada, podendo
pular níveis. Execuções esperadas até obra-prima com Crafting 17 (cálculo analítico sobre a rolagem do jogo):

| Regra | De péssimo | De normal | De excelente |
|---|---|---|---|
| +1 nível, 4 rolagens (adotada) | 9,8 | 7,8 | 5,7 |
| Salto, 1 rolagem (Tinkering) | 21,1 | 21,1 | 21,1 |
| Salto, 4 rolagens | 5,7 | 5,7 | 5,7 |

Com salto, o passo para obra-prima não acelera (depende de uma rolagem de obra-prima, de onde quer que se parta); só a
subida dos níveis baixos fica mais rápida, e um item péssimo viraria excelente numa execução em 72% das vezes. As 4
rolagens já resolvem o passo para obra-prima, e a subida gradual mantém o peso dos componentes.

**Chance da melhoria por execução, de excelente para obra-prima** (simulação da rolagem do jogo, 400 mil amostras;
uma fabricação sai obra-prima em 4,8% das vezes com Crafting 17 e 9,5% com 20):

| Crafting | 4 rolagens |
|---|---|
| 14 | 8,1% |
| 17 | 17,9% |
| 20 | 33,0% |

### Melhoria de item

- **Por que existe:** roupas vestidas perdem HP todo dia e saque chega danificado, então ganham chances pelo reparo.
  Armas da colônia ficam em 100% de HP e quase nunca seriam reparadas. A melhoria cobre esse caso e é o único caminho
  para lendário.
- **Ordem dos itens:** primeiro os de pior qualidade, obras-primas por último.
- **Rodízio:** o item tentado espera uma hora de jogo antes de ser escolhido de novo. Com vários itens do mesmo nível a
  bill passa por todos; com um só, ela espera.

### Reciclagem

- **Elegibilidade:** item danificado (até 99% de HP). A bill nasce aceitando 25% de HP ou menos, e o jogador pode
  ampliar até 99%.
- **Retorno:** 20% + 2,75% × Crafting do custo de fabricação (20% com Crafting 0, 75% com 20).
- **Componentes:** nunca são devolvidos.
- **Item sem custo de fabricação:** não aparece como ingrediente. São 44 armas e roupas no jogo base e nas DLCs, quase
  todas saque valioso (zeushammer, equipamento de eltex, lança-foguetes); continuam podendo ser reparadas.
- **Diferença para a fundição do jogo base:** cobre roupas de tecido e couro e não depende de fundição elétrica.

### Remoção de biocode

- **Itens:** armas e roupas biocodificadas.
- **Arma persona fica de fora:** no código do jogo ela é um tipo de biocode, e removê-lo desfaz o vínculo com o pawn.
- **Requisito:** Intellectual 10 e Crafting 3, como no Tinkering Bench. O XP vai para Intellectual (`workSkill`).

## Valores

| Item | Valor | Referência |
|---|---|---|
| Custo da bancada | 150 aço, sem componente | Forja a combustível: 100 aço; mesa de usinagem: 150 aço + 5 componentes. Sem componente porque a bancada sai em Metalurgia, antes de a colônia fabricar componentes. |
| Construção | 3000 de trabalho, 3×1 | Forjas e mesa de usinagem |
| Energia ligada | 120 W | Forja elétrica: 210 W |
| Textura | mesa de usinagem (`TableMachining`) | — |
| Reparo simples | 2500 por execução, Crafting 3 | Mais lento que o avançado, que é o papel do simples. Revólver pede Crafting 3. |
| Reparo avançado | 2000 por execução, Crafting 5 | Espada longa pede Crafting 5 |
| Melhoria de item | 3000, Crafting 6 | Tinkering Bench: 6000 (roupa) e 12000 (arma); rifle de assalto pede Crafting 6 |
| Reciclagem | 2500, sem skill | Fundição do jogo: 1600 |
| Curva da reciclagem | 20% + 2,75% × Crafting | Fundição do jogo: 25% fixos |
| Remoção de biocode | 10000, Intellectual 10 e Crafting 3 | Tinkering Bench: 15000, com as mesmas skills |
| Ícones das receitas | `uiIconThing`: reparo simples e reciclagem `Steel`; reparo avançado e melhoria `ComponentIndustrial`; remoção de biocode `ComponentSpacer` | Sem produto e sem ícone, o menu "Adicionar tarefa" mostra um placeholder (`FloatMenuOption`, *leitura de código*). O jogo base faz igual no abate (`Meat_Muffalo`) e no desmonte de mecanoide (`Steel`). |

## Implementação

Base: código do `Assembly-CSharp.dll` do 1.7 descompilado com `ilspycmd`.

### Alterar o próprio item

O item não é destruído e recriado: a receita muda só o que promete (HP, qualidade ou biocode). Isso preserva cor,
estilo, arte gravada, traits e dados de outros mods; o Tinkering Bench recria o item e perde tudo isso.

- **Como:** receita sem produtos, com um `RecipeWorker` que sobrescreve `ConsumeIngredient` para não destruir o item e
  faz a alteração em `Notify_IterationCompleted` (base comum em `RecipeWorker_InPlace`).
- **Por que funciona:** em `Toils_Recipe.FinishRecipeAndStartStoringProduct`, o jogo monta os produtos, chama
  `ConsumeIngredients` (que delega a `recipe.Worker.ConsumeIngredient`) e depois `bill.Notify_IterationCompleted`,
  que `Bill_Production` repassa a `recipe.Worker.Notify_IterationCompleted(billDoer, ingredients)`. Com a lista de
  produtos vazia, o job termina com sucesso.
- **Consequência aceita:** o item fica em cima da bancada ao terminar; o modo "levar para o estoque" da bill não se
  aplica e o item segue pelo transporte normal. Entre execuções de reparo, o pawn deixa o item na bancada e emenda a
  próxima.

### Chamadas usadas

- **Reparo:** ajuste direto de `HitPoints`.
- **Qualidade:** `CompQuality.SetQuality(q, null)`. Sem contexto de arte, não reinicializa a arte.
- **Biocode:** `CompBiocodable.UnCode()`.
- **Arma persona:** `CompBladelinkWeapon` herda de `CompBiocodable`, e seu `UnCode()` desfaz o vínculo. O filtro do jogo
  já a exclui: `SpecialThingFilterWorker_NonBiocodedWeapons` a considera "não biocodificada" e
  `SpecialThingFilterWorker_BiocodedWeapons` a recusa. Mesmo assim, `RecipeWorker_RemoveBiocode` recusa arma persona
  com erro, caso algo fure o filtro.
- **Rolagem de qualidade:** `QualityUtility.GenerateQualityCreatedByPawn(pawn, skill, consumeInspiration)`, que já
  aplica o bônus do papel de produção e, se pedido, consome a inspiração. Sem inspiração nem papel, vai no máximo até
  obra-prima; a inspiração dá +2 níveis.

### Chance (`QualityUpgrade`)

- ***p*:** uma rolagem real de fabricação; sucesso se o resultado for maior que a qualidade atual.
- **Multiplicador × fração abaixo de 1:** um sorteio com essa probabilidade decide se a rolagem acontece.
- **4x da melhoria:** quatro rolagens independentes; basta uma dar certo. Fica um pouco abaixo de 4 *p* quando *p* é
  alto, o que é aceitável.
- **Lendário:** sai da própria rolagem do jogo. Para item obra-prima, a rolagem usa `consumeInspiration: true`; como no
  jogo base, a inspiração é gasta na tentativa mesmo que falhe, e só a primeira das quatro rolagens da melhoria a usa.
- **Teto do reparo:** checado antes do sorteio, então o reparo nunca rola para item obra-prima.

### Filtros

O filtro de ingredientes do jogo (`ThingFilter.Allows`) limita por:

- **Faixa de HP** (`allowedHitPointsPercents`): menos de 100% para reparos e reciclagem, exatamente 100% para a
  melhoria. O filtro arredonda o HP para centésimos, então 99,5% ou mais conta como 100%.
- **Faixa de qualidade** (`allowedQualities`): não oferece upgrade a item que já está no teto.
- **Biocode** (`specialFiltersToDisallow`): proibir os filtros do Core `AllowNonBiocodedWeapons` e
  `AllowNonBiocodedApparel` deixa passar só item biocodificado.

**Proibir filtro especial no XML com `specialFiltersToDisallow`**, como o jogo base (cremação). O
`disallowedSpecialFilters` é desfeito para filtros configuráveis: em `ThingFilter.ResolveReferences` as categorias são
liberadas primeiro, e `SetAllow(categoria)` libera junto os filtros especiais configuráveis dela; o
`specialFiltersToDisallow` é aplicado depois. Todas as receitas usam `specialFiltersToDisallow`.

**Filtro fixo e filtro da bill.** O `fixedIngredientFilter` é o limite que o jogador não vê nem muda; o filtro da bill
é o que ele edita (faixa de HP e de qualidade). A bill nova nasce com o `defaultIngredientFilter` da receita (sem ele, o
jogo copia o fixo em `RecipeDef.ResolveReferences`). Todas as receitas declaram o padrão, com as mesmas faixas do fixo,
exceto reciclagem e reparo avançado, que nascem com 0 a 25% dentro do limite de 0 a 99%. Bill já criada mantém o filtro
que tinha.

**Filtros especiais próprios** (`configurable` falso, um def para `Weapons` e outro para `Apparel`, como os de biocode
do Core):

- `SpecialThingFilterWorker_WithoutQuality`: o filtro de qualidade deixa passar item **sem** qualidade
  (`ThingFilter.Allows` só checa quem tem `CompQuality`); a melhoria proíbe esses itens com ele.
- `SpecialThingFilterWorker_WithoutReturnableCost`: item sem custo de fabricação ou cujo custo é só componente, que
  não teria nada a devolver na reciclagem.
- `SpecialThingFilterWorker_RecentlyImproved`: item em espera do rodízio da melhoria (`ImprovementCooldown`).
- `SpecialThingFilterWorker_OutsideImprovementTier`: item fora do nível de qualidade buscado no momento
  (`ImprovementSearchTier`).

### Harmony

Dois patches; o resto usa só pontos de extensão do jogo. Patch novo é decisão do Rodrigo.

- **Limite da janela da bill** (postfix em `ThingFilterUI.DoThingFilterConfigWindow`,
  `ThingFilterUI_DoThingFilterConfigWindow_Patch`): no jogo base os controles de HP e de qualidade vão sempre de 0 a
  100% e de péssimo a lendário (`DrawHitPointsFilterConfig` desenha de 0 a 1 e não recebe o filtro fixo), então o
  jogador escolheria uma faixa que o filtro fixo barra. O postfix devolve a faixa da bill ao limite do fixo, só quando
  o `parentFilter` é o filtro fixo de uma receita do mod (`RangeLimit.Clamp`, com teste). Sem Harmony não dá: o jogo
  cria a bill com classe fixa (`BillUtility.MakeNewBill`), e corrigir a faixa a cada quadro seria gambiarra.
- **Ordem por qualidade da melhoria** (prefixo em `WorkGiver_DoBill.TryFindBestBillIngredients`,
  `WorkGiver_DoBill_TryFindBestBillIngredients_Patch`, só para bills da melhoria): a busca do jogo para no primeiro
  conjunto completo perto da bancada (`TryFindBestIngredientsHelper`), então ordenar o resultado não basta. O patch
  roda a busca uma vez por nível de qualidade, do pior para o melhor (faixa do filtro fixo), e fica com o primeiro
  nível que achar item; o filtro `OutsideImprovementTier` tira os outros níveis de cada rodada. Cada rodada é a busca
  normal, com raio, filtro da bill, itens proibidos e alcance. Se nenhum nível achar item, uma busca sem restrição
  preenche os ingredientes que faltam.
- **Rodízio sem Harmony:** o item melhorado fica em cima da bancada e seria sempre o mais próximo. O
  `ImprovementCooldown` guarda o item tentado por uma hora de jogo, e o filtro `RecentlyImproved` o tira da bill. A
  espera fica só em memória, para não deixar entrada do mod no save; ao carregar, recomeça zerada.
- **Dependência:** `brrainz.harmony` (Workshop 2009463077, traz o `0Harmony.dll` do 1.7), em `modDependencies` e
  `loadAfter` do `About.xml`. O build referencia o DLL por `HARMONY_PATH`, sem copiá-lo para `Assemblies`.

### Reciclagem (`RecipeWorker_Recycle`)

Sem produtos fixos e sem Harmony. O `ConsumeIngredient` padrão destrói o item; em `Notify_IterationCompleted` o
worker lê `CostListAdjusted()` do item destruído (def e material continuam legíveis), pula o que é `intricate`, aplica
a curva (`RecycleMath`), arredonda com `GenMath.RoundRandom` e põe os materiais com `GenPlace.TryPlaceThing` perto do
pawn, que divide pilha acima do limite. Mecanoide usa o nível fixo da raça, como `QualityUtility`.

Não usa o produto especial `Smelted`: `Thing.SmeltProducts` ignora a eficiência que recebe (fixa 25%) e só devolve
material `smeltable`, deixando tecido e couro de fora.

### Pesquisa, trabalho e custo

- **Pesquisa por receita:** `RecipeDef.researchPrerequisite`, verificado em `RecipeDef.AvailableNow`.
- **Quem trabalha:** um `WorkGiverDef` com `WorkGiver_DoBill` e a bancada em `fixedBillGiverDefs`; sem ele ninguém
  usa a bancada.
- **Energia opcional:** `unpoweredWorkTableWorkSpeedFactor` 0,5, como no Core.
- **Custo em fatias:** proporção exata ao dano numa execução só exigiria Harmony: `RecipeDef.WorkAmountTotal(Thing)`
  não é virtual e só lê o material, e `RecipeWorker.GetIngredientCount(IngredientCount, Bill)` não recebe o item. Por
  isso o custo total acompanha o dano pelo número de execuções.
- **`workAmount` obrigatório:** sem ele, `WorkAmountForStuff` lê o `WorkToMake` de `products[0]`, e as receitas não
  têm produtos.

### Mapa dos arquivos

| Área | Código | Defs |
|---|---|---|
| Base no lugar | `RecipeWorker_InPlace` | — |
| Reparo | `RecipeWorker_Repair`, `RepairMath`, `RepairExtension` | bancada, receitas, work giver |
| Qualidade e melhoria | `QualityUpgrade`, `RecipeWorker_Improve`, `ImprovementCooldown`, `ImprovementSearchTier` | `MaintenanceBench_ImproveItem`, filtros especiais |
| Reciclagem | `RecipeWorker_Recycle`, `RecycleMath` | `MaintenanceBench_Recycle`, filtros especiais |
| Biocode | `RecipeWorker_RemoveBiocode` | `MaintenanceBench_RemoveBiocode` |
| Harmony | `HarmonyBootstrap`, `RangeLimit` e os dois `*_Patch` | — |
| Tradução | — | `Languages/PortugueseBrazilian/DefInjected/` (28 chaves) |

## Comportamento do jogo que afeta o mod

- **Armas não perdem HP por uso** (*leitura de código*, não testado em jogo): nenhum caminho do `Assembly-CSharp.dll`
  reduz o HP de arma equipada. Foram verificados toda atribuição a `HitPoints`, as chamadas de `TakeDamage`, o
  tracker de equipamento, os ataques (`Verb*`, que danificam só o alvo), o repasse de dano do pawn
  (`Pawn.PreApplyDamage` e `PostApplyDamage`; a roupa absorve dano e se desgasta todo dia em `Pawn_ApparelTracker`,
  a arma não), fogo e deterioração (só no mapa) e os comps de dano periódico (`CompDamageOnInterval`,
  `CompDissolution`, ausentes nas armas do jogo base e das DLCs). A arma perde HP no chão e pode vir danificada no
  saque (`PawnGenerator.PostProcessGeneratedGear`, por `gearHealthRange`). É a premissa da melhoria de item.
- **Skill 20 decai mesmo trabalhando** (`SkillRecord.Interval`): no nível 20 a skill perde 12 XP a cada 200 ticks
  (3600 por dia), e o trabalho em bill dá 0,1 XP por tick × paixão (0,35 sem paixão, 1 menor, 1,5 maior). Sem paixão
  o saldo é negativo, e o `Set skill` do debug põe só metade da barra (15000 de 30000 XP). Para validar ganho de XP,
  use colono com skill abaixo de 10.
- **Carregamento:** o 1.7 roda num runtime .NET 10 próprio, com `UnityEngine` redirecionado para `UnityStubs.dll`; o
  DLL `net10.0` do mod carrega sem ajustes.

## Compatibilidade

- **Lista grande de mods:** funciona com cerca de 80 mods, incluindo CommonSense, Vanilla Expanded e o próprio
  Tinkering Bench, sem erro do mod no log.
- **NiceBillTab (`Andromeda.NiceBillTab`):** a janela da bill de reciclagem mostra a faixa de HP como 0% a 100%, mas o
  filtro funciona (item acima de 25% não é reciclado). A causa da exibição não foi achada no NiceBillTab descompilado,
  que roda como "incompatible version" (feito para 1.6). Nada a corrigir no mod.
