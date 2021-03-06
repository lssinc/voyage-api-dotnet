﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

namespace Voyage.Data.Repositories.ClientScope
{
    public class ClientScopeRepository : BaseRepository<Models.Entities.ClientScope>, IClientScopeRepository
    {
        public ClientScopeRepository(IVoyageDataContext context)
            : base(context)
        {
        }

        public async override Task<Models.Entities.ClientScope> AddAsync(Models.Entities.ClientScope model)
        {
            Context.ClientScopes.Add(model);
            await Context.SaveChangesAsync();
            return model;
        }

        public async override Task<int> DeleteAsync(object id)
        {
            var entity = await GetAsync(id);
            if (entity == null)
                return 0;

            Context.ClientScopes.Remove(entity);
            return await Context.SaveChangesAsync();
        }

        public async override Task<Models.Entities.ClientScope> GetAsync(object id)
        {
            if (Context.ClientScopes is DbSet<Models.Entities.ClientScope> dbSet)
            {
                return await dbSet.FindAsync(id);
            }

            return Context.ClientScopes.Find(id);
        }

        public override IQueryable<Models.Entities.ClientScope> GetAll()
        {
            return Context.ClientScopes;
        }

        public async override Task<Models.Entities.ClientScope> UpdateAsync(Models.Entities.ClientScope model)
        {
            Context.ClientScopes.AddOrUpdate(model);
            await Context.SaveChangesAsync();
            return model;
        }
    }
}
