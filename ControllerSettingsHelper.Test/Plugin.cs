#nullable enable

using IPA;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace ControllerSettingsHelper.Test {
  [Plugin(RuntimeOptions.SingleStartInit)]
  public class Plugin {
    private IPALogger? _logger;

    [Init]
    public void Init(IPALogger logger) {
      _logger = logger;
    }

    [OnStart]
    public void OnApplicationStart() {
      new TestRunner(_logger).Test(new[] { typeof(Plugin).Assembly });
      Application.Quit();
    }


    [OnExit]
    public void OnApplicationQuit() {
    }
  }
}
