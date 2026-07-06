using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public static QueueManager Instance { get; private set; }

    [SerializeField] int previewCount = 5;
    [SerializeField] ProductType[] pool = { ProductType.Parfum, ProductType.Ruj, ProductType.Sabun };

    readonly Queue<ProductType> _queue = new();

    public ProductType Current => _queue.Count > 0 ? _queue.Peek() : ProductType.None;

    // UI bu event'i dinleyerek kendini günceller
    public event System.Action OnChanged;

    void Awake() => Instance = this;

    public void Init(int fillCount = 15)
    {
        _queue.Clear();
        for (int i = 0; i < fillCount; i++) Enqueue();
        OnChanged?.Invoke();
    }

    public ProductType Consume()
    {
        if (_queue.Count == 0) return ProductType.None;
        var t = _queue.Dequeue();
        Enqueue();
        OnChanged?.Invoke();
        return t;
    }

    // UI için: ilk N ürünü listeler (sadece Current oynanabilir)
    public IReadOnlyList<ProductType> GetPreview()
    {
        var list = new List<ProductType>();
        int n = 0;
        foreach (var t in _queue)
        {
            if (n++ >= previewCount) break;
            list.Add(t);
        }
        return list;
    }

    void Enqueue() => _queue.Enqueue(pool[Random.Range(0, pool.Length)]);
}
