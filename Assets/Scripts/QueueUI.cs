using UnityEngine;
using UnityEngine.UI;

public class QueueUI : MonoBehaviour
{
    const int SlotCount = 5;

    Image[] _slots;

    void Start()
    {
        BuildUI();
        QueueManager.Instance.OnChanged += Refresh;
        Refresh();
    }

    void OnDestroy()
    {
        if (QueueManager.Instance != null)
            QueueManager.Instance.OnChanged -= Refresh;
    }

    void BuildUI()
    {
        // Canvas
        var canvasGo = new GameObject("QueueCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode        = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight  = 1f; // portrait için height'a göre ölçekle

        canvasGo.AddComponent<GraphicRaycaster>();

        // Alt panel
        var panelGo  = new GameObject("QueuePanel");
        panelGo.transform.SetParent(canvasGo.transform, false);

        var panelImg   = panelGo.AddComponent<Image>();
        panelImg.color = new Color(0.05f, 0.05f, 0.1f, 0.85f);

        var panelRect        = panelGo.GetComponent<RectTransform>();
        panelRect.anchorMin  = new Vector2(0f, 0f);
        panelRect.anchorMax  = new Vector2(1f, 0f);
        panelRect.pivot      = new Vector2(0.5f, 0f);
        panelRect.offsetMin  = Vector2.zero;
        panelRect.offsetMax  = new Vector2(0f, 240f);

        // 5 slot
        _slots = new Image[SlotCount];

        const float slotSize = 140f;
        const float spacing  = 24f;
        float totalW  = SlotCount * slotSize + (SlotCount - 1) * spacing;
        float startX  = -totalW * 0.5f + slotSize * 0.5f;

        for (int i = 0; i < SlotCount; i++)
        {
            var slotGo = new GameObject($"Slot_{i}");
            slotGo.transform.SetParent(panelGo.transform, false);

            var img    = slotGo.AddComponent<Image>();
            img.color  = Color.gray;

            var rect          = slotGo.GetComponent<RectTransform>();
            rect.anchorMin    = new Vector2(0.5f, 0.5f);
            rect.anchorMax    = new Vector2(0.5f, 0.5f);
            rect.pivot        = new Vector2(0.5f, 0.5f);
            rect.sizeDelta    = new Vector2(slotSize, slotSize);
            rect.anchoredPosition = new Vector2(startX + i * (slotSize + spacing), 0f);

            _slots[i] = img;
        }
    }

    void Refresh()
    {
        var preview = QueueManager.Instance.GetPreview();

        for (int i = 0; i < _slots.Length; i++)
        {
            if (i >= preview.Count)
            {
                _slots[i].color            = Color.gray;
                _slots[i].transform.localScale = Vector3.one;
                continue;
            }

            var color = GameManager.Instance.GetColor(preview[i]);
            color.a   = i == 0 ? 1f : 0.38f; // aktif = tam parlak, diğerleri soluk
            _slots[i].color = color;

            // Aktif slot biraz daha büyük
            _slots[i].transform.localScale = Vector3.one * (i == 0 ? 1.18f : 1f);
        }
    }
}
