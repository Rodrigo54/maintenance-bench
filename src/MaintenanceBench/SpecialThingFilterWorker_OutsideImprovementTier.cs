using RimWorld;
using Verse;

namespace MaintenanceBench;

// Casa com item fora do nível de qualidade que a busca de ingredientes da melhoria está percorrendo.
public class SpecialThingFilterWorker_OutsideImprovementTier : SpecialThingFilterWorker
{
    public override bool Matches(Thing t)
    {
        return ImprovementSearchTier.Current is QualityCategory tier
            && t.TryGetQuality(out QualityCategory quality)
            && quality != tier;
    }
}
