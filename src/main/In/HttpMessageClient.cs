using ei8.Cortex.Coding.Client;
using neurUL.Common.Domain.Model;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Chat.Nucleus.Client.In
{
    public class HttpMessageClient : IMessageClient
    {
        private static readonly string messagesPath = "nuclei/chat/messages";

        private readonly IHttpClientFactory httpClientFactory;

        public HttpMessageClient(IHttpClientFactory httpClientFactory)
        {
            AssertionConcern.AssertArgumentNotNull(httpClientFactory, nameof(httpClientFactory));

            this.httpClientFactory = httpClientFactory;
        }

        public async Task CreateMessage(
            string baseUrl, 
            string bearerToken, 
            string id, 
            string content, 
            string regionId, 
            string externalReferenceUrl, 
            IEnumerable<string> recipientAvatarIds = null, 
            CancellationToken token = default
        ) 
        {
            var data = new
            {
                Id = id,
                Content = content,
                RegionId = regionId,
                ExternalReferenceUrl = externalReferenceUrl,
                RecipientAvatarIds = recipientAvatarIds
            };

            var client = this.httpClientFactory.CreateClient().AuthPostAsync(
               $"{baseUrl}{HttpMessageClient.messagesPath}",
               JsonSerializer.Serialize(data),
               bearerToken,
               token
            );
        }
    }
}
