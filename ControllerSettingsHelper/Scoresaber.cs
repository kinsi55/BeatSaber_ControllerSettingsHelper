using HarmonyLib;
using System.Reflection;

namespace ControllerSettingsHelper {
  static class Scoresaber {
    static MethodBase ScoreSaber_playbackEnabled = AccessTools.Method("ScoreSaber.Core.ReplaySystem.HarmonyPatches.PatchHandleHMDUnmounted:Prefix");

    public static bool IsInReplay() {
      try {
        return ScoreSaber_playbackEnabled != null && (bool)ScoreSaber_playbackEnabled.Invoke(null, null) == false;
      } catch { }
      return false;
    }
  }
}
