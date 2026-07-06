using System;
using UnityEngine;

[CreateAssetMenu(fileName = "OrderData", menuName = "ShelfPuzzle/Order Data")]
public class OrderData : ScriptableObject
{
    public OrderEntry[] orders;
}

[Serializable]
public class OrderEntry
{
    public ProductType type;
    public int count;
}
