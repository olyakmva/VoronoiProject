using System;
using System.Collections.Generic;

namespace DiagramVoronoi
{
    /// <summary>
    /// Список с реализованными методами добавления в начало, в конец, в середину.
    /// </summary>
    public class BilateralList<T>
    {
        private class DoublyNode
        {
            public DoublyNode(T data)
            {
                Data = data;
            }
            public T Data { get; set; }
            public DoublyNode Previous { get; set; }
            public DoublyNode Next { get; set; }
        }

        public BilateralList()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }

        DoublyNode _head;
        DoublyNode _tail;
        int _count;
        public int Count { get { return _count; } }
        public T Head => _head.Data;
        public T Tail => _tail.Data;
        public void Add(T data)
        {
            DoublyNode node = new DoublyNode(data);

            if (_head == null)
                _head = node;
            else
            {
                _tail.Next = node;
                node.Previous = _tail;
            }
            _tail = node;
            _count++;
        }
        public void AddFirst(T data)
        {
            DoublyNode node = new DoublyNode(data);
            DoublyNode temp = _head;
            node.Next = temp;
            _head = node;
            if (_count == 0)
                _tail = _head;
            else
                temp.Previous = node;
            _count++;
        }
        public void Insert(int index, T data)
        {
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException();
            if (index == Count)
            {
                Add(data);
            }
            else if (index == 0)
            {
                AddFirst(data);
            }
            else
            {
                var current = _head;
                for (int i = 0; i < index; i++)
                {
                    current = current.Next;
                }
                DoublyNode add = new DoublyNode(data);
                add.Previous = current.Previous;
                add.Next = current;
                add.Previous.Next = add;
                current.Previous = add;

                _count++;
            }
        }
        public bool Remove(T data)
        {
            DoublyNode current = _head;

            while (current != null)
            {
                if (current.Data.Equals(data))
                {
                    break;
                }
                current = current.Next;
            }
            if (current != null)
            {
                if (current.Next != null)
                {
                    current.Next.Previous = current.Previous;
                }
                else
                {
                    _tail = current.Previous;
                }

                if (current.Previous != null)
                {
                    current.Previous.Next = current.Next;
                }
                else
                {
                    _head = current.Next;
                }
                _count--;
                return true;
            }
            return false;
        }

        public bool Contains(T data)
        {
            DoublyNode current = _head;
            while (current != null)
            {
                if (current.Data.Equals(data))
                    return true;
                current = current.Next;
            }
            return false;
        }
        public IEnumerable<T> GetEnumerator()
        {
            DoublyNode current = _head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }
        public IEnumerable<T> GetBackEnumerator()
        {
            DoublyNode current = _tail;
            while (current != null)
            {
                yield return current.Data;
                current = current.Previous;
            }
        }

        ////ToTest
        //public T[] ToArray()
        //{
        //    T[] values = new T[Count];
        //    DoublyNode<T> current = _head;
        //    int i = 0;
        //    while (current != null)
        //    {
        //        values[i] = current.Data;
        //        current = current.Next;
        //        i++;
        //    }
        //    return values;
        //}
    }
}
