using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Color parfumColor = new Color(0.45f, 0.65f, 1.00f);
    [SerializeField] Color rujColor    = new Color(1.00f, 0.45f, 0.65f);
    [SerializeField] Color sabunColor  = new Color(0.45f, 1.00f, 0.65f);

    bool _gameOver;

    public event System.Action OnWin;
    public event System.Action OnLose;

    void Awake() => Instance = this;

    void Start()
    {
        GridManager.Instance.BuildGrid();
        QueueManager.Instance.Init();
        OrderManager.Instance.OnLevelComplete += HandleWin;
    }

    void Update()
    {
        if (_gameOver) return;
        if (!Input.GetMouseButtonDown(0)) return;

        var worldPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var cell = GridManager.Instance.GetCellAtWorld(worldPos);
        if (cell == null || !cell.IsEmpty) return;

        var type = QueueManager.Instance.Current;
        if (type == ProductType.None) return;

        bool placed = GridManager.Instance.TryPlace(cell.Row, cell.Col, type, GetColor(type));
        if (!placed) return;

        QueueManager.Instance.Consume();

        if (GridManager.Instance.IsFull())
            HandleLose();
    }

    void HandleWin()
    {
        _gameOver = true;
        OnWin?.Invoke();
    }

    void HandleLose()
    {
        _gameOver = true;
        OnLose?.Invoke();
    }

    public Color GetColor(ProductType type) => type switch
    {
        ProductType.Parfum => parfumColor,
        ProductType.Ruj    => rujColor,
        ProductType.Sabun  => sabunColor,
        _                  => Color.gray
    };
}
