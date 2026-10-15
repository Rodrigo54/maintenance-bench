using RimWorld;
using Verse;

namespace MaintenanceBench;

// Remove o biocode do item no lugar, deixando o equipamento saqueado utilizável pela colônia.
public class RecipeWorker_RemoveBiocode : RecipeWorker_InPlace
{
    public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
    {
        foreach (Thing item in ingredients.Where(IsTargetItem))
        {
            // O filtro fixo da receita já tira a arma persona; a checagem fica porque o UnCode dela desfaria o
            // vínculo com o pawn, o que não tem volta.
            if (item.TryGetComp<CompBladelinkWeapon>() != null)
            {
                throw new InvalidOperationException(
                    $"{item.def.defName} é arma persona e não deveria passar pelo filtro de {recipe.defName}.");
            }

            CompBiocodable biocode = item.TryGetComp<CompBiocodable>()
                ?? throw new InvalidOperationException(
                    $"{item.def.defName} não é biocodificável e não deveria passar pelo filtro de {recipe.defName}.");
            biocode.UnCode();
        }
    }
}
