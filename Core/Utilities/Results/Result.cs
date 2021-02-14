using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Results
{
    public class Result : IResult
    {
        public Result(bool success, string message) : this(success) // tek parametre yollandığında aşağıdaki ctor içindeki success'i çalıştır demek.
        {
            Message = message;
        }

        // ctor Over Loading
        public Result(bool success) // IProductService içinde Add metodu artık sadece Success bilgisi (true) verebilir. Message yok.
        {
            Success = success;
        }

        public bool Success { get; }

        public string Message { get; }
    }
}
