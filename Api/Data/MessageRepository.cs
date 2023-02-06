using System;
using Api.DTOs;
using Api.Entities;
using Api.Helpers;
using Api.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;

        private readonly IMapper mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;

        }

        public void AddGroup(Group group)
        {
            this.context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            this.context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            this.context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await this.context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await this.context.Groups.Include(x => x.Connections)
            .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await this.context.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await this.context.Groups.Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = this.context.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(this.mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var query =  this.context.Messages
            .Where(
                m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false && m.SenderUsername == recipientUserName
                || m.RecipientUsername == recipientUserName && m.SenderDeleted == false && m.SenderUsername == currentUserName
            ).OrderBy(m => m.MessageSent)
            .AsQueryable();

            var unreadMessages = query.Where(m => m.DateRead == null && m.RecipientUsername == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            return await query.ProjectTo<MessageDto>(this.mapper.ConfigurationProvider).ToListAsync();
        }

        public void RemoveConnection(Connection connection)
        {
            this.context.Connections.Remove(connection);
        }

    }
}