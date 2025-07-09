namespace XArch.StructuredMarkdown
{
    using System.Collections.Generic;

    public class PropertyBag : Dictionary<string, object>, IDictionary<string, object>
    {
        public T? Get<T>(string key)
        {
            if (this.TryGetValue(key, out var value))
            {
                return (T?)value;
            }

            return default;
        }

        public void Set<T>(string key, T value)
        {
            this[key] = value;
        }
    }
}
