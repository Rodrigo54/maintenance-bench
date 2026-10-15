using Verse;

namespace MaintenanceBench;

// Guarda em memória quando cada item passou pela última tentativa de melhoria. Não vai para o save: depois de
// carregar, as esperas recomeçam zeradas, o que só libera os itens um pouco antes. Um GameComponent persistiria,
// mas deixaria uma entrada do mod em todo save.
public static class ImprovementCooldown
{
    // Uma hora de jogo.
    private const int CooldownTicks = 2500;

    private static readonly Dictionary<Thing, int> lastAttemptTicks = new();

    public static void RecordAttempt(Thing item)
    {
        int now = Find.TickManager.TicksGame;
        // Descarta esperas vencidas para o dicionário não acumular itens. Tick no futuro é de um jogo carregado
        // antes deste.
        lastAttemptTicks.RemoveAll(entry => now - entry.Value >= CooldownTicks || entry.Value > now);
        lastAttemptTicks[item] = now;
    }

    public static bool IsCoolingDown(Thing item)
    {
        return lastAttemptTicks.TryGetValue(item, out int attemptTick)
            && Find.TickManager.TicksGame - attemptTick < CooldownTicks;
    }
}
