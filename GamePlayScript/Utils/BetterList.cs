using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameScript
{
	/// <summary>
	/// This improved version of the System.Collections.Generic.List that doesn't release the buffer on Clear(), resulting in better performance and less garbage collection.
	/// </summary>
	public class BetterList<T>
	{
		#region Recyclable Buffers

		// How many recyclable buffers can be stored
		public int recyclableBuffersCapacity
		{
			set
			{
				recyclableBuffers.Capacity = value;
			}
			get
			{
				return recyclableBuffers.Capacity;
			}
		}
		// Store not using buffers in this, reuse them when need.
		private List<T[]> _recyclableBuffers = null;
		private List<T[]> recyclableBuffers
		{
			get
			{
				if (_recyclableBuffers == null)
				{
					_recyclableBuffers = new List<T[]>(5);
				}
				return _recyclableBuffers;
			}
		}

		public void ReleaseRecyclableBuffers()
		{
			recyclableBuffers.Clear();
		}

		private void RecycleBuffer(ref T[] buffer)
		{
			if (buffer != null)
			{
				if (recyclableBuffers.Count < recyclableBuffersCapacity)
				{
					recyclableBuffers.Add(buffer);
				}
				else
				{
					// discard a buffer
				}
				buffer = null;
			}
		}

		// New a buffer
		// If we can find a buffer that size is matching with required, reuse it. Otherwise, create a new one.
		private T[] FetchBuffer(int size)
		{
			for (int i = 0; i < recyclableBuffers.Count; i++)
			{
				var buffer = recyclableBuffers[i];
				if (buffer.Length == size)
				{
					for (int bufferIndex = 0; bufferIndex < buffer.Length; bufferIndex++)
					{
						buffer[bufferIndex] = default(T);
					}
					return buffer;
				}
			}
			return new T[size];
		}

		#endregion

		/// <summary>
		/// Direct access to the buffer. 
		/// </summary>
		private T[] buffer;

		/// <summary>
		/// Direct access to the buffer's size. 
		/// </summary>
		private int _size = 0;
		public int size
		{
			private set
			{
				_size = value;
			}
			get
			{
				return _size;
			}
		}

		/// <summary>
		/// For 'foreach' functionality.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			if (buffer != null)
			{
				for (int i = 0; i < size; ++i)
				{
					yield return buffer[i];
				}
			}
		}

		/// <summary>
		/// Convenience function. I recommend using .buffer instead.
		/// </summary>
		public T this[int i]
		{
			get { return buffer[i]; }
			set { buffer[i] = value; }
		}

		/// <summary>
		/// Helper function that expands the size of the array, maintaining the content.
		/// </summary>
		void AllocateMore()
		{
			if (buffer == null)
			{
				buffer = FetchBuffer(32);
			}
			else
			{
				T[] newBuffer = FetchBuffer(Mathf.Max(buffer.Length << 1, 32));
				if (size > 0)
				{
					buffer.CopyTo(newBuffer, 0);
				}
				RecycleBuffer(ref buffer);
				buffer = newBuffer;
			}
		}

		/// <summary>
		/// Trim the unnecessary memory, resizing the buffer to be of 'Length' size.
		/// Call this function only if you are sure that the buffer won't need to resize anytime soon.
		/// </summary>
		void Trim()
		{
			if (size > 0)
			{
				if (size < buffer.Length)
				{
					T[] newBuffer = FetchBuffer(size);
					for (int i = 0; i < size; i++)
					{
						newBuffer[i] = buffer[i];
					}
					RecycleBuffer(ref buffer);
					buffer = newBuffer;
				}
			}
			else
			{
				RecycleBuffer(ref buffer);
			}
		}

		/// <summary>
		/// Clear the array by resetting its size to zero. Note that the memory is not actually released.
		/// </summary>
		public void Clear() { size = 0; }

		/// <summary>
		/// Clear the array and release the used memory.
		/// </summary>

		public void Release()
		{
			size = 0;
			RecycleBuffer(ref buffer);
		}

		/// <summary>
		/// Add the specified item to the end of the list.
		/// </summary>
		public void Add(T item)
		{
			if (buffer == null || size == buffer.Length) AllocateMore();
			buffer[size++] = item;
		}

		/// <summary>
		/// Insert an item at the specified index, pushing the entries back.
		/// </summary>
		public void Insert(int index, T item)
		{
			if (buffer == null || size == buffer.Length) AllocateMore();

			if (index > -1 && index < size)
			{
				for (int i = size; i > index; --i) buffer[i] = buffer[i - 1];
				buffer[index] = item;
				++size;
			}
			else Add(item);
		}

		/// <summary>
		/// Returns 'true' if the specified item is within the list.
		/// </summary>
		public bool Contains(T item)
		{
			if (buffer == null) return false;
			for (int i = 0; i < size; ++i) if (buffer[i].Equals(item)) return true;
			return false;
		}

		/// <summary>
		/// Return the index of the specified item.
		/// </summary>
		public int IndexOf(T item)
		{
			if (buffer == null) return -1;
			for (int i = 0; i < size; ++i) if (buffer[i].Equals(item)) return i;
			return -1;
		}

		/// <summary>
		/// Remove the specified item from the list. Note that RemoveAt() is faster and is advisable if you already know the index.
		/// </summary>
		public bool Remove(T item)
		{
			if (buffer != null)
			{
				EqualityComparer<T> comp = EqualityComparer<T>.Default;

				for (int i = 0; i < size; ++i)
				{
					if (comp.Equals(buffer[i], item))
					{
						--size;
						buffer[i] = default(T);
						for (int b = i; b < size; ++b) buffer[b] = buffer[b + 1];
						buffer[size] = default(T);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Remove an item at the specified index.
		/// </summary>
		public void RemoveAt(int index)
		{
			if (buffer != null && index > -1 && index < size)
			{
				--size;
				buffer[index] = default(T);
				for (int b = index; b < size; ++b) buffer[b] = buffer[b + 1];
				buffer[size] = default(T);
			}
		}

		/// <summary>
		/// Remove an item from the end.
		/// </summary>
		public T Pop()
		{
			if (buffer != null && size != 0)
			{
				T val = buffer[--size];
				buffer[size] = default(T);
				return val;
			}
			return default(T);
		}

		/// <summary>
		/// Mimic List's ToArray() functionality, except that in this case the list is resized to match the current size.
		/// Returned array should be readonly.
		/// </summary>
		public T[] ToArray() { Trim(); return buffer; }

		/// <summary>
		/// List.Sort equivalent. Manual sorting causes no GC allocations.
		/// </summary>
		public void Sort(CompareFunc comparer)
		{
			for (int i = 0; i < size - 1; i++)
			{
				bool changed = false;
				for (int j = 0; j < size - 1; j++)
				{
					if (comparer(buffer[j], buffer[j + 1]) > 0)
					{
						T temp = buffer[j];
						buffer[j] = buffer[j + 1];
						buffer[j + 1] = temp;
						changed = true;
					}
				}
				if (changed == false)
				{
					break;
				}
			}
		}

		/// <summary>
		/// Comparison function should return -1 if left is less than right, 1 if left is greater than right, and 0 if they match.
		/// </summary>
		public delegate int CompareFunc(T left, T right);
	}
}