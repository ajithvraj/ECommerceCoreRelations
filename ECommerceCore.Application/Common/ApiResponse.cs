using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.Common
{
    public class ApiResponse<T>
    {

        public bool Success { get; set; }
        public string Message { get; set; }

        public T Data { get; set; } 

        public object Errors { get; set; } 


        public static ApiResponse<T> SuccessResult (T data , string message = "Success")
        {

            return new ApiResponse<T>
            {

                Success = true,
                Message = message,
                Data = data

            };

        }

        public static ApiResponse<T> Failure(string message , object errors = null)
        {

            return new ApiResponse<T>
            {

                Success = false,
                Message = message,
                Errors = errors

            };

        }




    }
}
