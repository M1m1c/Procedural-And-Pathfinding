using System;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortDown(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortUp(items[0]);
        return firstItem;
    }

    /// <summary>
    /// sorts the item to a higher index (closer to the end of the array),
    /// based on if its priority is lower than the child indexes after it.
    /// </summary>
    private void SortUp(T item)
    {
        while (true)
        {
            var childIndexLeft = item.HeapIndex * 2 + 1;
            var childIndexRight = item.HeapIndex * 2 + 2;
            var swapIndex = 0;

            if (!(childIndexLeft < currentItemCount)) { return; }

            swapIndex = childIndexLeft;
            if (childIndexRight < currentItemCount && 
                items[childIndexLeft].CompareTo(items[childIndexLeft]) < 0)
            {
                swapIndex = childIndexRight;
            }

            if (!(item.CompareTo(items[swapIndex]) < 0)){ return; }
            Swap(item, items[swapIndex]);
        }
    }

    /// <summary>
    /// sorts the item to a lower index (closer to the start of the array), 
    /// based on if its priority is higher than the parent index before it.
    /// </summary>
    void SortDown(T item)
    {
        var parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else { break; }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int itemAHeapindex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAHeapindex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}
