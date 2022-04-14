using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace ShiftyShingles
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ManualLogSource logger;

        private void Awake()
        {
            // Plugin startup logic
            logger = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded");
            Logger.LogInfo($"Patching...");
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"Patched");
        }

        [HarmonyPatch(typeof(HandToFront), "ShingleRenderBlueprint")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> StuckChecker_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new CodeMatcher(instructions).MatchForward(true,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(HandToFront), "tilesRotID")),
                new CodeMatch(OpCodes.Brtrue)
            ).InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_2))
            .SetOpcodeAndAdvance(OpCodes.Bne_Un)
            .Advance(1)
            .SetOpcodeAndAdvance(OpCodes.Ldc_I4_0)
            .Advance(3)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(HandToFront), "tilesRotID"))
            ).SetOpcodeAndAdvance(OpCodes.Ldc_I4_1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Add))
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Shingle), "shingSize")),
                new CodeMatch(OpCodes.Brfalse),
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Shingle), "shingSize")),
                new CodeMatch(OpCodes.Ldc_I4_2),
                new CodeMatch(OpCodes.Bne_Un),
                new CodeMatch(OpCodes.Ldarg_0)
            )
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Shingle), "shingSize")),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(HandToFront), "tilesRotID")),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(HandToFront), "_renderBlueprint")),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Plugin), "Translate"))
            )
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Shingle), "shingSize")),
                new CodeMatch(OpCodes.Brfalse),
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Shingle), "shingSize")),
                new CodeMatch(OpCodes.Ldc_I4_2),
                new CodeMatch(OpCodes.Bne_Un),
                new CodeMatch(OpCodes.Ldarg_0)
            )
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Shingle), "shingSize")),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(HandToFront), "tilesRotID")),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(HandToFront), "_renderBlueprint")),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Plugin), "Translate"))
            );

            return matcher.InstructionEnumeration();
        }

        static void Translate(int shingSize, int tilesRotID, GameObject _renderBlueprint)
        {
            if ((shingSize == 0 || shingSize == 2) && tilesRotID == 2)
            {
                _renderBlueprint.transform.Translate(Vector3.left * 0.1f);
            }
        }
    }
}
