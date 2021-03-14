using System;
using System.Collections.Generic;
using System.Text;

namespace Core.CrossCuttingConcerns.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key); // T Get hem liste hem tek bir object olabilir. Verilen key ile getirir.
        
        object Get(string key); // şeklinde de yapabiliriz ancak tip dönüşümü yapmak gerekir.

        void Add(string key, object value, int duration); // Her şeyin base'i olan object atarız.

        bool IsAdd(string key); // cache'de var mı metodu.

        void Remove(string key);

        void RemoveByPattern(string pattern);
    }
}
