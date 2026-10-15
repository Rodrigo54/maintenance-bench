using RimWorld;
using Verse;

namespace MaintenanceBench;

// Melhoria de item no lugar: não restaura HP, só tenta subir a qualidade em um nível. É o único caminho para
// lendário, e esse passo depende de inspiração de criatividade ou do papel de produção (ver QualityUpgrade).
public class RecipeWorker_Improve : RecipeWorker_InPlace
{
    // Chance 4x da chance base: quatro rolagens independentes. Fica um pouco abaixo de 4x quando a base é alta.
    private const int UpgradeRollCount = 4;

    public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
    {
        foreach (Thing item in ingredients.Where(IsTargetItem))
        {
            CompQuality quality = item.TryGetComp<CompQuality>()
                ?? throw new InvalidOperationException(
                    $"{item.def.defName} não tem qualidade e não deveria passar pelo filtro de {recipe.defName}.");
            QualityUpgrade.TryUpgrade(quality, billDoer, UpgradeRollCount);
            ImprovementCooldown.RecordAttempt(item);
        }
    }
}
