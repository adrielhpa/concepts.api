using AutoMapper;
using Concepts.Domain;
using Concepts.Domain.DTOs;
using Concepts.Repository;
using Concepts.Repository.ProductsRepo;
using Concepts.Services.Helpers;
using Concepts.Services.Products.Interfaces;
using Concepts.Services.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestType = Concepts.Services.Helpers.RequestType;

namespace Concepts.Services.Products
{
    public class ProductService : IProductService
    {
        private ProductRepository _productRepository;
        private readonly IMapper _mapper;
        private AmazonSQS _sqs;

        public ProductService(ProductRepository productRepository, IMapper mapper, IDistributedCache cache, AmazonSQS sqs)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _sqs = sqs;
        }

        public async Task<List<ProductDto>> GetAllProducts()
        {
            var results = await _productRepository.GetAll();
            return _mapper.Map<List<ProductDto>>(results);
        }

        public async Task<ProductDto> GetProductById(int id)
        {
            var product = await _productRepository.GetById(id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> CreateProduct(ProductDto product)
        {
            var productToAdd = _mapper.Map<ProductDto, Product>(product);
            var result = await _productRepository.Create(productToAdd);

            ResponseData response = new()
            {
                RequestType = RequestType.POST,
                Message = result != null ? "Product created successfully!" : "Error to create product!",
                ProductData = _mapper.Map<Product, ProductDto>(result),
                IsValid = result != null
            };
            await _sqs.SendMessage(response);

            return result != null;
        }

        public async Task<bool> UpdateProduct(ProductDto product)
        {
            var findedProduct = await _productRepository.GetById(product.Id);
            if (findedProduct is null) throw new NullReferenceException();

            findedProduct.Name = product.Name;
            findedProduct.Brand = product.Brand;
            findedProduct.Description = product.Description;
            findedProduct.Price = product.Price;

            var result = await _productRepository.Update(findedProduct);

            ResponseData response = new()
            {
                RequestType = RequestType.PUT,
                Message = result != null ? "Product updated successfully!" : "Error to update product!",
                ProductData = _mapper.Map<Product, ProductDto>(result),
                IsValid = result != null
            };
            await _sqs.SendMessage(response);

            return result != null;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await GetProductById(id);
            var result = await _productRepository.Delete(id);

            ResponseData response = new()
            {
                RequestType = RequestType.DELETE,
                Message = result ? "Product deleted successfully!" : "Error to delete product!",
                ProductData = product,
                IsValid = result
            };
            await _sqs.SendMessage(response);

            return result;
        }
    }
}
