using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using System.Collections;

namespace WinSwag
{
    /// <summary>
    /// Provides a strongly typed dictionary interface over an <see cref="ApplicationDataContainer"/>.
    /// Values are serialized as JSON.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SettingsDictionary<T> : IDictionary<string, T>
    {
        public ApplicationDataContainer Container { get; }

        private T Deserialize(object o) => JsonConvert.DeserializeObject<T>((string)o);
        private object Serialize(T o) => JsonConvert.SerializeObject(o);

        public ICollection<string> Keys => Container.Values.Keys;

        public ICollection<T> Values => Container.Values.Values.Select(Deserialize).ToList();

        public int Count => Container.Values.Count;

        public bool IsReadOnly => Container.Values.IsReadOnly;

        public T this[string key]
        {
            get => Deserialize(Container.Values[key]);
            set => Container.Values[key] = Serialize(value);
        }

        public SettingsDictionary(ApplicationDataContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public void Add(string key, T value) => Container.Values.Add(key, Serialize(value));

        public bool ContainsKey(string key) => Container.Values.ContainsKey(key);

        public bool Remove(string key) => Container.Values.Remove(key);

        public bool TryGetValue(string key, out T value)
        {
            if (Container.Values.TryGetValue(key, out var o))
            {
                value = Deserialize(o);
                return true;
            }
            value = default(T);
            return false;
        }

        public void Add(KeyValuePair<string, T> item) => Container.Values.Add(item.Key, Serialize(item.Value));

        public void Clear() => Container.Values.Clear();

        bool ICollection<KeyValuePair<string, T>>.Contains(KeyValuePair<string, T> item) =>
            Container.Values.Contains(new KeyValuePair<string, object>(item.Key, Serialize(item.Value)));

        void ICollection<KeyValuePair<string, T>>.CopyTo(KeyValuePair<string, T>[] array, int arrayIndex) =>
            throw new NotSupportedException();

        bool ICollection<KeyValuePair<string, T>>.Remove(KeyValuePair<string, T> item) =>
            throw new NotSupportedException();

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            foreach (var kvp in Container.Values)
                yield return new KeyValuePair<string, T>(kvp.Key, Deserialize(kvp.Value));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
