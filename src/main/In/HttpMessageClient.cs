﻿using neurUL.Common.Http;
using NLog;
using Polly;
using Splat;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Chat.Nucleus.Client.In
{
    public class HttpMessageClient : IMessageClient
    {
        private readonly IRequestProvider requestProvider;

        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => HttpMessageClient.logger.Error(ex, "Error occurred while communicating with Neurul Cortex. " + ex.InnerException?.Message)
            );

        private static readonly string messagesPath = "nuclei/chat/messages";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public HttpMessageClient(IRequestProvider requestProvider = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
        }
        
        public async Task CreateMessage(string avatarUrl, string id, string content, string regionId, string bearerToken, CancellationToken token = default(CancellationToken)) =>
            await HttpMessageClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.CreateMessageInternal(avatarUrl, id, content, regionId, bearerToken, token).ConfigureAwait(false));

        private async Task CreateMessageInternal(string avatarUrl, string id, string content, string regionId, string bearerToken, CancellationToken token = default(CancellationToken))
        {
            var data = new
            {
                Id = id,
                Content = content,
                RegionId = regionId
            };

            await this.requestProvider.PostAsync(
               $"{avatarUrl}{HttpMessageClient.messagesPath}",
               data,
               bearerToken
               );
        }
    }
}
