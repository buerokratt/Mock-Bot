﻿using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using MockBot.Api.Interfaces;
using MockBot.Api.Models;

namespace MockBot.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly IDictionary<Guid, Chat> _chats;

        public ChatService()
        {
            _chats = new ConcurrentDictionary<Guid, Chat>();
        }

        public Chat CreateChat()
        {
            var chat = new Chat();
            _chats.Add(chat.Id, chat);
            return chat;
        }

        public IEnumerable<Chat> FindAll()
        {
            return _chats.Values.ToList();
        }

        public Chat? FindById(Guid chatId)
        {
            return _chats.TryGetValue(chatId, out var chat) ? chat : null;
        }

        public Message? FindById(Guid chatId, string messageId)
        {
            return _chats[chatId].Messages.First(message => message.Id == Guid.Parse(messageId));
        }

        public Message? AddMessage(Guid chatId, string content)
        {
            var message = new Message(content);
            var chat = FindById(chatId);

            if (chat == null)
            {
                return null;
            }

            chat.Messages.Add(message);
            return message;
        }

        public void AddMessageMetadata([NotNull] Message message, [NotNull] HeadersInput headersInput)
        {
            message.SentBy = headersInput.XSentBy;
            message.SendTo = headersInput.XSendTo;
        }
    }
}