using RimWorld;
using Verse;

namespace MaintenanceBench;

// Base das receitas que alteram o próprio item em vez de destruí-lo e recriá-lo. Manter o mesmo objeto preserva
// cor, estilo, arte gravada e dados de outros mods. Os demais ingredientes (componentes) são consumidos
// normalmente.
public abstract class RecipeWorker_InPlace : RecipeWorker
{
    public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
    {
        if (IsTargetItem(ingredient))
        {
            return;
        }

        base.ConsumeIngredient(ingredient, recipe, map);
    }

    protected static bool IsTargetItem(Thing thing)
    {
        return thing.def.IsWithinCategory(ThingCategoryDefOf.Weapons)
            || thing.def.IsWithinCategory(ThingCategoryDefOf.Apparel);
    }
}
