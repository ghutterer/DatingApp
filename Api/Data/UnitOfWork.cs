using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Interfaces;
using AutoMapper;

namespace Api.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper mapper;
        private readonly DataContext context;

        public UnitOfWork(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;

        }
        public IUserRepository UserRepository => new UserRepository(this.context, this.mapper);

        public IMessageRepository MessageRepository => new MessageRepository(this.context, this.mapper);

        public ILikesRepository LikesRepository => new LikesRepository(this.context);

        public async Task<bool> Complete()
        {
            return await this.context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return this.context.ChangeTracker.HasChanges();
        }
    }
}