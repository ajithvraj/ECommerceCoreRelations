using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.Exceptions
{
    public class AppException : Exception
    {
       

        public AppException(string? message,int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; } 



    }
}
