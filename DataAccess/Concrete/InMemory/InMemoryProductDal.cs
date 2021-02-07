using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataAccess.Concrete.InMemory
{
    public class InMemoryProductDal : IProductDal
    {
        List<Product> _products;
        public InMemoryProductDal()
        {
            // Oracle, Sql Server, Postgres, MongoDb gibi veri tabanından geliyormuş gibi bizim için bellekte simüle eder.
            _products = new List<Product> {
                new Product{ProductId=1,CategoryId=1,ProductName="Bardak",UnitPrice=15,UnitsInStock=15},
                new Product{ProductId=2,CategoryId=1,ProductName="Kamera",UnitPrice=500,UnitsInStock=3},
                new Product{ProductId=3,CategoryId=2,ProductName="Telefon",UnitPrice=1500,UnitsInStock=2},
                new Product{ProductId=4,CategoryId=2,ProductName="Klavye",UnitPrice=150,UnitsInStock=65},
                new Product{ProductId=5,CategoryId=2,ProductName="Fare",UnitPrice=85,UnitsInStock=1}
            };
        }

        public List<Product> GetAll()
        {
            return _products;
        }

        public List<Product> GetAllByCategory(int categoryId)
        {
            return _products.Where(p => p.CategoryId == categoryId).ToList();
        }

        public void Add(Product product)
        {
            _products.Add(product);
        }

        public void Delete(Product product) // Silinecek ürünün Heap'teki referans numarası 201 olsun.
        {
            // LINQ bilmiyor olsaydık, silmek için şöyle yapardık.

            //// _products.Remove(product); Bu şekilde asla listeden silemeyiz.

            //Product productToDelete; // Silinecek ürün diyelim.

            //foreach (var p in _products) // Eşleşen ürünü bulmak için listeyi döneriz ve p takma adı verdiğimiz, ID'si uyuşan ürün p'yi silinecek ürüne aktarırız.
            //{
            //    if (product.ProductId == p.ProductId)
            //    {
            //        productToDelete = p;
            //    }
            //}

            //_products.Remove(productToDelete); // Ve silinecek ürünü remove ile _product global ismini verdiğimiz için parametreye yazıp silebiliriz. // Hided

            // LINQ - Language Integrated Query - Dile Gömülü Sorgulama
            // LINQ ile Hided kısmından daha kısa yazarız
            // Lambda => (Mesela burada her p için anlamında kullanalım)
            Product productToDelete = _products.SingleOrDefault(p=> p.ProductId == product.ProductId);
            //                           foreach yapar          p=> takma ismi verir ve if parantezine yazılan kural.
            // Hided kısmındaki foreach ile aynı işi yapar. Çok daha kolay.
            _products.Remove(productToDelete);
            // Ve sildik.
        }

        public void Update(Product product) // Bu product kullanıcının ekranından gelen data.
        {
            // Gönderdiğim ürün ID'sine sahip olan listedeki ürünü bul.
            Product productToUpdate = _products.SingleOrDefault(p => p.ProductId == product.ProductId);
            productToUpdate.ProductName = product.ProductName;
            productToUpdate.CategoryId = product.CategoryId;
            productToUpdate.UnitPrice = product.UnitPrice;
            productToUpdate.UnitsInStock = product.UnitsInStock;
            // ProductId'ye dokunmadık çünkü o aynı kalıyor, o ID'nin diğer bilgilerini güncelledik.
        }

        public List<Product> GetAll(Expression<Func<Product, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public Product Get(Expression<Func<Product, bool>> filter)
        {
            throw new NotImplementedException();
        }
    }
}
