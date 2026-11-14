using BepInEx;
using BepInEx.Logging;
using PlateSwapFix.Patches;

namespace PlateSwapFix
{
    [BepInPlugin("com.mcreaper.plateswapfix", "Plate Swap Fix", "1.0.0")]
    [BepInDependency("com.SPT.core", "4.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource LogSource;

        private void Awake()
        {
            LogSource = Logger;

            new ItemAddedPatch().Enable();
            new ItemRemovedPatch().Enable();

            LogSource.LogInfo("Plate Swap Fix loaded");
        }
    }
}
