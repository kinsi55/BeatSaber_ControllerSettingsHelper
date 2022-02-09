#nullable enable

using HMUI;
using System;
using System.Linq;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace ControllerSettingsHelper {
  internal class SettingReadjuster : IDisposable {

    private readonly IPALogger logger;
    private Vector3SO? controllerRotation;
    private Vector3 previousRotation;

    public SettingReadjuster(IPALogger logger) {
      this.logger = logger;
      Enable();
    }

    private void Enable() {
      var mainSettings = Resources.FindObjectsOfTypeAll<MainSettingsModelSO>().FirstOrDefault();
      if (mainSettings == null) {
        logger.Error("ImportFromGame: Unable to get a handle on MainSettingsModelSO. Exiting...");
        return;
      }

      controllerRotation = mainSettings.controllerRotation;
      previousRotation = controllerRotation.value;

      controllerRotation.didChangeEvent += ReadjustPosition;
    }

    public void Dispose() {
      if (controllerRotation != null) {
        controllerRotation.didChangeEvent -= ReadjustPosition;
        controllerRotation = null;
      }
      GC.SuppressFinalize(this);
    }

    private void ReadjustPosition() {
      var mainSettings = Resources.FindObjectsOfTypeAll<MainSettingsModelSO>().FirstOrDefault();
      var position = mainSettings?.controllerPosition;
      var rotation = controllerRotation;
      if (mainSettings == null || position == null || rotation == null) {
        return;
      }

      var targetPosition = Calibrate(position, rotation, previousRotation);

      logger.Debug($"previousRotation: {previousRotation}, rotation: {rotation.value}");
      logger.Debug($"previousPosition: {position.value * 100}, position: {targetPosition * 100}");

      previousRotation = rotation.value;
      mainSettings.controllerPosition.value = targetPosition;
      SetPositionSlider("X", targetPosition.x);
      SetPositionSlider("Y", targetPosition.y);
      SetPositionSlider("Z", targetPosition.z);
    }

    internal static Vector3 Calibrate(Vector3 position, Vector3 rotation, Vector3 previousRotation) {
      var transform = new GameObject().transform;
      transform.Rotate(previousRotation);
      transform.Translate(position);
      var world = transform.TransformPoint(Vector3.zero);

      transform.position = Vector3.zero;
      transform.rotation = Quaternion.identity;
      transform.Rotate(rotation);
      var local = transform.InverseTransformPoint(world);

      return local;
    }

    private static void SetPositionSlider(string axis, float value) {
      var sliderObject = GameObject.Find($"/MenuCore/UI/ScreenSystem/ScreenContainer/MainScreen/SettingsViewController/Container/ControllersTransformSettings/Content/Position{axis}/Slider");
      var slider = sliderObject.GetComponent<CustomFormatRangeValuesSlider>();
      if (slider.enabled) {
        slider.value = value * 100;
      }
    }
  }
}
