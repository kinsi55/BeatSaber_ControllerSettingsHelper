using HMUI;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace ControllerSettingsHelper {
  class AxisDisplay : MonoBehaviour {
    GameObject parent;
    bool useWorldAngles;
    Color color;


    static Sprite arrowSprite;

    public static AxisDisplay CreateFor(Color color, GameObject parent = null, bool useWorldAngles = false) {
      if (arrowSprite == null) {
        var Tex2D = new Texture2D(2, 2);

        using (Stream resFilestream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ControllerSettingsHelper.arrow.png")) {
          byte[] ba = new byte[resFilestream.Length];
          resFilestream.Read(ba, 0, ba.Length);
          Tex2D.LoadImage(ba);

          arrowSprite = Sprite.Create(Tex2D, new Rect(0, 0, Tex2D.width, Tex2D.height), Vector2.zero, 1000);
        }
      }


      var d = new GameObject("AxisDisplay", new[] { typeof(AxisDisplay) }).GetComponent<AxisDisplay>();

      d.parent = parent;
      d.color = color;
      d.useWorldAngles = useWorldAngles;

      //d.Init();

      return d;
    }

    GameObject[] arrows = new GameObject[3];
    GameObject[] arrowTexts = new GameObject[3];

    public void Init() => Init(new[] { new Vector3(90, 90), Vector3.zero, new Vector3(90, 0) });
    public void Init(Vector3[] rots) {
      var axis = new[] { "X", "Y", "Z" };

      for (int i = 0; i < 3; i++) {
        var arr = arrows[i] = new GameObject($"{axis[i]} Arrow", new[] { typeof(Canvas), typeof(ImageView) });
        var label = arrowTexts[i] = new GameObject($"{axis[i]} Label", new[] { typeof(Canvas), typeof(TextMeshPro) });

        var arrI = arr.GetComponent<ImageView>();
        arrI.color = color;
        arrI.sprite = arrowSprite;

        arrI.material = Resources.FindObjectsOfTypeAll<Material>().FirstOrDefault(m => m.name == "UINoGlow");

        arr.transform.SetParent(transform, false);

        var tmp = label.GetComponent<TextMeshPro>();
        tmp.text = axis[i];
        tmp.fontSize = 2.3f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;

        label.transform.SetParent(transform, false);

        var r = ((RectTransform)arr.transform);
        var r2 = ((RectTransform)label.transform);

        r.pivot = new Vector2(0.5f, 0);
        r2.localEulerAngles = r.sizeDelta = Vector3.one;
        r.anchoredPosition3D = Vector3.zero;
        r.localScale = new Vector3(0.2f, 0.2f, 0);
        r.localEulerAngles = rots[i];

        r.localPosition = r.up * 0.05f;
        r2.localPosition = r.up * 0.3f;

        r2.sizeDelta = Vector3.zero;
      }
    }

    void Update() {
      if (parent != null) {
        transform.position = parent.transform.position;
        if (!useWorldAngles)
          transform.rotation = parent.transform.rotation;
      }

      for (var i = 0; i < 3; i++)
        arrowTexts[i].transform.rotation = Quaternion.identity;
    }

    void OnDisable() => GameObject.Destroy(gameObject);
  }
}
