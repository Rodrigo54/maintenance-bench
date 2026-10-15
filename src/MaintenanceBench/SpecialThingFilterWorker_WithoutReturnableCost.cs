using RimWorld;
using Verse;

namespace MaintenanceBench;

// Casa com item que a reciclagem não teria o que devolver: sem custo de fabricação (saque como zeushammer e
// equipamento de eltex) ou com custo só em componentes, que nunca são devolvidos.
public class SpecialThingFilterWorker_WithoutReturnableCost : SpecialThingFilterWorker
{
    public override bool Matches(Thing t)
    {
        return AlwaysMatches(t.def);
    }

    // Material (stuff) nunca é intricate, então custo em material sempre tem o que devolver.
    public override bool AlwaysMatches(ThingDef def)
    {
        return def.costStuffCount <= 0 && (def.costList == null || def.costList.All(cost => cost.thingDef.intricate));
    }

    public override bool CanEverMatch(ThingDef def)
    {
        return AlwaysMatches(def);
    }
}
