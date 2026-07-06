using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResultUI : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.OnWin  += () => Show(won: true);
        GameManager.Instance.OnLose += () => Show(won: false);
    }

    void Show(bool won)
    {
        int  levelNum = LevelManager.Instance.CurrentIndex + 1;
        bool isLast   = LevelManager.Instance.IsLastLevel;

        // Canvas
        var canvasGo = new GameObject("ResultCanvas");
        var canvas   = canvasGo.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight  = 1f;
        canvasGo.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<EventSystem>() == null)
        {
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<StandaloneInputModule>();
        }

        // Karartma
        var overlayGo  = new GameObject("Overlay");
        overlayGo.transform.SetParent(canvasGo.transform, false);
        overlayGo.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.72f);
        Stretch(overlayGo.GetComponent<RectTransform>());

        // Merkez panel
        var panelGo  = new GameObject("Panel");
        panelGo.transform.SetParent(canvasGo.transform, false);
        panelGo.AddComponent<Image>().color = won
            ? new Color(0.08f, 0.14f, 0.08f, 0.97f)
            : new Color(0.14f, 0.05f, 0.05f, 0.97f);

        var panelRect       = panelGo.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.08f, 0.33f);
        panelRect.anchorMax = new Vector2(0.92f, 0.67f);
        panelRect.offsetMin = panelRect.offsetMax = Vector2.zero;

        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // "LEVEL 3"
        MakeText(panelGo, "LevelNum",
            $"LEVEL {levelNum} / {LevelManager.Instance.TotalLevels}",
            36, false, new Color(1f, 1f, 1f, 0.45f),
            new Vector2(0f, 0.80f), Vector2.one, font);

        // Başlık
        MakeText(panelGo, "Title",
            won ? "KAZANDIN!" : "KAYBETTIN",
            84, true,
            won ? new Color(1f, 0.85f, 0.2f) : new Color(1f, 0.35f, 0.35f),
            new Vector2(0f, 0.52f), new Vector2(1f, 0.82f), font);

        // Alt yazı
        MakeText(panelGo, "Sub",
            won ? "Siparişleri tamamladın!" : "Grid doldu, tekrar dene!",
            40, false, new Color(1f, 1f, 1f, 0.6f),
            new Vector2(0.05f, 0.30f), new Vector2(0.95f, 0.54f), font);

        // Buton
        var btnGo = new GameObject("Btn");
        btnGo.transform.SetParent(panelGo.transform, false);

        var btnImg   = btnGo.AddComponent<Image>();
        btnImg.color = won ? new Color(0.18f, 0.68f, 0.28f)
                           : new Color(0.68f, 0.18f, 0.18f);

        var btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = btnImg;

        if (won) btn.onClick.AddListener(() => LevelManager.Instance.NextLevel());
        else     btn.onClick.AddListener(() => LevelManager.Instance.RestartLevel());

        var btnRect       = btnGo.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.12f, 0.06f);
        btnRect.anchorMax = new Vector2(0.88f, 0.26f);
        btnRect.offsetMin = btnRect.offsetMax = Vector2.zero;

        string btnText = won
            ? (isLast ? "BAŞA DÖN" : "SONRAKİ LEVEL")
            : "TEKRAR OYNA";

        MakeText(btnGo, "BtnLabel", btnText, 52, true, Color.white,
            Vector2.zero, Vector2.one, font);
    }

    static void MakeText(GameObject parent, string name, string text, int size,
                         bool bold, Color color, Vector2 anchorMin, Vector2 anchorMax, Font font)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);

        var t       = go.AddComponent<Text>();
        t.font      = font;
        t.text      = text;
        t.fontSize  = size;
        t.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
        t.color     = color;
        t.alignment = TextAnchor.MiddleCenter;

        var r       = go.GetComponent<RectTransform>();
        r.anchorMin = anchorMin;
        r.anchorMax = anchorMax;
        r.offsetMin = r.offsetMax = Vector2.zero;
    }

    static void Stretch(RectTransform r)
    {
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = r.offsetMax = Vector2.zero;
    }
}
