namespace System.Collections.Generic
{
    public static class CollectionExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> valueFactory)
        {
            return dict.TryGetValue(key, out var value)
                ? value
                : dict[key] = (valueFactory ?? throw new ArgumentNullException(nameof(valueFactory)))(key);
        }
    }
}
