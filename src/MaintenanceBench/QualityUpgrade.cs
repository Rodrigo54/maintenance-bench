using RimWorld;
using Verse;

namespace MaintenanceBench;

// A chance base de upgrade é a de uma fabricação normal do pawn superar a qualidade atual. Em vez de calculá-la,
// cada rolagem é uma fabricação real; basta uma superar a qualidade atual para o item subir um nível.
public static class QualityUpgrade
{
    public static void TryUpgrade(CompQuality quality, Pawn crafter, int rollCount)
    {
        // Sem inspiração nem papel de produção, a rolagem do jogo vai no máximo até obra-prima, e é isso que
        // reserva o lendário a esses dois casos. A inspiração só é gasta no passo de obra-prima para lendário.
        bool isLegendaryStep = quality.Quality == QualityCategory.Masterwork;
        for (int roll = 0; roll < rollCount; roll++)
        {
            QualityCategory rolled = QualityUtility.GenerateQualityCreatedByPawn(crafter, SkillDefOf.Crafting,
                consumeInspiration: isLegendaryStep);
            if (rolled > quality.Quality)
            {
                // Sem contexto de arte, SetQuality não regenera a arte gravada.
                quality.SetQuality(quality.Quality + 1, null);
                return;
            }
        }
    }
}
