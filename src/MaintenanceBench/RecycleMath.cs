namespace MaintenanceBench;

// Curva de retorno da reciclagem, sem tipos do jogo para poder ser testada fora dele.
public static class RecycleMath
{
    // De 20% com Crafting 0 a 75% com Crafting 20.
    private const float BaseReturnFraction = 0.20f;
    private const float ReturnFractionPerCraftingLevel = 0.0275f;

    public static float GetReturnFraction(int craftingLevel)
    {
        return BaseReturnFraction + ReturnFractionPerCraftingLevel * craftingLevel;
    }
}
