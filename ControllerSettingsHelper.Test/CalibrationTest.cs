using UnityEngine;
using Xunit;

namespace ControllerSettingsHelper.Test {
  public class CalibrationTest {
    [Test]
    public void TestZeroAdjustment() {
      var newPosition = SettingReadjuster.Calibrate(Vector3.zero, Vector3.zero, Vector3.zero);
      Assert.Equal(Vector3.zero, newPosition);
    }

    [Test]
    public void TestBasicAdjustment() {
      var position = new Vector3(0.5f, 0.3f, 0.1f);
      var rotation = new Vector3(90, 0, 0);
      var previousRotation = Vector3.zero;
      var newPosition = SettingReadjuster.Calibrate(position, rotation, previousRotation);
      Assert.NotStrictEqual(new Vector3(0.5f, 0.1f, -0.3f), newPosition);
    }
  }
}
