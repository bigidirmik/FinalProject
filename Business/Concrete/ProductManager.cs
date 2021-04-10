using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;
        ICategoryService _categoryService; // Başka bir kuralı enjekte ederken servisin kendisini kullanırız.

        // Kategori sayısı 15'i geçtiyse ekleme yapma kuralı için categoryDal değil servisi enjekte ettik.
        public ProductManager(IProductDal productDal,ICategoryService categoryService)
        {
            _productDal = productDal;
            _categoryService = categoryService;
        }


        // business codes (her şey geçerli ise ekle, değil ise ekleme gibi)
        // Magic strings
        //validation (business code ile ayrıdır!)

        //ProductValidator'da yazıldığı için sildik. (comment'ledik)
        //if (product.ProductName.Length<2)
        //{
        //    return new ErrorResult(Messages.ProductNameInvalid);
        //}

        //ProductValidation çalıştırmak için yeni kod ise bu. (Bu da refactor edilecek)
        //var context = new ValidationContext<Product>(product);
        //ProductValidator productValidator = new ProductValidator();
        //var result = productValidator.Validate(context);
        //if (!result.IsValid)
        //{
        //    throw new ValidationException(result.Errors);
        //}

        //üstteki kodu core'daki concern.validation içine aldık, objektif parametreler ekledik (refactor ettik)
        //ValidationTool.Validate(new ProductValidator(), product);
        //Bu koda da gerek kalmadı, metot üzerinde ValidationAspect olarak aynı şeyi yaptık.
        // Claim
        
        //[SecuredOperation("product.add,admin")]
        [ValidationAspect(typeof(ProductValidator))]
        [CacheRemoveAspect("IProductService.Get")] // sadece Get yazarsak cache içinde get içeren her şeyi; farklı bir Service yani Manager'dan olsa bile siler.
        public IResult Add(Product product)
        {
            IResult result = BusinessRules.Run(
                CheckIfProductNameExists(product.ProductName),
                CheckIfProductCountOfCategoryCorrect(product.CategoryId),
                CheckIfCategoryLimitExceded()
                );
            
            if (result != null)
            {
                return result;
            }
            
            _productDal.Add(product);

            return new SuccessResult(Messages.ProductAdded);
        }

        [CacheAspect] // key: Business.Concrete.ProductManager.GetAll, parametre var ise parantez içinde yanına eklenir + value:
        public IDataResult<List<Product>> GetAll()
        {
            // İş kodları (if şöyle ise...)
            if (DateTime.Now.Hour == 22)
            {
                return new ErrorDataResult<List<Product>>(Messages.MaintenanceTime);
            }
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(), Messages.ProductsListed);
        }

        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.CategoryId == id));
        }

        [CacheAspect]
        //[PerformanceAspect(5)] // bu aspect içinde stopwatch bizden interval değer istemişti, inteval verdiğimiz 5'tir. çalışması 5 saniyeyi geçerse uyarır.
        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductId == productId));
        }

        public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.UnitPrice >= min && p.UnitPrice <= max));
        }

        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            if (DateTime.Now.Hour == 23)
            {
                return new ErrorDataResult<List<ProductDetailDto>>(Messages.MaintenanceTime);
            }
            return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails());
        }

        [ValidationAspect(typeof(ProductValidator))]
        [CacheRemoveAspect("IProductService.Get")] // sadece Get yazarsak cache içinde get içeren her şeyi; farklı bir Service yani Manager'dan olsa bile siler.
        public IResult Update(Product product)
        {
            if (CheckIfProductCountOfCategoryCorrect(product.CategoryId).Success)
            {
                _productDal.Update(product);

                return new SuccessResult(Messages.ProductAdded);
            }
            return new ErrorResult();
        }

        //[TransactionScopeAspect] // işlem hata verirse eklemez geri alır.
        public IResult AddTransactionalTest(Product product)
        {
            Add(product);
            if (product.UnitPrice < 10)
            {
                throw new Exception("");
            }
            Add(product);
            return null;
        }



        // Business Codes


        //Bir kategoride en fazla 10 ürün olabilir .
        private IResult CheckIfProductCountOfCategoryCorrect(int categoryId)
        {
            //.GetAll(p => p.CategoryId).Count şu demektir:
            //Select Count(*) from Products where categoryId=1
            var result = _productDal.GetAll(p => p.CategoryId == categoryId).Count;
            if (result >= 10)
            {
                return new ErrorResult(Messages.ProductCountOfCategoryError);
            }
            return new SuccessResult();
        }

        //Aynı isimde ürün olamaz.
        private IResult CheckIfProductNameExists(string productName)
        {
            var result = _productDal.GetAll(p => p.ProductName == productName).Any(); // şuna uyan kayıt var mı? (ANY) //.Count'da olur // if result==null da olur
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }
            return new SuccessResult();
        }

        private IResult CheckIfCategoryLimitExceded()
        {
            var result = _categoryService.GetAll();
            if (result.Data.Count > 15)
            {
                return new ErrorResult(Messages.CategoryLimitExceded);
            }
            return new SuccessResult();
        }
    }
}
