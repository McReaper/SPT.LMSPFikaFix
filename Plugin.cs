using BepInEx;
using BepInEx.Logging;
using LMSPFikaFix.Patches;

namespace LMSPFikaFix
{
    // Officially requires Fika and LMSP (README, release, Forge page), but neither is a
    // BepInDependency: a player without LMSP still needs the fix for the other players'
    // swaps, since their game works out the hits it deals to them.
    [BepInPlugin("com.mcreaper.lmspfikafix", "LMSP Fika Fix", "1.0.0")]
    [BepInDependency("com.SPT.core", "4.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource LogSource;

        private void Awake()
        {
            LogSource = Logger;

            new ItemAddedPatch().Enable();
            new ItemRemovedPatch().Enable();

            LogSource.LogInfo("LMSP Fika Fix loaded");
        }
    }
}
