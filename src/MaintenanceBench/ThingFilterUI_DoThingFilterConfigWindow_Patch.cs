using HarmonyLib;
using RimWorld;
using Verse;

namespace MaintenanceBench;

// Os controles de HP e de qualidade da janela da bill vão sempre de 0 a 100% e de péssimo a lendário, sem respeitar o
// filtro fixo da receita. O filtro fixo já barra o item, mas a tela mostraria uma faixa que não vale. Nas receitas
// do mod, a faixa da bill volta ao limite da receita logo depois de desenhada.
[HarmonyPatch(typeof(ThingFilterUI), nameof(ThingFilterUI.DoThingFilterConfigWindow))]
public static class ThingFilterUI_DoThingFilterConfigWindow_Patch
{
    // Filtros fixos das receitas do mod. A janela da bill passa o filtro fixo da receita como parentFilter.
    private static readonly HashSet<ThingFilter> ModFixedFilters = DefDatabase<RecipeDef>.AllDefs
        .Where(recipe => recipe.workerClass?.Assembly == typeof(RecipeWorker_InPlace).Assembly)
        .Select(recipe => recipe.fixedIngredientFilter)
        .ToHashSet();

    public static void Postfix(ThingFilter filter, ThingFilter parentFilter)
    {
        if (parentFilter == null || !ModFixedFilters.Contains(parentFilter))
        {
            return;
        }

        FloatRange hitPoints = filter.AllowedHitPointsPercents;
        FloatRange hitPointsLimit = parentFilter.AllowedHitPointsPercents;
        (float minHitPoints, float maxHitPoints) = RangeLimit.Clamp(hitPoints.min, hitPoints.max,
            hitPointsLimit.min, hitPointsLimit.max);
        filter.AllowedHitPointsPercents = new FloatRange(minHitPoints, maxHitPoints);

        QualityRange qualities = filter.AllowedQualityLevels;
        QualityRange qualitiesLimit = parentFilter.AllowedQualityLevels;
        (int minQuality, int maxQuality) = RangeLimit.Clamp((int)qualities.min, (int)qualities.max,
            (int)qualitiesLimit.min, (int)qualitiesLimit.max);
        filter.AllowedQualityLevels = new QualityRange((QualityCategory)minQuality, (QualityCategory)maxQuality);
    }
}
