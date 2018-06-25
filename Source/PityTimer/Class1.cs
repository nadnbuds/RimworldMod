using RimWorld;
using Harmony;
using HugsLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;
using Verse;

namespace PityTimer
{
    [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt))]
    [HarmonyPatch("Interacted")]
    public class PityTimer : ModBase
    {
        public const string OpToFind = "Single Clamp(Single, Single, Single)";
        public static float pityMultiplier = 1.5f;
        public static Dictionary<string, int> recruitMap;

        public PityTimer()
        {
            recruitMap = new Dictionary<string, int>();
            Log.Message("PityTimerStarted");
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

                if (codeList[i].opcode == OpCodes.Call && 
                    codeList[i].operand.ToString().Equals(OpToFind))
                {
                    instruction = new CodeInstruction(OpCodes.Ldarg_2);
                    yield return instruction;

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

        public static float PityTimerTrigger(float num, float lowerBound, float upperBound, Pawn recipient)
        {
            float mapUpperBound = Mathf.Pow((1.0f / num) * pityMultiplier, 4);

            if(recruitMap.ContainsKey(recipient.ThingID))
            {
                int val = recruitMap[recipient.ThingID]++;
                float scale = Mathf.Pow(val, 4)/mapUpperBound;
                num += (1 - num) * scale;
            }
            else
            {
                recruitMap.Add(recipient.ThingID, 1);
            }
            return Mathf.Clamp(num, lowerBound, upperBound);
        }
    }
}
