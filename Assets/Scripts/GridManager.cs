using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] int   rows     = 5;
    [SerializeField] int   cols     = 6;
    [SerializeField] float cellSize = 0.75f;
    [SerializeField] float cellGap  = 0.08f;

    GridCell[,] _cells;

    // type = eşleşen ürün, count = temizlenen hücre sayısı
    public event System.Action<ProductType, int> OnMatchCompleted;

    void Awake() => Instance = this;

    public void BuildGrid()
    {
        _cells = new GridCell[rows, cols];

        float step    = cellSize + cellGap;
        float offsetX = (cols - 1) * step * 0.5f;
        float offsetY = (rows - 1) * step * 0.5f;
        var   sprite  = CreateSquareSprite();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var go = new GameObject($"Cell_{r}_{c}");
                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector3(c * step - offsetX, -(r * step - offsetY), 0f);
                go.transform.localScale    = Vector3.one * cellSize;

                var cell = go.AddComponent<GridCell>();
                cell.Init(r, c, sprite);

                var coll = go.AddComponent<BoxCollider2D>();
                coll.size = Vector2.one;

                _cells[r, c] = cell;
            }
        }
    }

    public bool TryPlace(int row, int col, ProductType type, Color color)
    {
        if (!_cells[row, col].IsEmpty) return false;

        _cells[row, col].SetProduct(type, color);
        CheckMatches(type);
        return true;
    }

    public bool IsFull()
    {
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                if (_cells[r, c].IsEmpty) return false;
        return true;
    }

    public GridCell GetCellAtWorld(Vector2 worldPos)
    {
        var hit = Physics2D.OverlapPoint(worldPos);
        return hit != null ? hit.GetComponent<GridCell>() : null;
    }

    // Yalnızca yeni yerleştirilen type için match kontrolü yapar.
    // Yerçekimi yok, bu yüzden cascade match mümkün değil.
    void CheckMatches(ProductType type)
    {
        var toRemove = new HashSet<(int r, int c)>();

        // Sadece yatay — raf temasıyla örtüşüyor
        for (int r = 0; r < rows; r++)
            for (int c = 0; c <= cols - 3; c++)
                if (IsRunOf(type, r, c, dr: 0, dc: 1, len: 3))
                    for (int i = 0; i < 3; i++) toRemove.Add((r, c + i));

        if (toRemove.Count == 0) return;

        foreach (var (r, c) in toRemove)
            _cells[r, c].Clear();

        OnMatchCompleted?.Invoke(type, toRemove.Count);
    }

    bool IsRunOf(ProductType type, int r, int c, int dr, int dc, int len)
    {
        for (int i = 0; i < len; i++)
            if (_cells[r + dr * i, c + dc * i].Product != type) return false;
        return true;
    }

    static Sprite CreateSquareSprite()
    {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
