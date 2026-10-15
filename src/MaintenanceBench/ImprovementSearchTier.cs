using RimWorld;

namespace MaintenanceBench;

// Nível de qualidade a que a busca de ingredientes da melhoria está restrita no momento. O patch da busca define um
// nível por vez, e SpecialThingFilterWorker_OutsideImprovementTier tira da busca os itens dos outros níveis. Fora
// dessa busca fica nulo e não restringe nada.
public static class ImprovementSearchTier
{
    public static QualityCategory? Current { get; set; }
}
