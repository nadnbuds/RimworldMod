using RimWorld;
using Harmony;
using HugsLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace PityTimer
{
    [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt))]
    [HarmonyPatch("Interacted")]
    public class PityTimer : ModBase
    {
        public PityTimer()
        {
            Verse.Log.Message("PityTimerStarted");
        }

        public override string ModIdentifier => "LorenzoAlamilloPityTimer";

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> MyTranspiler(IEnumerable<CodeInstruction> instr)
        {
            MethodInfo Clamp = AccessTools.Method(typeof(float),"Clamp", new System.Type[] { typeof(float), typeof(float), typeof(float) });
            List<CodeInstruction> codeList = new List<CodeInstruction>(instr);
            for(int i = 0; i < codeList.Count; ++i)
            {
                CodeInstruction instruction = codeList[i];
                if (codeList[i].opcode == OpCodes.Call
                    && codeList[i].operand.ToString().Equals("Single Clamp(Single, Single, Single)"))
                {
                    yield return instruction;
                    Verse.Log.Message("PityTimer OpCode Found");
                    instruction = new CodeInstruction(OpCodes.Call);
                    instruction.operand = typeof(PityTimer).GetMethod(
                        nameof(PityTimer.PityTimerTrigger), BindingFlags.Static | BindingFlags.Public); 

                    yield return instruction;
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static void PityTimerTrigger()
        {
            Verse.Log.Message("PityTimer Function Trigger");
        }
    }
}
