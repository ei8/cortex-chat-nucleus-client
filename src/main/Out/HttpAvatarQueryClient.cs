using ei8.Cortex.Chat.Common;
using Microsoft.AspNetCore.Http.Extensions;
using neurUL.Common.Http;
using NLog;
using Polly.Retry;
using Polly;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Chat.Nucleus.Client.Out
{
    public class HttpAvatarQueryClient : IAvatarQueryClient
    {
        private readonly IRequestProvider requestProvider;
        private static readonly string remoteDescription = "Chat Nucleus";

        private static AsyncRetryPolicy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => HttpAvatarQueryClient.logger.Error(ex, $"Error occurred while communicating with '{HttpAvatarQueryClient.remoteDescription}'. " + ex.InnerException?.Message)
            );

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string avatarsPath = "nuclei/chat/avatars";

        public HttpAvatarQueryClient(IRequestProvider requestProvider = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
        }

        public async Task<IEnumerable<AvatarResult>> GetAvatarsByIdsAsync(string baseUrl, string bearerToken, IEnumerable<Guid> ids = null, CancellationToken token = default)
        {
            var qb = new QueryBuilder();
            if (ids != null && ids.Any())
                qb.Add("id", ids.Select(i => i.ToString()));
            var queryString = qb.Any() ? "?" + qb.ToString() : string.Empty;

            return await HttpAvatarQueryClient.exponentialRetryPolicy.ExecuteAsync(async () =>
                await this.requestProvider.GetAsync<IEnumerable<AvatarResult>>(
                    $"{baseUrl}{HttpAvatarQueryClient.avatarsPath}{queryString}", bearerToken, token
                )
            );
        }
    }
}
