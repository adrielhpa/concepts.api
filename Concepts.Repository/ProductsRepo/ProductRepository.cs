using Concepts.Domain;
using Concepts.Repository.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concepts.Repository.ProductsRepo
{
    public class ProductRepository : IBaseRepository<Product>
    {
        private readonly ConceptContext _context;
        private IDistributedCache _cache;
        public ProductRepository(ConceptContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<Product>> GetAll()
        {
            var recordKey = $"Concepts_Products_{DateTime.Now.ToString("MMddyyyy")}";
            var cacheData = await _cache.GetRecordAsync<List<Product>>(recordKey);
            if (cacheData is not null) return cacheData;
            else
            {
                var result = await _context.Products.Include(x => x.CreatedByUser).ToListAsync();
                await _cache.SetRecordAsync(recordKey, result);
                return result;
            }
        }

        public async Task<Product> GetById(int id)
        {
            var recordKey = $"Concepts_Products_{DateTime.Now.ToString("MMddyyyy")}";
            var cacheData = await _cache.GetRecordAsync<List<Product>>(recordKey);

            if (cacheData is not null)
            {
                var cacheProduct = cacheData.Find(p => p.Id == id);
                if (cacheProduct is not null) return cacheProduct;
            }

            var product = await _context.Products.Where(x => x.Id == id).Include(x => x.CreatedByUser).FirstOrDefaultAsync();
            if (product is null) throw new NullReferenceException();

            return product;
        }

        public async Task<Product?> Create(Product entity)
        {
            _context.Products.Add(entity);
            var saveResult = await SaveChangesAsync() > 0;
            await PopulateRedis();

            return saveResult ? entity : null;
        }

        public async Task<Product?> Update(Product entity)
        {
            _context.Products.Update(entity);
            var saveResult = await SaveChangesAsync() > 0;
            await PopulateRedis();

            return saveResult ? entity : null;
        }

        public async Task<bool> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
            if (product is null) throw new NullReferenceException();
            _context.Products.Remove(product);
            var saveResult = await SaveChangesAsync() > 0;
            await PopulateRedis();

            return saveResult;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task PopulateRedis()
        {
            var recordKey = $"Concepts_Products_{DateTime.Now.ToString("MMddyyyy")}";
            var cacheData = await _cache.GetRecordAsync<List<Product>>(recordKey);
            await _cache.RemoveAsync(recordKey);
            await _cache.SetRecordAsync(recordKey, await _context.Products.Include(x => x.CreatedByUser).ToListAsync());
        }
    }
}
