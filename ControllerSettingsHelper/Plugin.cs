#nullable enable

using BeatSaberMarkupLanguage.Settings;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace ControllerSettingsHelper {
  [Plugin(RuntimeOptions.SingleStartInit)]
  public class Plugin {
    private Harmony? _harmony;
    private IPALogger? _logger;
    private SettingReadjuster? _adjuster;

    [Init]
    public void Init(IPALogger logger, IPA.Config.Config configuration) {
      _logger = logger;
      Config.Instance = configuration.Generated<Config>();
    }

    [OnStart]
    public void OnApplicationStart() {
      _harmony = new Harmony("Kinsi55.BeatSaber.ControllerSettingsHelper");
      _harmony.PatchAll(Assembly.GetExecutingAssembly());

      SceneManager.activeSceneChanged += OnActiveSceneChanged;
      BSMLSettings.instance.AddSettingsMenu("Controller Helper", "ControllerSettingsHelper.Views.settings.bsml", Config.Instance);
      _adjuster = new SettingReadjuster(_logger!);
    }

    [OnExit]
    public void OnApplicationQuit() {
      _adjuster?.Dispose();
      _harmony?.UnpatchSelf();
    }

    public void OnActiveSceneChanged(Scene oldScene, Scene newScene) {
      if (Config.Instance.EnableAxisArrowsInMenu && newScene.name == "MainMenu")
        SharedCoroutineStarter.instance.StartCoroutine(SpawnAxis(false));

      if (Config.Instance.EnableAxisArrowsInReplay && newScene.name == "GameCore")
        SharedCoroutineStarter.instance.StartCoroutine(SpawnAxis(true));
    }

    private IEnumerator SpawnAxis(bool inSong) {
      yield return 0;
      yield return new WaitForEndOfFrame();

      if (inSong && !Scoresaber.IsInReplay()) {
        yield break;
      }

      foreach (var c in Resources.FindObjectsOfTypeAll<VRController>()) {
        if (c.transform.childCount == 0 || (inSong && c.transform.GetChild(0).GetComponent<Saber>() == null))
          continue;

        var s = c.transform.GetChild(0).gameObject;

        if (c.node == UnityEngine.XR.XRNode.LeftHand) {
          var lO = new[] { new Vector3(90, -90), Vector3.zero, new Vector3(90, 0) };

          AxisDisplay.CreateFor(Color.red, s).Init(lO);
          AxisDisplay.CreateFor(Color.gray, s, true).Init(lO);
        } else if (c.node == UnityEngine.XR.XRNode.RightHand) {
          AxisDisplay.CreateFor(Color.red, s).Init();
          AxisDisplay.CreateFor(Color.gray, s, true).Init();
        }
      }
    }
  }
}
