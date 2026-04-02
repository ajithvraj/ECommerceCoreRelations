using ECommerceCore.Domain.Enities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.Interfaces.CustomerInterface
{
    public interface ICustomerRepository
    {


      Task<Customer>AddCustomerAsync(Customer customer);
        Task<Customer?>GetCustomerByIdAsync(int id);
        Task<Customer?>GetCustomerByEmailAsync(string email);
        Task<bool>ExistsCustomerByEmailAsync(string email);

        
        Task<Customer?>GetCustomerByNameAsync(string name );

        





   

       


    }
}
