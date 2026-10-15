namespace MaintenanceBench;

// Cálculo do reparo em fatias, sem tipos do jogo para poder ser testado fora dele.
public static class RepairMath
{
    // A fatia é arredondada para cima para que item com HP máximo baixo ainda avance a cada execução;
    // a última execução só completa o que falta.
    public static int GetRepairedHitPoints(int currentHitPoints, int maxHitPoints, float sliceFraction)
    {
        int slice = (int)Math.Ceiling(maxHitPoints * sliceFraction);
        return Math.Min(maxHitPoints, currentHitPoints + slice);
    }

    // Fração do HP máximo efetivamente devolvida numa execução. É ela, e não a fatia nominal, que pondera a
    // chance de upgrade: a última execução devolve menos e por isso conta menos.
    public static float GetRestoredFraction(int hitPointsBefore, int hitPointsAfter, int maxHitPoints)
    {
        return (hitPointsAfter - hitPointsBefore) / (float)maxHitPoints;
    }
}
