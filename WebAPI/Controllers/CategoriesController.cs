using Business.Abstract;
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
    public class CategoriesController : ControllerBase
    {
        ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll() //GET
        {
            var result = _categoryService.GetAll(); // Get yerine GetAll() yaptık.
            if (result.Success)
            {
                return Ok(result); //result.Data sadece data verir, message ve success bilgisi vermez. tercihen sadece result yazdım
            }
            return BadRequest(result);//result.Message sadece mesaj verir, data ve success bilgisi vermez. tercihen sadece message yazdım
        }
    }
}
