using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.DataAccess
{
    // Generic Constraint: Yalnızca Entities içindeki Concrete elemanları T'yi kullanabilsin istiyoruz.
    // where ile başlıyoruz
    // class: T'nin sadece bir referans tip olabileceği şartı
    // IEntity: T'nin sadece Entity ve bunu implement eden bir nesne olabileceği şartı
    // new(): T'nin sadece new'lenebilir olacağı şartı. Böylece IEntity'de olamaz ve geriye sadece Concrete elemanları kalır.
    public interface IEntityRepository<T> where T:class,IEntity,new()
    {
        List<T> GetAll(Expression<Func<T,bool>> filter=null);
        T Get(Expression<Func<T, bool>> filter);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
