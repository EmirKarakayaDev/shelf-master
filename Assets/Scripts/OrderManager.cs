using System.Collections.Generic;
using UnityEngine;

// LevelManager (-100) bittikten sonra çalışır
[DefaultExecutionOrder(-50)]
public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    readonly Dictionary<ProductType, int> _remaining = new();

    public event System.Action OnOrderUpdated;
    public event System.Action OnLevelComplete;

    void Awake()
    {
        Instance = this;
        LoadOrders(); // LevelManager.Awake garantili bitti (order=-100)
    }

    void Start()
    {
        GridManager.Instance.OnMatchCompleted += HandleMatch;
    }

    void OnDestroy()
    {
        if (GridManager.Instance != null)
            GridManager.Instance.OnMatchCompleted -= HandleMatch;
    }

    void LoadOrders()
    {
        _remaining.Clear();
        var levelData = LevelManager.Instance?.CurrentLevel;
        if (levelData == null) return;

        foreach (var entry in levelData.orders)
            _remaining[entry.type] = entry.count;

        OnOrderUpdated?.Invoke();
    }

    public int GetRemaining(ProductType type) =>
        _remaining.TryGetValue(type, out int val) ? Mathf.Max(0, val) : 0;

    public IEnumerable<(ProductType type, int remaining)> GetAllOrders()
    {
        foreach (ProductType t in System.Enum.GetValues(typeof(ProductType)))
            if (t != ProductType.None && _remaining.ContainsKey(t))
                yield return (t, Mathf.Max(0, _remaining[t]));
    }

    void HandleMatch(ProductType type, int count)
    {
        if (!_remaining.ContainsKey(type)) return;

        _remaining[type] -= count;
        OnOrderUpdated?.Invoke();

        if (IsComplete()) OnLevelComplete?.Invoke();
    }

    bool IsComplete()
    {
        foreach (var val in _remaining.Values)
            if (val > 0) return false;
        return true;
    }
}
