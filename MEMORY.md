# MEMORY.md — Maintenance Bench

Memória do projeto: conceito e regras de jogo fechadas, decisões técnicas com a evidência por trás de cada uma,
valores de ajuste, achados de teste, histórico do roteiro e o próximo passo. Comandos, ambiente de teste e convenções
estão no [`CLAUDE.md`](CLAUDE.md). As regras foram fechadas na entrevista de 13/10/2026; até 15/10/2026 este conteúdo
ficava no `IDEA.md`, dividido então entre `README.md`, `CLAUDE.md` e este arquivo.

## Estado atual

**Próximo passo (15/10/2026):** o roteiro de implementação está completo: as etapas 1 a 9, a ordem por qualidade da
melhoria e o reparo avançado nascendo com 0 a 25% estão validados em jogo. O repositório git foi criado em
15/10/2026. O que vier agora sai de "Fora de escopo por enquanto" (Workshop, arte própria, suporte ao 1.6) e depende
de decisão do Rodrigo.

## Conceito

Uma única bancada que cuida do equipamento da colônia do começo ao fim da vida útil: conserta armas e roupas,
pode melhorar a qualidade delas, recicla o que não vale mais consertar e remove o biocode de equipamento
saqueado.

A motivação veio de não existir, para as versões recentes do jogo, um mod que junte essas funções. O
*Tinkering Bench and Biohacking Station* re-rola qualidade e remove biocode, mas não repara; os mods de reparo
(MendAndRecycle, Repair Workbench, [Og] Repair Your Gear) não mexem em qualidade nem em biocode. O Maintenance
Bench é independente: não depende de nenhum deles e não reaproveita código deles.

## Regras de jogo

### A bancada

- **Pesquisa:** liberada por Metalurgia (`Smithing`, medieval, 700).
- **Energia opcional:** trabalha a 50% da velocidade sem energia e a 100% ligada.
- **Itens aceitos:** armas e roupas (incluindo armaduras) que tenham HP, filtradas pelas categorias `Weapons` e
  `Apparel`. O jogo trata alguns recursos como arma equipável (cerveja, madeira, presas e chifres), mas eles ficam
  nas categorias `Drugs`, `ResourcesRaw` e `ItemsMisc` e por isso não entram.
- **Pesquisas novas:** nenhuma. Todas as receitas usam pesquisas do jogo base.

### Receitas

| Receita | Pesquisa | Custo por execução | Item aceito | Efeito |
|---|---|---|---|---|
| Reparo simples | Metalurgia (`Smithing`) | só tempo | danificado | devolve até 25% do HP máximo; chance de upgrade 0,5x |
| Reparo avançado | Usinagem (`Machining`) | 1 componente industrial | danificado | devolve até 75% do HP máximo; chance de upgrade 1x |
| Melhoria de item | Usinagem (`Machining`) | 2 componentes industriais | 100% de HP | chance de upgrade 4x |
| Reciclagem | Metalurgia (`Smithing`) | só tempo | danificado; bill nasce com até 25% de HP | devolve parte dos materiais de fabricação |
| Remoção de biocode | Microeletrônica (`MicroelectronicsBasics`) | 3 componentes industriais | biocodificado, exceto arma persona | remove o biocode |

Observação sobre a árvore do jogo base: Microeletrônica exige Eletricidade, mas não Usinagem. Dá para liberar
a remoção de biocode antes do reparo avançado, e isso é aceito.

### Reparo

- **Fatia por execução:** cada execução devolve uma fatia do HP máximo (25% no simples, 75% no avançado). O
  "até" cobre a última execução, que só completa o que falta.
- **Repetição:** a bill continua pegando o mesmo item enquanto ele estiver danificado. Um item com 10% de HP
  precisa de quatro execuções simples ou duas avançadas.
- **Papel de cada receita:** o simples é gratuito e lento, para manutenção barata; o avançado é rápido e
  consome componente, para equipamento valioso ou quando há pressa.

### Chance de upgrade

Toda execução de reparo ou de melhoria pode subir a qualidade do item em **um nível**. A qualidade nunca piora.

- **Reparo:** chance = *p* × multiplicador × fração do HP máximo devolvida naquela execução.
- **Melhoria de item:** chance = *p* × 4 (não restaura HP, então não há fração). Era 2x até 14/10/2026; subiu
  porque, com Crafting 17, excelente para obra-prima levava em média 11 execuções (22 componentes). Com 4x são
  cerca de 6 (ver a tabela em Ajuste da melhoria).
- **Chance base *p*:** a chance de uma rolagem normal de fabricação daquele pawn dar uma qualidade acima da
  que o item já tem. Usa a curva de qualidade do jogo base, sem tabela própria.

Efeito prático do multiplicador, para um item reparado de 0% a 100%: o reparo simples soma 0,5 *p* (quatro
execuções de 25%) e o avançado soma 1 *p* (uma de 75% e uma de 25%). O multiplicador equivale a "chance por
barra de HP restaurada". A fração de HP também impede o abuso de reparar todo dia o desgaste mínimo de uma
roupa para ganhar uma tentativa cheia.

### Tetos de qualidade

- **Pelo reparo:** a qualidade sobe no máximo até obra-prima.
- **Pela melhoria de item:** sobe até obra-prima para qualquer crafter. O passo de obra-prima para lendário só
  pode acontecer se quem trabalha estiver com a inspiração de criatividade (que é consumida) ou tiver o papel
  de especialista em produção.

### Por que existe a melhoria de item

Roupas e armaduras vestidas perdem HP todo dia e equipamento saqueado chega danificado, então têm chance de
upgrade pelo reparo com frequência. Armas da colônia, em geral, ficam em 100% de HP e quase nunca passariam por
reparo. A melhoria de item cobre esse caso e é o único caminho para lendário.

### Reciclagem

- **Elegibilidade:** item danificado (até 99% de HP). A bill nasce aceitando só 25% de HP ou menos, e o jogador
  pode ampliar até 99% (decisão de 14/10/2026; antes os 25% eram limite fixo).
- **Retorno:** uma parte do custo de fabricação, conforme a skill de Crafting de quem recicla, de 20% com
  Crafting 0 até 75% com Crafting 20 (etapa 9; antes 15% a 40%).
- **Componentes:** nunca são devolvidos.
- **Item sem custo de fabricação:** não aparece como ingrediente, por um filtro especial próprio. São 44 armas e
  roupas no jogo base e nas DLCs, quase todas saque valioso (zeushammer, equipamento de eltex, lança-foguetes); elas
  continuam podendo ser reparadas.
- **Diferença para a fundição do jogo base:** cobre também roupas de tecido e couro, e não depende de
  fundição elétrica.

### Remoção de biocode

- **Itens:** armas e roupas biocodificadas.
- **Arma persona fica de fora:** no código do jogo ela é um tipo de biocode, e removê-lo desfaz o vínculo com
  o pawn.
- **Requisito:** exige skill de Intellectual, como no Tinkering Bench original (lá, Intellectual 10 e
  Crafting 3).

## Decisões técnicas

Fonte da evidência: descompilação do `Assembly-CSharp.dll` da instalação local (versão `1.7.4977 rev926`) com o
ILSpy, e leitura do código-fonte do MendAndRecycle e do Tinkering Bench. As assinaturas citadas abaixo foram
reconferidas com o `ilspycmd` na `rev1248` e continuam as mesmas. O que está marcado como hipótese não foi testado
em jogo.

### Harmony: interface da bill e ordem da melhoria

Até 14/10/2026 o mod era todo sem Harmony. As exceções, decididas pelo Rodrigo, são a ordem por qualidade da
melhoria (ver Ajuste da melhoria) e um postfix em
`ThingFilterUI.DoThingFilterConfigWindow`: o controle de HP e o de qualidade da janela da bill vão sempre de 0 a
100% e de péssimo a lendário (`DrawHitPointsFilterConfig` desenha de 0 a 1 e não recebe o filtro fixo), então o
jogador conseguia escolher uma faixa que o filtro fixo barrava. O postfix devolve a faixa da bill ao limite do
filtro fixo, só quando o `parentFilter` é o filtro fixo de uma receita do mod (`RangeLimit.Clamp`, testado). As
alternativas sem Harmony foram descartadas: o jogo cria a bill com classe fixa (`BillUtility.MakeNewBill`), então
não dá para trocar a janela, e corrigir a faixa a cada quadro pela bancada seria gambiarra.

- **Dependência:** o mod `brrainz.harmony` (Workshop 2009463077, que traz o `0Harmony.dll` do 1.7), declarada em
  `modDependencies` e `loadAfter` do `About.xml`. O build referencia esse DLL por `HARMONY_PATH` no `.env`, sem
  copiá-lo para `Assemblies`.
- **Validado em jogo (14/10/2026):** os controles param no limite das receitas do mod.
- **O resto continua sem Harmony:** reparo, melhoria, reciclagem e filtros usam só pontos de extensão do jogo.

### Alterar o próprio item, sem Harmony

O item não é destruído e recriado: a receita muda só o que promete (HP, qualidade ou biocode). Isso preserva
cor, estilo, arte gravada, traits e dados de outros mods. O Tinkering Bench recria o item e perde tudo isso.

- **Como:** receita sem produtos, com um `RecipeWorker` próprio que sobrescreve `ConsumeIngredient` para não
  destruir o item e faz a alteração em `Notify_IterationCompleted`.
- **Evidência:** em `Toils_Recipe.FinishRecipeAndStartStoringProduct`, o jogo monta os produtos, chama
  `ConsumeIngredients` (que delega a `recipe.Worker.ConsumeIngredient`) e depois
  `bill.Notify_IterationCompleted`. `Bill_Production.Notify_IterationCompleted` repassa para
  `recipe.Worker.Notify_IterationCompleted(billDoer, ingredients)`. Com a lista de produtos vazia, o job
  termina com sucesso.
- **Consequência aceita:** o item fica em cima da bancada ao terminar. O modo "levar para o estoque" da bill
  não se aplica; o item segue pelo transporte normal depois.
- **Confirmado em jogo na etapa 3:** o fluxo funciona como o código sugeria (ver Achados de teste). O MendAndRecycle
  faz reparo no lugar por outro caminho (job próprio e um patch Harmony).

### Chamadas usadas para cada alteração

- **Reparo:** ajuste direto de `HitPoints`.
- **Qualidade:** `CompQuality.SetQuality(q, null)`. Sem contexto de arte, o método não reinicializa a arte.
- **Biocode:** `CompBiocodable.UnCode()`, que é público.
- **Arma persona:** `CompBladelinkWeapon` herda de `CompBiocodable` e seu `UnCode()` desfaz o vínculo. O filtro
  do jogo já a exclui: `SpecialThingFilterWorker_NonBiocodedWeapons` considera a arma persona como "não
  biocodificada" e `SpecialThingFilterWorker_BiocodedWeapons` a recusa. Proibir `AllowNonBiocodedWeapons` deixa
  passar só arma biocodificada comum, sem filtro próprio.
- **Rolagem de qualidade:** `QualityUtility.GenerateQualityCreatedByPawn(pawn, skill, consumeInspiration)`.
  Esse método já aplica o bônus do papel de produção e, se pedido, consome a inspiração. A rolagem base vai no
  máximo até obra-prima; só a inspiração (+2 níveis) ou o papel de produção levam a lendário.

### Como implementar a chance (sugestão)

- **Chance base *p*:** em vez de calcular a probabilidade, fazer uma rolagem real de fabricação e considerar
  sucesso se o resultado for maior que a qualidade atual.
- **Multiplicador × fração abaixo de 1:** um sorteio com essa probabilidade decide se a rolagem acontece.
- **Multiplicador 4x da melhoria:** quatro rolagens independentes; basta uma dar certo. O resultado fica um
  pouco abaixo de 4 *p* quando *p* é alto, o que é aceitável.
- **Inspiração:** as rolagens comuns não devem consumir a inspiração; ela só é consumida no passo de
  obra-prima para lendário.
- **Implementado assim (`QualityUpgrade`):** a regra do lendário sai da própria rolagem do jogo, que sem
  inspiração nem papel de produção vai no máximo até obra-prima. Para item obra-prima, a rolagem é feita com
  `consumeInspiration: true`; como no jogo base, a inspiração é gasta na tentativa, mesmo que ela falhe, e só a
  primeira das quatro rolagens da melhoria a usa. O teto do reparo em obra-prima é checado antes do sorteio, então
  o reparo nunca chega a rolar para item obra-prima.

### Filtros de item sem patch

O filtro de ingredientes do jogo (`ThingFilter.Allows`) já limita por:

- **Faixa de HP** (`allowedHitPointsPercents`): menos de 100% para os reparos e a reciclagem, exatamente 100%
  para a melhoria.
- **Faixa de qualidade** (`allowedQualities`): para não oferecer upgrade a item que já está no teto.
- **Biocode** (`specialFiltersToDisallow`): os filtros especiais do Core `AllowNonBiocodedWeapons` e
  `AllowNonBiocodedApparel` deixam passar só item biocodificado.

**Proibir filtro especial no XML:** usar `specialFiltersToDisallow`, como o jogo base (cremação). O
`disallowedSpecialFilters` é desfeito para filtros configuráveis: em `ThingFilter.ResolveReferences` as categorias são
liberadas primeiro, e `SetAllow(categoria)` libera junto os filtros especiais configuráveis dela; o
`specialFiltersToDisallow` é aplicado depois. Achado em 14/10/2026, quando a remoção de biocode aceitou uma mochila
tóxica (não biocodificável) e o worker deu erro depois de consumir os componentes. Os filtros próprios do mod
(`configurable` falso) não eram afetados, mas as receitas usam `specialFiltersToDisallow` em todos os casos.

Detalhe: o filtro arredonda o HP para centésimos, então um item com 99,5% ou mais conta como 100%.

**Filtro fixo e filtro da bill.** Cada receita tem dois filtros que valem juntos: o `fixedIngredientFilter`, limite
que o jogador não vê nem muda, e o filtro da bill, que o jogador edita na janela da bill (faixa de HP e de
qualidade). A bill nova nasce com o `defaultIngredientFilter` da receita; sem ele declarado, o jogo copia o fixo
(`RecipeDef.ResolveReferences`). As quatro receitas declaram o padrão explicitamente, com as mesmas faixas do
fixo, exceto a reciclagem e o reparo avançado, que nascem com 0 a 25% dentro do limite de 0 a 99%. No jogo base, o controle de HP da bill
vai de 0 a 100% sem respeitar o limite fixo: se o jogador ampliar além dele, a tela aceita e o filtro fixo continua
barrando.
Só vale para bill nova; bill já criada mantém o filtro que tinha. O controle passar do limite foi corrigido com
Harmony (ver Harmony: interface da bill e ordem da melhoria).

Exceção: o filtro de qualidade deixa passar item **sem** qualidade (`ThingFilter.Allows` só checa quem tem
`CompQuality`). A melhoria de item proíbe esses itens com um filtro especial próprio,
`SpecialThingFilterWorker_WithoutQuality`, em dois defs (`Weapons` e `Apparel`, como os de biocode do Core),
com `configurable` falso.

### Pesquisa e energia

- **Pesquisa por receita:** `RecipeDef.researchPrerequisite`, verificado em `RecipeDef.AvailableNow`.
- **Quem trabalha na bancada:** um `WorkGiverDef` com `WorkGiver_DoBill` e a bancada em `fixedBillGiverDefs`. Sem
  ele nenhum colono usa a bancada, mesmo com bill e item disponíveis. Tipo de trabalho: Artesanato (`Crafting`),
  como no Tinkering Bench, porque o reparo usa a skill de Crafting e cobre armas e roupas.
- **Energia opcional:** `unpoweredWorkTableWorkSpeedFactor` nas propriedades da construção. O Core já usa esse
  campo com valor 0,5 (`Data/Core/Defs/ThingDefs_Buildings/Buildings_Production.xml`).

### Custo variável conforme o dano

Proporção exata numa única operação não sai sem Harmony: o tempo de trabalho é fixo por receita
(`RecipeDef.WorkAmountTotal(Thing)` não é virtual e só usa o item para ler o material) e
`RecipeWorker.GetIngredientCount(IngredientCount, Bill)` não recebe o item. Por isso o reparo é em fatias: o custo
total acompanha o dano pelo número de execuções.

Toda receita do mod precisa declarar `workAmount`: sem ele, `WorkAmountForStuff` lê o `WorkToMake` de
`products[0]`, e a lista de produtos é vazia.

### Ajuste da melhoria (14/10/2026)

Mudanças pedidas pelo Rodrigo depois do teste da etapa 5.

**Chance.** Simulação da rolagem do jogo (`QualityUtility`, 400 mil amostras): com Crafting 17 uma fabricação sai
obra-prima em 4,8% das vezes, e com 20 em 9,5%. Chance de excelente para obra-prima por execução:

| Crafting | 2 rolagens | 4 rolagens |
|---|---|---|
| 14 | 4,2% | 8,1% |
| 17 | 9,3% | 17,9% |
| 20 | 18,1% | 33,0% |

**Escolha do item.** O jogo escolhe o ingrediente mais próximo da bancada
(`WorkGiver_DoBill.TryFindBestIngredientsHelper`), e o item melhorado fica em cima dela, então a bill voltava
sempre ao mesmo item. Correção sem Harmony: o item tentado entra numa espera de uma hora de jogo
(`ImprovementCooldown`), e um filtro especial (`SpecialThingFilterWorker_RecentlyImproved`) o tira da bill
enquanto ela dura. Com vários itens a bill passa por todos; com um item só, ela espera. A espera fica só em
memória, para não deixar entrada do mod no save; ao carregar, ela recomeça zerada. Priorizar o item de pior
qualidade exigiria Harmony e ficou de fora até o teste da etapa 7 (ver Ordem por qualidade).

**Ordem por qualidade (14/10/2026).** No teste, a bill alternava entre um vacsuit obra-prima (sem chance de virar
lendário) e os outros itens. Regra do Rodrigo: melhorar primeiro os itens de pior qualidade e deixar as obras-primas
por último. A busca do jogo para assim que acha um conjunto completo perto da bancada, então ordenar o que ela achou
não basta. Patch Harmony (prefixo em `WorkGiver_DoBill.TryFindBestBillIngredients`, só para bills da melhoria): roda
a busca do jogo uma vez por nível de qualidade, do pior para o melhor (a faixa vem do filtro fixo), e fica com o
primeiro nível que achar item. Um filtro especial (`SpecialThingFilterWorker_OutsideImprovementTier`, com o nível
atual em `ImprovementSearchTier`) tira da busca os outros níveis. Cada rodada é a busca normal, com raio, filtro da
bill, itens proibidos e alcance. Se nenhum nível achar item, uma busca sem restrição preenche os ingredientes que
faltam. A espera de uma hora continua, para alternar entre itens do mesmo nível. *Validado em jogo (15/10/2026):*
péssimo, depois bom, obra-prima por último; o rodízio no mesmo nível e a falta de componentes seguem como antes.

**Salto de níveis: avaliado e rejeitado.** O Tinkering Bench faz uma rolagem e fica com a maior entre a qualidade
atual e a rolada, podendo pular vários níveis. Execuções esperadas até obra-prima com Crafting 17 (cálculo
analítico sobre a rolagem do jogo):

| Regra | De péssimo | De normal | De excelente |
|---|---|---|---|
| +1 nível, 4 rolagens (adotada) | 9,8 | 7,8 | 5,7 |
| Salto, 1 rolagem (Tinkering) | 21,1 | 21,1 | 21,1 |
| Salto, 4 rolagens | 5,7 | 5,7 | 5,7 |

Com salto, o passo para obra-prima não muda (depende de uma rolagem de obra-prima, seja qual for o ponto de
partida); só a subida dos níveis baixos acelera, e um item péssimo viraria excelente em uma execução em 72% das
vezes. O Rodrigo manteve o +1 nível (14/10/2026): as 4 rolagens já resolvem o passo para obra-prima, e a subida
gradual mantém o peso dos componentes.

### Reciclagem

A receita também não tem produtos fixos. O `RecipeWorker` calcula o retorno a partir do custo de fabricação do
item, coloca os materiais perto da bancada e então destrói o item. Nada disso exige Harmony.

- **Por que não o produto especial `Smelted` do jogo:** `Thing.SmeltProducts` ignora a eficiência que recebe (fixa
  25%) e só devolve material `smeltable`, o que deixaria tecido e couro de fora.
- **Implementado assim (`RecipeWorker_Recycle`):** o `ConsumeIngredient` padrão destrói o item e, em
  `Notify_IterationCompleted`, o worker lê `CostListAdjusted()` do item destruído (def e material continuam
  legíveis), pula o que é `intricate`, arredonda com `GenMath.RoundRandom` e põe os materiais com
  `GenPlace.TryPlaceThing` perto do pawn, que divide pilha acima do limite. Mecanoide usa o nível fixo da raça,
  como `QualityUtility`.
- **Filtro de item sem custo (`SpecialThingFilterWorker_WithoutReturnableCost`):** casa com item sem custo de
  fabricação e também com item cujo custo é só componente, que não teria nada a devolver.

### Versão e build

- **Alvo:** só RimWorld 1.7, nativo em `net10.0`, compilado contra o `Assembly-CSharp.dll` da instalação.
- **Contexto:** o 1.7 roda em um runtime .NET 10 próprio, com `UnityEngine` redirecionado para
  `UnityStubs.dll`. Mods nativos do 1.7 (Harmony, Vanilla Expanded Framework) já compilam para `net10.0`.
- **Requisito:** SDK do .NET 10 (instalado: `10.0.402`).
- **Evidência de carregamento:** entre os mods do Workshop instalados, o Harmony traz DLLs `net10.0` para o 1.7, e o
  Smarter Construction traz um DLL `net472` na pasta do 1.7.
- **Idioma:** defs em inglês, com tradução PT-BR na pasta de idioma desde a primeira versão.
- **Textura:** a bancada reaproveita a arte de uma mesa do jogo base na primeira versão.

### Decisões de validação (13/10/2026)

- **Testes automatizados:** só a lógica pura, em classes sem dependência de tipos do jogo: a fatia de HP
  devolvida, a probabilidade de a rolagem acontecer (multiplicador × fração de HP) e a curva de retorno da
  reciclagem. O que toca o jogo é validado em jogo.
- **Validação da chance de upgrade em jogo:** três cenários extremos no modo dev: item péssimo com crafter de skill
  20 (sobe quase sempre), obra-prima sem inspiração (nunca vira lendário) e obra-prima com inspiração de
  criatividade (pode virar lendário e a inspiração é consumida). A proporção fica com os testes unitários; o mod
  não leva código de debug.
- **Se o fluxo no lugar falhar na etapa 3:** a implementação para e a decisão volta para o Rodrigo, com a
  evidência e as alternativas.
- **Valores de ajuste fino:** uma tabela provisória, com referência no jogo base e no Tinkering Bench, é aprovada em
  lote antes da etapa 3; a etapa 9 só ajusta.

## Valores, achados e escopo

### Valores de ajuste (aprovados em 14/10/2026, revistos e validados em jogo na etapa 9 em 15/10/2026)

Os valores marcados com *etapa 9* foram mudados pelo Rodrigo depois dos testes; o valor anterior fica entre parênteses.

| Item | Valor | Referência |
|---|---|---|
| Custo da bancada | 150 aço, sem componente | Forja a combustível: 100 aço; mesa de usinagem: 150 aço + 5 componentes. Sem componente porque a bancada sai em Metalurgia, antes de a colônia fabricar componentes. |
| Construção | 3000 de trabalho, 3×1 | Forjas e mesa de usinagem |
| Energia ligada | 120 W (*etapa 9*; era 210 W) | Forja elétrica: 210 W |
| Textura | mesa de usinagem (`TableMachining`) | — |
| Reparo simples | 2500 por execução (*etapa 9*; era 1500), Crafting 3 | Fundir arma: 1600; revólver pede Crafting 3. Fica mais lento que o avançado, que é o papel do simples (gratuito e lento). |
| Reparo avançado | 2000 por execução, Crafting 5 | Espada longa pede Crafting 5 |
| Melhoria de item | 3000 (*etapa 9*; era 6000), Crafting 6 | Tinkering Bench: 6000 (roupa) e 12000 (arma); rifle de assalto pede Crafting 6 |
| Reciclagem | 2500 (*etapa 9*; era 1600), sem skill | Fundição do jogo: 1600 |
| Curva da reciclagem | 20% + 2,75% × Crafting (20% a 75%) (*etapa 9*; era 15% a 40%) | A fundição do jogo devolve 25% fixos (`Thing.SmeltProducts`, que usa `CostListAdjusted`, ignora materiais `intricate` e arredonda com `GenMath.RoundRandom`) |
| Remoção de biocode | 10000, Intellectual 10 e Crafting 3 | Tinkering Bench: 15000, com as mesmas skills |
| Ícones das receitas (*etapa 9*; antes placeholder) | `uiIconThing`: reparo simples e reciclagem `Steel`; reparo avançado e melhoria `ComponentIndustrial`; remoção de biocode `ComponentSpacer` | Sem produto, a receita não tem ícone e o menu "Adicionar tarefa" mostra um placeholder (`FloatMenuOption`, leitura de código). O jogo base faz igual no abate (`Meat_Muffalo`) e no desmonte de mecanoide (`Steel`). |

### Achados de teste

- **Carregamento do DLL:** *confirmado em jogo* — o DLL `net10.0` carrega no 1.7 sem ajustes.
- **Fluxo no lugar:** *confirmado em jogo* — o reparo simples devolve 25% do HP por execução, a bill termina
  normalmente e para de pegar o item quando ele chega a 100%. Cor e estilo se mantêm e o pawn ganha XP de
  Crafting.
- **Convivência com outros mods:** o reparo funcionou num save do Rodrigo com a lista completa dele (cerca de 80
  mods, incluindo CommonSense, Vanilla Expanded e o próprio Tinkering Bench), sem erro do mod no log.
- **NiceBillTab (`Andromeda.NiceBillTab`):** no save do Rodrigo, a janela da bill de reciclagem mostra a faixa de HP
  como 0% a 100%, mas o filtro funciona (confirmado em jogo: item acima de 25% não é reciclado). Pelo código do
  jogo a bill nova copia a faixa da receita (0 a 25%); a causa da exibição não foi achada no NiceBillTab
  descompilado, que nessa lista roda como "incompatible version" (feito para 1.6). Nada a corrigir no mod.
- **Entre execuções de reparo:** *confirmado em jogo* — o pawn deixa o item na bancada e emenda a próxima
  execução, sem vai e vem.

### Resolvidas na reconferência da `rev1248`

- **Reciclagem de item sem custo de fabricação:** o item não aparece como ingrediente (ver Reciclagem).
- **Armas não perdem HP por uso:** nenhum caminho do `Assembly-CSharp.dll` encontrado reduz o HP de arma equipada
  (busca estática no código descompilado, não testada em jogo). Foram verificados:
  - toda atribuição direta a `HitPoints`, todas as chamadas de `TakeDamage` e o tracker de equipamento;
  - os ataques (`Verb*`) danificam só o alvo;
  - o dano que o pawn recebe não é repassado à arma (`Pawn.PreApplyDamage` e `PostApplyDamage`), ao contrário da
    roupa, que absorve parte dele e se desgasta todo dia em `Pawn_ApparelTracker`;
  - fogo e deterioração só atingem coisas no mapa, e os componentes de dano periódico (`CompDamageOnInterval`,
    `CompDissolution`) não aparecem em nenhuma arma do jogo base nem das DLCs.

  A arma perde HP quando está no chão (deterioração ao ar livre, fogo, explosões) e pode já vir danificada no saque:
  `PawnGenerator.PostProcessGeneratedGear` gera o equipamento de pawns inimigos com HP sorteado por
  `gearHealthRange`. A premissa da melhoria de item se mantém para as armas em uso pela colônia.
- **Exclusão da arma persona:** pela leitura dos workers de filtro, o filtro de biocode do jogo já cobre (ver
  Chamadas usadas para cada alteração). Confirmado em jogo na etapa 7: a arma persona não
  aparece como ingrediente.

### Fora de escopo por enquanto

- Arte própria da bancada.
- Suporte ao RimWorld 1.6.
- Publicação na Workshop.
- Remoção de vínculo de arma persona.

## Histórico do roteiro

Cada etapa só começou depois que a anterior foi validada em jogo.

**XP no nível 20 não é bug do mod:** no teste da etapa 5 o crafter caiu de 20 para 19,89. O mod não mexe em skill;
é o decaimento do jogo (`SkillRecord.Interval`): no nível 20 a skill perde 12 XP a cada 200 ticks (3600 por dia),
e o trabalho em bill dá 0,1 XP por tick × paixão (0,35 sem paixão, 1 menor, 1,5 maior). Sem paixão o saldo é
negativo até trabalhando. O `Set skill` do debug põe só metade da barra (15000 de 30000 XP), então a queda para 19
vem em poucos dias. Para validar ganho de XP, usar colono com skill abaixo de 10, que não decai.

1. **Instalar o SDK do .NET 10.** Valida: `dotnet --list-sdks` mostra uma versão 10. *Feito: `10.0.402`.*
2. **Esqueleto do mod e script de build.** A estrutura descrita no `CLAUDE.md`, com `About.xml`,
   projeto C# vazio em `net10.0`, projeto de teste e `scripts/copy.ps1`. Valida: `bun run build`, `test` e `copy`
   passam, o mod aparece na lista do jogo e carrega sem erro no log. *Validada em jogo.*
3. **Bancada e reparo simples.** Def da bancada, energia opcional e a primeira receita no lugar, com a tabela de
   valores provisórios já aprovada. Valida: o item recupera 25% por execução, mantém cor e arte, a bill para
   quando ele chega a 100% e o pawn ganha XP de Crafting. *Validada em jogo.* Arquivos: `RecipeWorker_Repair`,
   `RepairMath`, `RepairExtension`, os defs da bancada, das receitas e do work giver.
4. **Reparo avançado e chance de upgrade.** Segunda receita, consumo de componente e a fórmula da chance com
   teto em obra-prima. Valida: fatia de 75%, componente consumido, item péssimo com skill 20 subindo quase
   sempre e nenhum reparo passando de obra-prima. *Implementada; validados em jogo a fatia de 75%, o componente
   consumido, o upgrade frequente com skill 20, o XP e o teto (obra-prima com `Inspired_Creativity` continua
   obra-prima e a inspiração fica). Validada em jogo.*
5. **Melhoria de item.** Receita para item com 100% de HP, chance 4x e a regra do lendário. Valida: só aceita
   item cheio, obra-prima sem inspiração nunca vira lendário, e com inspiração pode virar e a inspiração some.
   *Validada em jogo.* Arquivos: `RecipeWorker_InPlace` (base extraída do reparo),
   `RecipeWorker_Improve`, `QualityUpgrade`, `SpecialThingFilterWorker_WithoutQuality`, a receita
   `MaintenanceBench_ImproveItem` e os defs de filtro especial.
6. **Reciclagem.** Receita para item danificado (padrão da bill: 25% de HP ou menos) e filtro de item sem custo. Valida: retorno conforme
   a skill, sem componentes, item destruído ao final e item sem custo fora da lista de ingredientes.
   *Validada em jogo.* Arquivos: `RecipeWorker_Recycle`, `RecycleMath`,
   `SpecialThingFilterWorker_WithoutReturnableCost`, a receita `MaintenanceBench_Recycle` e os defs de filtro.
7. **Remoção de biocode.** Receita com o filtro de biocode do jogo. Valida: item saqueado fica utilizável e
   arma persona não aparece como ingrediente. *Validada em jogo.* Arquivos:
   `RecipeWorker_RemoveBiocode` e a receita `MaintenanceBench_RemoveBiocode`. Primeiro teste em jogo achou o bug de
   `disallowedSpecialFilters` (ver Filtros de item sem patch); corrigido e validado em jogo. O XP vai para Intellectual
   (`workSkill`), e o worker recusa arma persona com erro mesmo que algo fure o filtro, porque o `UnCode` dela
   desfaria o vínculo.
8. **Tradução PT-BR.** Valida: todos os textos do mod aparecem em português com o jogo em PT-BR. *Validada em jogo
   (15/10/2026).* Arquivos em `Languages/PortugueseBrazilian/DefInjected/` (bancada, receitas, work giver e
   filtros especiais; as 28 chaves conferidas por script contra os defs). Termos da tradução oficial do jogo: bill é
   "tarefa", HP é "pontos de vida", biocode é "biocodificação" (por isso a receita é "remoção de biocodificação"), a
   inspiração de criatividade é "artista". O `About.xml` não tem tradução no jogo e fica em inglês. As mensagens de
   erro do C# já estavam em português; o mod não tem textos `Keyed`.
9. **Ajuste fino.** Fechar os valores pendentes com base no que os testes mostrarem. *Validada em jogo
   (15/10/2026):* ver os valores marcados com *etapa 9* em Valores de ajuste. Arquivos: `RecycleMath` e seu
   teste, os defs da bancada e das receitas.
