using System;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;

namespace PlateSwapFix.Patches
{
    // Every Player (yours, the copies of the other players, the bots) registers as a view
    // of its inventory controller, which calls these two on every add and remove. They are
    // explicit interface implementations, so no subclass can override them: Fika's
    // ObservedPlayer empties OnItemAddedOrRemoved, which they call, but not these.

    internal class ItemAddedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return PlayerImplementation.Of(typeof(IAddHandler), nameof(IAddHandler.OnItemAdded));
        }

        [PatchPostfix]
        private static void Postfix(Player __instance, AddItemEventArgs __0)
        {
            if (__0.Status == CommandStatus.Succeed)
            {
                WornArmor.OnItemMoved(__instance, __0.Item, __0.To, added: true);
            }
        }
    }

    internal class ItemRemovedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return PlayerImplementation.Of(typeof(IRemoveHandler), nameof(IRemoveHandler.OnItemRemoved));
        }

        [PatchPostfix]
        private static void Postfix(Player __instance, RemoveItemEventArgs __0)
        {
            if (__0.Status == CommandStatus.Succeed)
            {
                WornArmor.OnItemMoved(__instance, __0.Item, __0.From, added: false);
            }
        }
    }

    internal static class PlayerImplementation
    {
        internal static MethodInfo Of(Type interfaceType, string methodName)
        {
            InterfaceMapping map = typeof(Player).GetInterfaceMap(interfaceType);
            for (int i = 0; i < map.InterfaceMethods.Length; i++)
            {
                if (map.InterfaceMethods[i].Name == methodName)
                {
                    return map.TargetMethods[i];
                }
            }
            throw new MissingMethodException(typeof(Player).FullName, interfaceType.Name + "." + methodName);
        }
    }
}
