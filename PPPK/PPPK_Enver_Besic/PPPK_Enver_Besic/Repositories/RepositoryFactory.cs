using System;
using System.Collections.Generic;
using PPPK_Enver_Besic.Models;

namespace PPPK_Enver_Besic.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public RepositoryFactory(AppDbContext context)
        {
            _context = context;
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            if (!_repositories.ContainsKey(typeof(T)))
            {
                var repositoryInstance = new Repository<T>(_context);
                _repositories.Add(typeof(T), repositoryInstance);
            }
            return (IRepository<T>)_repositories[typeof(T)];
        }
    }
}
