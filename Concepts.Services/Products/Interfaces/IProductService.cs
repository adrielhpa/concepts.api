using Concepts.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concepts.Services.Products.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllProducts();
        Task<ProductDto> GetProductById(int id);
        Task<bool> CreateProduct(ProductDto product);
        Task<bool> UpdateProduct(ProductDto product);
        Task<bool> DeleteProduct(int id);
    }
}
