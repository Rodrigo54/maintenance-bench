namespace MaintenanceBench.Tests;

public class RepairMathTests
{
    private const float BasicSlice = 0.25f;
    private const float AdvancedSlice = 0.75f;
    private const float BasicUpgradeMultiplier = 0.5f;
    private const float AdvancedUpgradeMultiplier = 1f;
    private const float Tolerance = 1e-5f;

    [Theory]
    [InlineData(10, 100, BasicSlice, 35)]
    [InlineData(10, 100, AdvancedSlice, 85)]
    [InlineData(0, 180, BasicSlice, 45)]
    public void DevolveUmaFatiaDoHpMaximo(int current, int max, float slice, int expected)
    {
        Assert.Equal(expected, RepairMath.GetRepairedHitPoints(current, max, slice));
    }

    [Theory]
    [InlineData(90, 100, BasicSlice)]
    [InlineData(30, 100, AdvancedSlice)]
    [InlineData(99, 100, BasicSlice)]
    public void UltimaExecucaoSoCompletaOQueFalta(int current, int max, float slice)
    {
        Assert.Equal(max, RepairMath.GetRepairedHitPoints(current, max, slice));
    }

    [Fact]
    public void FatiaArredondaParaCimaEmItemComHpBaixo()
    {
        // 3 × 0,25 = 0,75: sem arredondar para cima o item nunca sairia do lugar.
        Assert.Equal(2, RepairMath.GetRepairedHitPoints(1, 3, BasicSlice));
    }

    [Theory]
    [InlineData(10, BasicSlice, 4)]
    [InlineData(10, AdvancedSlice, 2)]
    [InlineData(0, BasicSlice, 4)]
    [InlineData(0, AdvancedSlice, 2)]
    public void ExecucoesAteFicarCheioBatemComORoteiro(int startPercent, float slice, int expectedRuns)
    {
        const int max = 100;
        int hitPoints = startPercent;
        int runs = 0;
        while (hitPoints < max)
        {
            hitPoints = RepairMath.GetRepairedHitPoints(hitPoints, max, slice);
            runs++;
        }

        Assert.Equal(expectedRuns, runs);
    }

    [Theory]
    [InlineData(10, 35, 100, 0.25f)]
    [InlineData(90, 100, 100, 0.10f)]
    [InlineData(0, 45, 180, 0.25f)]
    public void FracaoDevolvidaEhOQueOReparoRealmenteRestaurou(int before, int after, int max, float expected)
    {
        Assert.Equal(expected, RepairMath.GetRestoredFraction(before, after, max), Tolerance);
    }

    [Theory]
    [InlineData(0, BasicSlice, BasicUpgradeMultiplier, 0.5f)]
    [InlineData(0, AdvancedSlice, AdvancedUpgradeMultiplier, 1f)]
    [InlineData(10, BasicSlice, BasicUpgradeMultiplier, 0.45f)]
    [InlineData(10, AdvancedSlice, AdvancedUpgradeMultiplier, 0.9f)]
    public void ChanceSomadaNoReparoInteiroEhMultiplicadorVezesHpRestaurado(
        int startPercent, float slice, float multiplier, float expectedTotal)
    {
        const int max = 100;
        int hitPoints = startPercent;
        float totalChance = 0f;
        while (hitPoints < max)
        {
            int before = hitPoints;
            hitPoints = RepairMath.GetRepairedHitPoints(hitPoints, max, slice);
            totalChance += multiplier * RepairMath.GetRestoredFraction(before, hitPoints, max);
        }

        Assert.Equal(expectedTotal, totalChance, Tolerance);
    }
}
