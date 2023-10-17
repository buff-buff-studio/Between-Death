using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Refactor.Misc
{
    /// <summary>
    /// Represents a container entry
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    public class PersistentDictionaryPair<TK, TV>
    {
        public TK Key;
        public TV Value;
    }
    
    /// <summary>
    /// Container is a lightweight Dictionary/Map-like collection.
    /// 
    /// - Customizable bucket count
    /// - No security checks (Higher performance)
    /// - No exceptions (Higher performance and safer)
    /// - Short Header (Compared to Dictionary)
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    public interface IPersistentDictionary<in TK, TV>
    {
        int Count { get; }
        int Capacity { get; }
        bool ContainsKey(TK key);
        
        TV Get(TK key);
        bool TryGetValue(TK key, out TV value);
        void Set(TK key, TV value);
        bool Remove(TK key, out TV value);
        void Clear();
    }
    
    [Serializable]
    public class PersistentDictionary<TK, TV> : IPersistentDictionary<TK, TV>, IEnumerable<PersistentDictionaryPair<TK, TV>>
    {
        private const int _DEFAULT_MIN_CAPACITY = 10;
        private const int _DEFAULT_MIN_BUCKETS = 10;
        
        [Serializable]
        private struct Entry
        {
            public TK Key;
            public int Next;
            public TV Value;
        }
        
        [SerializeField, HideInInspector]
        private Entry[] _entries;
        [SerializeField, HideInInspector]
        private int[] _buckets;
        [SerializeField, HideInInspector]
        private int _used;
        [SerializeField, HideInInspector]
        private int _nextFree = -2;
        [SerializeField, HideInInspector]
        private int _freeCount;
        [SerializeField, HideInInspector]
        private int _version;

        /// <summary>
        /// Returns the count of values into the container
        /// </summary>
        public int Count => _used - _freeCount;
        
        /// <summary>
        /// Returns the allocated capacity for container items
        /// </summary>
        public int Capacity => _entries.Length;
        
        /// <summary>
        /// Returns the version of the container
        /// </summary>
        public int Version => _version;

        /// <summary>
        /// Gets or sets a value into the container.
        /// Nor key nor value are null-checked
        /// </summary>
        /// <value></value>
        public TV this[TK key] 
        {
            get => Get(key);
            set => Set(key, value);
        }

        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="buckets"></param>
        public PersistentDictionary(int capacity = _DEFAULT_MIN_CAPACITY, int buckets = _DEFAULT_MIN_BUCKETS)
        {
            _buckets = new int[Math.Max(_DEFAULT_MIN_BUCKETS, buckets)];
            _entries = new Entry[Math.Max(_DEFAULT_MIN_CAPACITY, capacity)];
            for (int i = 0; i < _buckets.Length; i++)
                _buckets[i] = -1;
        }

        #region Methods
        /// <summary>
        /// Checks if container contains a certain key.
        /// The key is not null-checked
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TK key)
        {
            for (int entry = _buckets[(key.GetHashCode() & 0x7FFFFFFF) % _buckets.Length]; entry > -1; entry = _entries[entry].Next)
            {
                if(_entries[entry].Key.Equals(key))
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Tries to get a value from the container.
        /// The key is not null-checked
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TV Get(TK key)
        {
            for (int entry = _buckets[(key.GetHashCode() & 0x7FFFFFFF) % _buckets.Length]; entry > -1; entry = _entries[entry].Next)
            {
                if(_entries[entry].Key.Equals(key))
                {
                    return _entries[entry].Value;
                }
            }

            return default;
        }

        /// <summary>
        /// Tries to get a value from the container, returning if the value was found or not.
        /// The key is not null-checked
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TK key, out TV value)
        {
            for (int entry = _buckets[(key.GetHashCode() & 0x7FFFFFFF) % _buckets.Length]; entry > -1; entry = _entries[entry].Next)
            {
                if(_entries[entry].Key.Equals(key))
                {
                    value = _entries[entry].Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Sets or replace a value into the container.
        /// Nor key nor value are null-checked
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(TK key, TV value)
        {
            int n = (key.GetHashCode() & 0x7FFFFFFF) % _buckets.Length;
            int p = _buckets[n];

            if(p == -1)
            {
                
                int pos = _GetNextFreeIndex();
                _entries[pos].Key = key;
                _entries[pos].Value = value;
                _entries[pos].Next = -1;
                _buckets[n] = pos;
                _version++;
                return;
            }
            
            while(true)
            {
                if(_entries[p].Key.Equals(key))
                {
                    _entries[p].Value = value;
                    _version++;
                    return;
                }
                
                if(_entries[p].Next == -1)
                {
                    int pos = _GetNextFreeIndex();
                    _entries[pos].Key = key;
                    _entries[pos].Value = value;
                    _entries[pos].Next = -1;
                    _entries[p].Next = pos;
                    _version++;
                    return;
                }
                
                p = _entries[p].Next;
            }
        }

        /// <summary>
        /// Removes a key from container.
        /// The key is not null-checked
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(TK key, out TV value)
        {
            int n = (key.GetHashCode() & 0x7FFFFFFF) % _buckets.Length;
            int p = _buckets[n];
            if (p == -1)
            {
                value = default;
                return false;
            }

            int prev = -1;
            while(p != -1)
            {
                if(_entries[p].Key.Equals(key))
                {
                    if (prev == -1)
                        _buckets[n] = -1;
                    else
                        _entries[prev].Next = _entries[p].Next;

                    if(_freeCount == 0) 
                        _entries[p].Next = -2; 
                    else
                        _entries[p].Next = _nextFree; 

                    _nextFree = -(p + 2);
                    _freeCount ++;
                    
                    value = _entries[p].Value;
                    _entries[p].Value = default;
                    _version++;
                    return true;
                }

                prev = p;
                p = _entries[p].Next;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Clears the container keeping its previous capacity
        /// </summary>
        public void Clear()
        {
            if (_used <= 0) return;
            for (int i = 0; i < _buckets.Length; i++)
                _buckets[i] = -1;
            Array.Clear(_entries, 0, _used);
            _nextFree = -2;
            _freeCount = 0;
            _used = 0;
            _version++;
        }
        #endregion
        
        private int _GetNextFreeIndex()
        {
            if (_freeCount <= 0)
            {
                if(_used + 1 > _entries.Length)
                    Array.Resize(ref _entries, _entries.Length * 2);
                return _used++;
            }
            
            int free = -(_nextFree + 2);
            _nextFree = _entries[free].Next;
            if (_freeCount == 1)
                _nextFree = -2;
            _freeCount--;
            return free;
        }
        
        #region Iterator
        public IEnumerable<TK> Keys => _GetKeys();
        public IEnumerable<TV> Values => _GetValues();
        public IEnumerator<PersistentDictionaryPair<TK, TV>> Pairs => GetEnumerator();

        public IEnumerator<PersistentDictionaryPair<TK, TV>> GetEnumerator()
        {
            PersistentDictionaryPair<TK, TV> pair = new PersistentDictionaryPair<TK, TV>();
            for(int i = 0; i < _used; i ++)
            {
                if(_entries[i].Next < -1)
                    continue;

                pair.Key = _entries[i].Key;
                pair.Value = _entries[i].Value;
                yield return pair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            PersistentDictionaryPair<TK, TV> pair = new PersistentDictionaryPair<TK, TV>();
            for(int i = 0; i < _used; i ++)
            {
                if(_entries[i].Next < -1)
                    continue;

                pair.Key = _entries[i].Key;
                pair.Value = _entries[i].Value;
                yield return pair;
            }
        }

        private IEnumerable<TK> _GetKeys()
        {
            for(int i = 0; i < _used; i ++)
            {
                if(_entries[i].Next < -1)
                    continue;

                yield return _entries[i].Key;
            }
        }

        private IEnumerable<TV> _GetValues()
        {
            for(int i = 0; i < _used; i ++)
            {
                if(_entries[i].Next < -1)
                    continue;

                yield return _entries[i].Value;
            }
        }
        #endregion
    }
}