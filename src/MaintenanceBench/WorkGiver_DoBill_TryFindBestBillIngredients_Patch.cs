using HarmonyLib;
using RimWorld;
using Verse;

namespace MaintenanceBench;

// O jogo escolhe o ingrediente mais próximo e para de procurar assim que acha um conjunto completo perto da bancada.
// Na melhoria, o item de pior qualidade deve vir antes e a obra-prima por último. Para isso a busca do jogo roda uma
// vez por nível de qualidade, do pior para o melhor, e fica com o primeiro nível que achar item. Cada rodada é a
// busca normal (raio, filtro da bill, itens proibidos, alcance), só restrita a um nível.
[HarmonyPatch(typeof(WorkGiver_DoBill), "TryFindBestBillIngredients")]
public static class WorkGiver_DoBill_TryFindBestBillIngredients_Patch
{
    private static readonly Func<Bill, Pawn, Thing, List<ThingCount>, List<IngredientCount>, bool>
        TryFindBestBillIngredients = AccessTools.MethodDelegate<
            Func<Bill, Pawn, Thing, List<ThingCount>, List<IngredientCount>, bool>>(
            AccessTools.Method(typeof(WorkGiver_DoBill), "TryFindBestBillIngredients"));

    // As rodadas chamam o próprio método patcheado; durante elas o prefixo deixa a busca do jogo seguir.
    private static bool isSearchingByTier;

    public static bool Prefix(Bill bill, Pawn pawn, Thing billGiver, List<ThingCount> chosen,
        List<IngredientCount> missingIngredients, ref bool __result)
    {
        if (isSearchingByTier || bill.recipe.Worker is not RecipeWorker_Improve)
        {
            return true;
        }

        isSearchingByTier = true;
        try
        {
            __result = SearchByQualityTier(bill.recipe.fixedIngredientFilter.AllowedQualityLevels,
                () => TryFindBestBillIngredients(bill, pawn, billGiver, chosen, missingIngredients));
        }
        finally
        {
            isSearchingByTier = false;
            ImprovementSearchTier.Current = null;
        }

        return false;
    }

    private static bool SearchByQualityTier(QualityRange tiers, Func<bool> search)
    {
        for (QualityCategory tier = tiers.min; tier <= tiers.max; tier++)
        {
            ImprovementSearchTier.Current = tier;
            if (search())
            {
                return true;
            }
        }

        // Nenhum nível achou item: uma busca sem restrição preenche os ingredientes que faltam, como o jogo faria.
        ImprovementSearchTier.Current = null;
        return search();
    }
}
