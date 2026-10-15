using HarmonyLib;
using Verse;

namespace MaintenanceBench;

// Aplica os patches Harmony do mod quando o jogo termina de carregar os defs.
[StaticConstructorOnStartup]
public static class HarmonyBootstrap
{
    static HarmonyBootstrap()
    {
        new Harmony("rodrigo54.maintenancebench").PatchAll();
    }
}
