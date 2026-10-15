using Verse;

namespace MaintenanceBench;

// Parâmetros de uma receita de reparo, declarados no XML da receita.
public class RepairExtension : DefModExtension
{
    // Fração do HP máximo devolvida por execução.
    public float hitPointsFraction;

    // Chance de upgrade por barra de HP restaurada, em múltiplos da chance base de fabricação.
    public float upgradeChanceMultiplier;

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string error in base.ConfigErrors())
        {
            yield return error;
        }

        if (hitPointsFraction <= 0f || hitPointsFraction > 1f)
        {
            yield return $"hitPointsFraction deve estar entre 0 (exclusivo) e 1, mas é {hitPointsFraction}.";
        }

        if (upgradeChanceMultiplier < 0f)
        {
            yield return $"upgradeChanceMultiplier não pode ser negativo, mas é {upgradeChanceMultiplier}.";
        }

        // A chance de a rolagem acontecer é multiplicador × fração; acima de 1 ela deixaria de ser probabilidade.
        if (upgradeChanceMultiplier * hitPointsFraction > 1f)
        {
            yield return "upgradeChanceMultiplier × hitPointsFraction não pode passar de 1.";
        }
    }
}
