﻿using System.Linq;
using Launchpad.Data.Interfaces;

namespace Launchpad.Data
{
    public abstract class BaseRepository<TModel> : IRepository<TModel>
    {
        protected ILaunchpadDataContext Context;

        public BaseRepository(ILaunchpadDataContext context)
        {
            Context = context;
        }

        public abstract IQueryable<TModel> GetAll();

        public abstract TModel Get(object id);

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }
    }
}
