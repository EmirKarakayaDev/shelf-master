using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    struct Slot { public Image icon; public Text label; }

    Slot[] _slots;
    ProductType[] _types;

    void Start()
    {
        BuildUI();
        OrderManager.Instance.OnOrderUpdated += Refresh;
        Refresh();
    }

    void OnDestroy()
    {
        if (OrderManager.Instance != null)
            OrderManager.Instance.OnOrderUpdated -= Refresh;
    }

    void BuildUI()
    {
        var orders = new List<(ProductType, int)>();
        foreach (var o in OrderManager.Instance.GetAllOrders())
            orders.Add(o);

        _types = new ProductType[orders.Count];
        _slots = new Slot[orders.Count];
        for (int i = 0; i < orders.Count; i++)
            _types[i] = orders[i].Item1;

        // Canvas
        var canvasGo = new GameObject("OrderCanvas");
        var canvas   = canvasGo.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight  = 1f;
        canvasGo.AddComponent<GraphicRaycaster>();

        // Üst panel
        var panelGo  = new GameObject("OrderPanel");
        panelGo.transform.SetParent(canvasGo.transform, false);

        var panelImg   = panelGo.AddComponent<Image>();
        panelImg.color = new Color(0.05f, 0.05f, 0.1f, 0.85f);

        var panelRect       = panelGo.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.pivot     = new Vector2(0.5f, 1f);
        panelRect.offsetMin = new Vector2(0f, -200f);
        panelRect.offsetMax = Vector2.zero;

        // Her sipariş için: renkli kare + "x12" yazısı
        var font     = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        float step   = 1080f / orders.Count;
        float startX = -1080f * 0.5f + step * 0.5f;

        for (int i = 0; i < orders.Count; i++)
        {
            float cx = startX + i * step;

            // İkon
            var iconGo   = new GameObject($"Icon_{i}");
            iconGo.transform.SetParent(panelGo.transform, false);

            var icon = iconGo.AddComponent<Image>();
            var iconRect          = iconGo.GetComponent<RectTransform>();
            iconRect.anchorMin    = iconRect.anchorMax = new Vector2(0.5f, 0.5f);
            iconRect.pivot        = new Vector2(0.5f, 0.5f);
            iconRect.sizeDelta    = new Vector2(90f, 90f);
            iconRect.anchoredPosition = new Vector2(cx - 55f, 0f);

            // Sayı metni
            var lblGo = new GameObject($"Label_{i}");
            lblGo.transform.SetParent(panelGo.transform, false);

            var lbl         = lblGo.AddComponent<Text>();
            lbl.font        = font;
            lbl.fontSize    = 56;
            lbl.fontStyle   = FontStyle.Bold;
            lbl.color       = Color.white;
            lbl.alignment   = TextAnchor.MiddleLeft;

            var lblRect           = lblGo.GetComponent<RectTransform>();
            lblRect.anchorMin     = lblRect.anchorMax = new Vector2(0.5f, 0.5f);
            lblRect.pivot         = new Vector2(0f, 0.5f);
            lblRect.sizeDelta     = new Vector2(150f, 80f);
            lblRect.anchoredPosition = new Vector2(cx + 5f, 0f);

            _slots[i] = new Slot { icon = icon, label = lbl };
        }
    }

    void Refresh()
    {
        if (_types == null) return;

        for (int i = 0; i < _slots.Length; i++)
        {
            int  rem   = OrderManager.Instance.GetRemaining(_types[i]);
            var  color = GameManager.Instance.GetColor(_types[i]);
            bool done  = rem <= 0;

            _slots[i].icon.color  = done ? new Color(color.r, color.g, color.b, 0.25f) : color;
            _slots[i].label.text  = $"x{rem}";
            _slots[i].label.color = done ? new Color(1f, 1f, 1f, 0.35f) : Color.white;
        }
    }
}
