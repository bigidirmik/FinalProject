using Core.Utilities.IoC;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using System.Linq;

namespace Core.CrossCuttingConcerns.Caching.Microsoft
{
    public class MemoryCacheManager : ICacheManager
    {
        // Çok önemli!
        // Adapter Pattern: Adaptasyon deseni; metotlar içinde Set, Get, TrygetValue zaten hazrıda var ancak biz bunu projeye direkt yazmak yerine
        // metotlar haline getiriyoruz, çünkü Cache Manager'ımızı örneğin MicrosoftCache yerine Redis'e geçirince direkt yazılan tüm hazır operasyonları
        // gidip tek tek değiştirmek zorunda kalırız. Fakat bu şekilde metot yapınca o hazır operasyonları Add Remove gibi kendimize adapte ediyoruz.
        // Sistemi değiştirmek istediğimizde de tek bir yerde değişiklik yaparak diğer yeni sistemi aktif ediyoruz. Hazırlanan iki sistem arasında böylece geçiş yapabiliyoruz.

        IMemoryCache _memoryCache;

        public MemoryCacheManager()
        {
            _memoryCache = ServiceTool.ServiceProvider.GetService<IMemoryCache>(); // using microsoft.extensions.dependencyinjection elle ekledik!
        }

        public void Add(string key, object value, int duration)
        {
            _memoryCache.Set(key, value, TimeSpan.FromMinutes(duration));
        }

        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public object Get(string key)
        {
            return _memoryCache.Get(key);
        }

        public bool IsAdd(string key)
        {
            return _memoryCache.TryGetValue(key, out _); // out parametresi zorunlu ancak biz sadece bool döndürmesini istiyoruz, bu şekilde out boş verilmiş oldu. Bir şey döndürme demiş olduk.
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public void RemoveByPattern(string pattern)
        {
            var cacheEntriesCollectionDefinition = typeof(MemoryCache).GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cacheEntriesCollection = cacheEntriesCollectionDefinition.GetValue(_memoryCache) as dynamic;
            List<ICacheEntry> cacheCollectionValues = new List<ICacheEntry>();

            foreach (var cacheItem in cacheEntriesCollection)
            {
                ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);
                cacheCollectionValues.Add(cacheItemValue);
            }

            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = cacheCollectionValues.Where(d => regex.IsMatch(d.Key.ToString())).Select(d => d.Key).ToList();

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
            }
        }
    }
}
