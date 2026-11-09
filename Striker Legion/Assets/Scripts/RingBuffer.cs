using System.Collections.Generic;
using UnityEngine;

public class RingBuffer<T>
{
	private T[] buffer;
	private int head = 0;   // 最も古いデータの位置
	private int tail = 0;   // 次に書き込む位置
	private int capacity;
	private int count = 0;  // 現在入っている要素数

	public RingBuffer(int capacity)
	{
		this.capacity = capacity;
		buffer = new T[capacity];
	}

	// 要素の追加（容量がいっぱいの場合は、最も古いデータが自動で上書きされる）
	public void Add(T item)
	{
		buffer[tail] = item;
		tail = (tail + 1) % capacity;

		if (count < capacity)
		{
			count++;
		}
		else
		{
			// 満杯のときは最も古いデータが消えるため、headを進める
			head = (head + 1) % capacity;
		}
	}

	// 現在バッファにたまっている要素数
	public int Count => count;

	// 最大容量
	public int Capacity => capacity;

	// インデックスを指定して要素を参照する（0が一番古いデータ、Count - 1が一番新しいデータ）
	public T GetAt(int index)
	{
		if (index < 0 || index >= count)
		{
			throw new System.ArgumentOutOfRangeException(nameof(index));
		}
		int actualIndex = (head + index) % capacity;
		return buffer[actualIndex];
	}

	// バッファ内の全要素を古い順に列挙して参照する
	public IEnumerable<T> GetAllItems()
	{
		for (int i = 0; i < count; i++)
		{
			yield return GetAt(i);
		}
	}

	// バッファを初期状態に戻す
	public void Clear()
	{
		head = 0;
		tail = 0;
		count = 0;
		System.Array.Clear(buffer, 0, buffer.Length);
	}
}

