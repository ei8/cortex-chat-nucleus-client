using ei8.Cortex.Chat.Common;
using ei8.Cortex.Coding.Mirrors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Chat.Nucleus.Client.Out
{
    public interface IMessageQueryClient
    {
        Task<IEnumerable<IMirrorImageSeries<MessageResult>>> GetMessagesAsync(
            string baseUrl, 
            string bearerToken, 
            DateTimeOffset? maxTimestamp, 
            int? pageSize, 
            bool includeRemote,
            IEnumerable<Guid> avatarIds = null,
            CancellationToken token = default
        );
    }
}
