using Concepts.Domain;
using Concepts.Repository.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Concepts.Repository.UsersRepo
{
    public class UserRepository : IBaseRepository<User>
    {
        private readonly ConceptContext _context;
        private IDistributedCache _cache;
        public UserRepository(ConceptContext context, IDistributedCache cache)
        {
            _cache = cache;
            _context = context;
        }
        public async Task<List<User>> GetAll()
        {
            var recordKey = $"Concepts_Users_{DateTime.Now.ToString("MMddyyyy")}";
            var cacheData = await _cache.GetRecordAsync<List<User>>(recordKey);
            if (cacheData is not null) return cacheData;

            var result = await _context.Users.ToListAsync();
            await _cache.SetRecordAsync(recordKey, result);
            return result;
        }

        public async Task<User> GetById(int id)
        {
            var recordKey = $"Concepts_Users_{DateTime.Now.ToString("MMddyyyy")}";
            var cacheData = await _cache.GetRecordAsync<List<User>>(recordKey);

            if (cacheData is not null)
            {
                var cacheUser = cacheData.Find(u => u.Id == id);
                if (cacheUser is not null) return cacheUser;
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user is null) throw new NullReferenceException();
            return user;

        }
        public async Task<User?> Create(User entity)
        {
            _context.Users.Add(entity);
            var saveResult = await SaveChangesAsync() > 0;
            await PopulateRedis();

            return saveResult ? entity : null;
        }

        public async Task<User?> Update(User entity)
        {
            _context.Users.Update(entity);
            var saveResult = await SaveChangesAsync() > 0;
            await PopulateRedis();

            return saveResult ? entity : null;
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user is null) throw new NullReferenceException();
            _context.Users.Remove(user);
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
            var recordKey = $"Concepts_Users_{DateTime.Now.ToString("MMddyyyy")}";
            var cacheData = await _cache.GetRecordAsync<List<User>>(recordKey);
            await _cache.RemoveAsync(recordKey);
            await _cache.SetRecordAsync(recordKey, await _context.Users.ToListAsync());
        }
    }
}
