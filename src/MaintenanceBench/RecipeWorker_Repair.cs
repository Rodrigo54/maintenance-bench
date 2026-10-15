using RimWorld;
using Verse;

namespace MaintenanceBench;

// Repara o item no lugar: cada execução devolve uma fatia do HP máximo e pode subir a qualidade em um nível.
public class RecipeWorker_Repair : RecipeWorker_InPlace
{
    // O reparo nunca leva a lendário; esse passo é exclusivo da melhoria de item.
    private const QualityCategory RepairQualityCap = QualityCategory.Masterwork;

    public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
    {
        RepairExtension extension = GetRepairExtension();
        foreach (Thing item in ingredients.Where(IsTargetItem))
        {
            int hitPointsBefore = item.HitPoints;
            item.HitPoints = RepairMath.GetRepairedHitPoints(hitPointsBefore, item.MaxHitPoints,
                extension.hitPointsFraction);
            float restoredFraction = RepairMath.GetRestoredFraction(hitPointsBefore, item.HitPoints,
                item.MaxHitPoints);
            TryUpgradeQuality(item, billDoer, extension.upgradeChanceMultiplier * restoredFraction);
        }
    }

    // Um sorteio com probabilidade multiplicador × fração de HP decide se a rolagem de upgrade acontece.
    private static void TryUpgradeQuality(Thing item, Pawn crafter, float rollChance)
    {
        CompQuality quality = item.TryGetComp<CompQuality>();
        if (quality == null || quality.Quality >= RepairQualityCap || !Rand.Chance(rollChance))
        {
            return;
        }

        QualityUpgrade.TryUpgrade(quality, crafter, rollCount: 1);
    }

    private RepairExtension GetRepairExtension()
    {
        return recipe.GetModExtension<RepairExtension>()
            ?? throw new InvalidOperationException($"A receita {recipe.defName} não declara RepairExtension.");
    }
}
