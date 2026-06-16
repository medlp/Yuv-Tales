using System.Collections.Generic;
using UnityEngine;

namespace DS
{
    public static class DSDialogueFlags
    {
        private static Dictionary<string, bool> flags = new Dictionary<string, bool>();

        public static bool Get(string flagName)
        {
            if (string.IsNullOrEmpty(flagName))
                return false;

            return flags.TryGetValue(flagName, out bool value) ? value : false;
        }

        public static void Set(string flagName, bool value)
        {
            if (string.IsNullOrEmpty(flagName))
                return;

            flags[flagName] = value;
        }

        public static bool IsChoiceAvailable(DS.Data.DSDialogueChoiceData choice)
        {
            if (string.IsNullOrEmpty(choice.RequiredFlag))
                return true; 

            return Get(choice.RequiredFlag) == choice.RequiredFlagValue;
        }

        public static void ApplyChoiceEffect(DS.Data.DSDialogueChoiceData choice)
        {
            if (string.IsNullOrEmpty(choice.OnChosenFlag))
                return;

            Set(choice.OnChosenFlag, choice.OnChosenFlagValue);
        }

        public static void ResetAll()
        {
            flags.Clear();
        }

        public static void DebugPrintAll()
        {
            foreach (var kv in flags)
                Debug.Log($"[DSFlags] {kv.Key} = {kv.Value}");
        }
    }
}
