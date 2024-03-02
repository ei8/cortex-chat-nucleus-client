using ei8.Cortex.Chat.Common;
using Microsoft.AspNetCore.Http.Extensions;
using neurUL.Common.Http;
using NLog;
using Polly;
using Polly.Retry;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ei8.Cortex.Chat.Nucleus.Client.Out
{
    public class HttpMessageQueryClient : IMessageQueryClient
    {
        private readonly IRequestProvider requestProvider;

        private static AsyncRetryPolicy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => HttpMessageQueryClient.logger.Error(ex, "Error occurred while communicating with Neurul Cortex. " + ex.InnerException?.Message)
            );

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string messagesPath = "nuclei/chat/messages";

        public HttpMessageQueryClient(IRequestProvider requestProvider = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
        }

        public async Task<IEnumerable<MessageResult>> GetMessagesAsync(
            string baseUrl,
            string bearerToken,
            DateTimeOffset? maxTimestamp,
            int? pageSize,
            IEnumerable<Guid> avatarIds = null,
            CancellationToken token = default
            )
        {
            var qb = new QueryBuilder();
            if (maxTimestamp.HasValue)
                qb.Add("maxTimestamp", HttpUtility.UrlEncode(maxTimestamp.Value.ToString("o")));
            if (pageSize.HasValue)
                qb.Add("pageSize", pageSize.Value.ToString());
            if (avatarIds != null && avatarIds.Any())
                qb.Add("avatarId", avatarIds.Select(eri => eri.ToString()));
            var queryString = qb.Any() ? "?" + qb.ToString() : string.Empty;
            
            return await HttpMessageQueryClient.exponentialRetryPolicy.ExecuteAsync(async () =>
                await this.requestProvider.GetAsync<IEnumerable<MessageResult>>(
                    $"{baseUrl}{HttpMessageQueryClient.messagesPath}{queryString}", bearerToken, token
                )
            );
        }
    }
}
