using Verse;

namespace MaintenanceBench;

// Casa com item que passou há pouco por uma tentativa de melhoria. O jogo escolhe o ingrediente mais próximo, e o
// item melhorado fica em cima da bancada; sem esta espera a bill voltaria sempre a ele e ignoraria os outros.
public class SpecialThingFilterWorker_RecentlyImproved : SpecialThingFilterWorker
{
    public override bool Matches(Thing t)
    {
        return ImprovementCooldown.IsCoolingDown(t);
    }
}
