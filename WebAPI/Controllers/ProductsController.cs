using Business.Abstract;
using Business.Concrete;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        //Dependency chain (bağımlılık zinciri) yerine Field tanımlayarak yaptık;
        //Field, Loosely coupled (gevşek bağlılık) olmaktadır.
        //Ioc : Inversion of Control : Değişimin kontrolü
        IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll() //GET
        {
            //Swagger - Hazır dökümantasyon imkanı sunar.
            var result = _productService.GetAll(); // Get yerine GetAll() yaptık.
            if (result.Success)
            {
                return Ok(result); //result.Data sadece data verir, message ve success bilgisi vermez. tercihen sadece result yazdım
            }
            return BadRequest(result);//result.Message sadece mesaj verir, data ve success bilgisi vermez. tercihen sadece message yazdım
        }

        // İki HttpGet olunca:
        // [HttpGet] 500 hatası verir, birinci yöntem [HttpGet(id)] olarak yazıp id alacağını belirtmektir.
        // İkinci yöntem parantezli tırnak (" ") içinde isim vermektir. [HttpGet("getall")]
        // ikinci yöntem tercih edilir
        [HttpGet("getbyid")]
        public IActionResult GetById(int id) // Get yerine GetById() yaptık.
        {
            var result = _productService.GetById(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("add")] // silme ve güncelleme için de çoğunlukla bu kullanılır. // tercihen silme için httpdelete güncelleme için put kullanabilirsin.
        public IActionResult Add(Product product) //POST ekleyeciğimiz şey product olduğu için belirttik // Post yerine Add() yaptık.
        {
            var result = _productService.Add(product);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
