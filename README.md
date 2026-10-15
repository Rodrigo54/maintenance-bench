# Maintenance Bench

Mod para RimWorld 1.7. No jogo em português aparece como **Bancada de manutenção**.

Uma única bancada que cuida do equipamento da colônia do começo ao fim da vida útil: conserta armas e roupas, pode
melhorar a qualidade delas, recicla o que não vale mais consertar e remove a biocodificação de equipamento saqueado.

O item é alterado no lugar, sem ser destruído e recriado: cor, estilo, arte gravada e dados de outros mods continuam
como estavam.

## Requisitos

- RimWorld 1.7.
- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077), carregado antes deste mod.

As DLCs não são necessárias.

## A bancada

- Liberada pela pesquisa **Metalurgia**. Nenhuma pesquisa nova: todas as receitas usam pesquisas do jogo base.
- Custa 150 de aço e ocupa 3×1.
- Funciona sem energia a 50% da velocidade; ligada (120 W), a 100%.
- Aceita armas e roupas (incluindo armaduras). O trabalho é de **Artesanato**.

## Receitas

| Receita | Pesquisa | Custo por execução | Item aceito | Efeito |
|---|---|---|---|---|
| Reparo simples | Metalurgia | só tempo | danificado | devolve até 25% dos pontos de vida máximos |
| Reparo avançado | Usinagem | 1 componente industrial | danificado | devolve até 75% dos pontos de vida máximos |
| Melhoria de item | Usinagem | 2 componentes industriais | com 100% dos pontos de vida | tenta subir a qualidade |
| Reciclagem | Metalurgia | só tempo | danificado (a tarefa nasce com 25% ou menos) | devolve parte dos materiais |
| Remoção de biocodificação | Microeletrônica | 3 componentes industriais | biocodificado, exceto arma persona | remove a biocodificação |

### Reparo

Cada execução devolve uma fatia dos pontos de vida, e a tarefa continua pegando o mesmo item até ele ficar inteiro.
O reparo simples é gratuito e lento, para manutenção do dia a dia; o avançado consome componente e é mais rápido, para
equipamento valioso ou quando há pressa.

### Qualidade

Toda execução de reparo ou de melhoria pode subir a qualidade do item em **um nível**. A qualidade nunca piora.

- A chance vem da própria rolagem de fabricação do jogo para quem trabalha: quanto maior o Artesanato, maior a chance.
- No reparo, a chance é proporcional à vida devolvida naquela execução; o reparo avançado tem o dobro da chance do
  simples para a mesma vida restaurada. O reparo sobe no máximo até obra-prima.
- A melhoria de item tem uma chance bem maior e é o único caminho para lendário: o passo de obra-prima para lendário
  exige a inspiração de artista (que é consumida) ou o papel de especialista em produção.
- A melhoria trabalha primeiro nos itens de pior qualidade e deixa as obras-primas por último.

### Reciclagem

Devolve de 20% (Artesanato 0) a 75% (Artesanato 20) do custo de fabricação do item. Componentes nunca são devolvidos.
Ao contrário da fundição do jogo base, recicla também roupas de tecido e couro. Itens sem custo de fabricação (quase
todos saque raro) não aparecem na reciclagem, mas podem ser reparados.

### Remoção de biocodificação

Deixa utilizável a arma ou roupa biocodificada que veio de saque. Exige Intelectual 10 e Artesanato 3. Armas persona
ficam de fora, porque remover a biocodificação desfaria o vínculo com o dono.

## Compilar a partir do código

Requer o SDK do .NET 10 e o [Bun](https://bun.sh).

1. Copie `.env.example` para `.env` e aponte `RIMWORLD_PATH` para a instalação do jogo e `HARMONY_PATH` para a pasta
   `1.7/Assemblies` do mod Harmony.
2. `bun install` instala as dependências e o hook de commit.
3. `bun run build` compila a DLL em `mod/maintenance-bench/Assemblies`, `bun run test` roda os testes e
   `bun run copy` instala o mod em `RimWorld/Mods` (com o jogo fechado).

## Licença

[MIT](LICENSE.md).
