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
		internal static Plugin? Instance { get; private set; }
		internal static IPALogger? Log { get; private set; }
		internal static Harmony? harmony { get; private set; }
		internal SettingReadjuster? callibrator;

		[Init]
		public void Init(IPALogger logger, IPA.Config.Config conf) {
			Instance = this;
			Log = logger;
			Config.Instance = conf.Generated<Config>();
		}

		[OnStart]
		public void OnApplicationStart() {
			harmony = new Harmony("Kinsi55.BeatSaber.ControllerSettingsHelper");
			harmony.PatchAll(Assembly.GetExecutingAssembly());

			SceneManager.activeSceneChanged += OnActiveSceneChanged;
			BSMLSettings.instance.AddSettingsMenu("Controller Helper", "ControllerSettingsHelper.Views.settings.bsml", Config.Instance);
			callibrator = new SettingReadjuster(Log!);
		}

		public void OnActiveSceneChanged(Scene oldScene, Scene newScene) {
			if(Config.Instance.EnableAxisArrowsInMenu && newScene.name == "MainMenu")
				SharedCoroutineStarter.instance.StartCoroutine(SpawnAxis(false));

			if(Config.Instance.EnableAxisArrowsInReplay && newScene.name == "GameCore")
				SharedCoroutineStarter.instance.StartCoroutine(SpawnAxis(true));
		}

		IEnumerator SpawnAxis(bool inSong) {
			yield return 0;
			yield return new WaitForEndOfFrame();

			if(inSong && !Scoresaber.IsInReplay())
				yield break;

			foreach(var c in Resources.FindObjectsOfTypeAll<VRController>()) {
				if(c.transform.childCount == 0 || (inSong && c.transform.GetChild(0).GetComponent<Saber>() == null))
					continue;

				var s = c.transform.GetChild(0).gameObject;

				if(c.node == UnityEngine.XR.XRNode.LeftHand) {
					var lO = new[] { new Vector3(90, -90), Vector3.zero, new Vector3(90, 0) };

					AxisDisplay.CreateFor(Color.red, s).Init(lO);
					AxisDisplay.CreateFor(Color.gray, s, true).Init(lO);
				} else if(c.node == UnityEngine.XR.XRNode.RightHand) {
					AxisDisplay.CreateFor(Color.red, s).Init();
					AxisDisplay.CreateFor(Color.gray, s, true).Init();
				}
			}
		}

		[OnExit]
		public void OnApplicationQuit() {
			callibrator?.Dispose();
			harmony?.UnpatchSelf();
		}
	}
}
