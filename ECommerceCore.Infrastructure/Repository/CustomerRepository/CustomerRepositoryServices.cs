using ECommerceCore.Application.Interfaces.CustomerInterface;
using ECommerceCore.Domain.Enities;
using ECommerceCore.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Infrastructure.Repository.CustomerRepository
{
    public class CustomerRepositoryServices : ICustomerRepository
    {
        private readonly AppDbContext _db;

        public CustomerRepositoryServices(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            await _db.Customers.AddAsync(customer);
            await _db.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _db.Customers.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> ExistsCustomerByEmailAsync(string email)
        {
            return await _db.Customers.AnyAsync(x => x.Email == email);
        }

        public async Task<Customer?> GetCustomerByNameAsync(string name)
        {
            return await _db.Customers.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<Customer?> GetCustomerByRefreshTokenAsync(string refreshToken)
        {
            return await _db.Customers.FirstOrDefaultAsync(c => c.RefreshToken == refreshToken);
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
            if (customer == null) return false;

            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _db.Customers
                .Where(c => c.Role != "Admin")
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
