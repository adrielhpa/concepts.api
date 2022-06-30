﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concepts.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T> GetById(int id);
        Task<T?> Create(T entity);
        Task<T?> Update(T entity);
        Task<bool> Delete(int id);
        Task<int> SaveChangesAsync();
    }
}
