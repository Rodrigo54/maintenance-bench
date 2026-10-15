namespace MaintenanceBench.Tests;

public class RangeLimitTests
{
    [Fact]
    public void FaixaDentroDoLimiteNaoMuda()
    {
        Assert.Equal((0.1f, 0.2f), RangeLimit.Clamp(0.1f, 0.2f, 0f, 0.99f));
    }

    [Fact]
    public void PontaAcimaDoLimiteVoltaAoLimite()
    {
        Assert.Equal((0f, 0.99f), RangeLimit.Clamp(0f, 1f, 0f, 0.99f));
    }

    [Fact]
    public void PontaAbaixoDoLimiteSobeAoLimite()
    {
        // Melhoria: só item cheio.
        Assert.Equal((1f, 1f), RangeLimit.Clamp(0.5f, 1f, 1f, 1f));
    }

    [Fact]
    public void FaixaTodaForaColapsaNaPontaMaisProxima()
    {
        Assert.Equal((5, 5), RangeLimit.Clamp(6, 6, 0, 5));
    }
}
