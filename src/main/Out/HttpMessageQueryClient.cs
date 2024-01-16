using ei8.Cortex.Chat.Common;
using neurUL.Common.Http;
using NLog;
using Polly;
using Polly.Retry;
using Splat;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<MessageData>> GetMessagesAsync(string baseUrl, string bearerToken, DateTimeOffset? maxTimestamp = null, int? pageSize = null, CancellationToken token = default)
        {
            var maxTimestampParam = maxTimestamp.HasValue ?
                "maxTimestamp=" + HttpUtility.UrlEncode(maxTimestamp.Value.ToString("o")) :
                string.Empty;
            var pageSizeParam = pageSize.HasValue ?
                "pageSize=" + pageSize.Value.ToString() :
                string.Empty;
            var queryString = maxTimestamp.HasValue || pageSize.HasValue ? "?" : string.Empty;
            queryString += maxTimestamp.HasValue ? maxTimestampParam : string.Empty;
            queryString += maxTimestamp.HasValue && pageSize.HasValue ? "&" : string.Empty;
            queryString += pageSize.HasValue ? pageSizeParam : string.Empty;

            return await HttpMessageQueryClient.exponentialRetryPolicy.ExecuteAsync(async () =>
                await this.requestProvider.GetAsync<IEnumerable<MessageData>>(
                    $"{baseUrl}{HttpMessageQueryClient.messagesPath}{queryString}", bearerToken, token
                )
            );
        }
    }
}
