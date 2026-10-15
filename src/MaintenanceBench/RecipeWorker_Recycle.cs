using RimWorld;
using Verse;

namespace MaintenanceBench;

// Recicla o item: devolve perto da bancada parte do material de fabricação, conforme a skill de Crafting de quem
// recicla. Diferente da fundição do jogo, devolve também material que não é fundível, como tecido e couro.
// O ConsumeIngredient padrão já destruiu o item quando esta chamada chega, mas def e material continuam legíveis.
public class RecipeWorker_Recycle : RecipeWorker
{
    public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
    {
        float returnFraction = RecycleMath.GetReturnFraction(GetCraftingLevel(billDoer));
        foreach (Thing material in ingredients.SelectMany(item => MakeReturnedMaterials(item, returnFraction)))
        {
            // Pilha acima do limite é dividida pelo próprio GenPlace.
            GenPlace.TryPlaceThing(material, billDoer.Position, billDoer.Map, ThingPlaceMode.Near);
        }
    }

    private static IEnumerable<Thing> MakeReturnedMaterials(Thing item, float returnFraction)
    {
        // Componentes (intricate) nunca são devolvidos, como na fundição.
        foreach (ThingDefCountClass cost in item.CostListAdjusted().Where(cost => !cost.thingDef.intricate))
        {
            int count = GenMath.RoundRandom(cost.count * returnFraction);
            if (count <= 0)
            {
                continue;
            }

            Thing material = ThingMaker.MakeThing(cost.thingDef);
            material.stackCount = count;
            yield return material;
        }
    }

    // Mecanoide não tem skill própria; o jogo usa o nível fixo da raça, como em QualityUtility.
    private static int GetCraftingLevel(Pawn pawn)
    {
        return pawn.RaceProps.IsMechanoid
            ? pawn.RaceProps.mechFixedSkillLevel
            : pawn.skills.GetSkill(SkillDefOf.Crafting).Level;
    }
}
