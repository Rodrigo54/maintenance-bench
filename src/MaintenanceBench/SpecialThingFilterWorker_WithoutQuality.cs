using RimWorld;
using Verse;

namespace MaintenanceBench;

// Casa com item sem qualidade. O filtro de qualidade do jogo deixa esses itens passarem, então a melhoria de item
// os proíbe por este filtro especial.
public class SpecialThingFilterWorker_WithoutQuality : SpecialThingFilterWorker
{
    public override bool Matches(Thing t)
    {
        return AlwaysMatches(t.def);
    }

    public override bool AlwaysMatches(ThingDef def)
    {
        return !def.HasComp(typeof(CompQuality));
    }

    public override bool CanEverMatch(ThingDef def)
    {
        return AlwaysMatches(def);
    }
}
