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

namespace ei8.Cortex.Chat.Nucleus.Client.Out
{
    public class HttpAvatarQueryClient : IAvatarQueryClient
    {
        private static readonly string avatarsPath = "nuclei/chat/avatars";

        private readonly IHttpClientFactory httpClientFactory;

        public HttpAvatarQueryClient(IHttpClientFactory httpClientFactory)
        {
            AssertionConcern.AssertArgumentNotNull(httpClientFactory, nameof(httpClientFactory));

            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<IMirrorImageSeries<AvatarInfo>>> GetAvatarsAsync(string baseUrl, string bearerToken, IEnumerable<Guid> ids = null, CancellationToken token = default)
        {
            var qb = new QueryBuilder();
            if (ids != null && ids.Any())
                qb.Add("id", ids.Select(i => i.ToString()));
            var queryString = qb.Any() ? "?" + qb.ToString() : string.Empty;

            var getResult = await this.httpClientFactory.CreateClient().AuthGetMirrorImageSeriesAsync<AvatarInfo>(
                $"{baseUrl}{HttpAvatarQueryClient.avatarsPath}{queryString}",
                bearerToken,
                token
            );

            if (!getResult.Response.IsSuccessStatusCode)
            {
                var failContent = await getResult.Response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new HttpRequestException($"Error encountered while sending request to '{baseUrl}': '{getResult.Response.Content.ReadAsStringAsync()}");
            }

            return getResult.Result;
        }
    }
}
