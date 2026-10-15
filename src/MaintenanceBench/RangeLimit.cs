using System.Numerics;

namespace MaintenanceBench;

// Limite de faixa, sem tipos do jogo para poder ser testado fora dele.
public static class RangeLimit
{
    // Traz as duas pontas da faixa para dentro do limite; uma faixa toda fora dele colapsa na ponta mais próxima.
    public static (T Min, T Max) Clamp<T>(T min, T max, T limitMin, T limitMax) where T : INumber<T>
    {
        return (T.Clamp(min, limitMin, limitMax), T.Clamp(max, limitMin, limitMax));
    }
}
