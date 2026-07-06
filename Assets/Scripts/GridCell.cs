using UnityEngine;

public class GridCell : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }
    public ProductType Product { get; private set; } = ProductType.None;
    public bool IsEmpty => Product == ProductType.None;

    SpriteRenderer _bg;
    SpriteRenderer _fill;

    public void Init(int row, int col, Sprite sprite)
    {
        Row = row;
        Col = col;

        _bg = gameObject.AddComponent<SpriteRenderer>();
        _bg.sprite = sprite;
        _bg.color = new Color(0.15f, 0.15f, 0.2f);

        var fillGo = new GameObject("Fill");
        fillGo.transform.SetParent(transform);
        fillGo.transform.localPosition = Vector3.zero;
        fillGo.transform.localScale    = Vector3.one * 0.82f;

        _fill = fillGo.AddComponent<SpriteRenderer>();
        _fill.sprite       = sprite;
        _fill.sortingOrder = 1;
        _fill.enabled      = false;
    }

    public void SetProduct(ProductType type, Color color)
    {
        Product       = type;
        _fill.enabled = type != ProductType.None;
        if (_fill.enabled) _fill.color = color;
    }

    public void Clear() => SetProduct(ProductType.None, Color.clear);
}
