namespace MaintenanceBench.Tests;

public class RecycleMathTests
{
    private const float Tolerance = 1e-5f;

    [Theory]
    [InlineData(0, 0.20f)]
    [InlineData(10, 0.475f)]
    [InlineData(20, 0.75f)]
    public void RetornoVaiDe20A75PorcentoConformeACrafting(int craftingLevel, float expected)
    {
        Assert.Equal(expected, RecycleMath.GetReturnFraction(craftingLevel), Tolerance);
    }
}
