using ei8.Cortex.Chat.Common;
using ei8.Cortex.Coding.Client;
using ei8.Cortex.Coding.Mirrors;
using Microsoft.AspNetCore.Http.Extensions;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ei8.Cortex.Chat.Nucleus.Client.Out
{
    public class HttpMessageQueryClient : IMessageQueryClient
    {
        private static readonly string messagesPath = "nuclei/chat/messages";

        private readonly IHttpClientFactory httpClientFactory;

        public HttpMessageQueryClient(IHttpClientFactory httpClientFactory)
        {
            AssertionConcern.AssertArgumentNotNull(httpClientFactory, nameof(httpClientFactory));

            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<IMirrorImageSeries<MessageResult>>> GetMessagesAsync(
            string baseUrl,
            string bearerToken,
            DateTimeOffset? maxTimestamp,
            int? pageSize,
            bool includeRemote,
            IEnumerable<Guid> avatarIds = null,
            CancellationToken token = default
            )
        {
            var qb = new QueryBuilder();
            if (maxTimestamp.HasValue)
                qb.Add(nameof(maxTimestamp), HttpUtility.UrlEncode(maxTimestamp.Value.ToString("o")));
            if (pageSize.HasValue)
                qb.Add(nameof(pageSize), pageSize.Value.ToString());
            if (avatarIds != null && avatarIds.Any())
                qb.Add("avatarId", avatarIds.Select(eri => eri.ToString()));
            qb.Add(nameof(includeRemote), includeRemote.ToString());

            var queryString = qb.Any() ? qb.ToString() : string.Empty;

            return await this.httpClientFactory.CreateClient().AuthGetMirrorImageSeriesAsync<MessageResult>(
                $"{baseUrl}{HttpMessageQueryClient.messagesPath}{queryString}",
                bearerToken,
                token
            );
        }
    }
}
