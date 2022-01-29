﻿// MIT License
// Copyright Eraware

using Eraware.Modules.MyModule.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Eraware.Modules.MyModule.Data.Repositories
{
    /// <summary>
    /// Provides common generic data access methods for entities.
    /// </summary>
    /// <typeparam name="T">The type of the entities.</typeparam>
    public class Repository<T> : IRepository<T>
        where T : BaseEntity
    {
        private readonly ModuleDbContext context;
        private DbSet<T> entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The module database context.</param>
        public Repository(ModuleDbContext context)
        {
            this.context = context;
            this.entities = context.Set<T>();
        }

        /// <inheritdoc/>
        public IEnumerable<T> GetAll()
        {
            return this.entities.AsEnumerable();
        }

        /// <inheritdoc/>
        public IQueryable<T> Get()
        {
            return this.entities;
        }

        /// <inheritdoc/>
        public T GetById(int id)
        {
            return this.entities.SingleOrDefault(e => e.Id == id);
        }

        /// <inheritdoc/>
        public void Create(T entity, int userId = -1)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entity.CreatedByUserId = userId;
            entity.UpdatedByUserId = userId;
            this.entities.Add(entity);
            this.context.SaveChanges();
        }

        /// <inheritdoc/>
        public void Update(T entity, int userId = -1)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = userId;
            this.context.SaveChanges();
        }

        /// <inheritdoc/>
        public void Delete(int id)
        {
            T entity = this.entities.SingleOrDefault(e => e.Id == id);
            if (entity is null)
            {
                return;
            }

            this.entities.Remove(entity);
            this.context.SaveChanges();
        }
    }
}