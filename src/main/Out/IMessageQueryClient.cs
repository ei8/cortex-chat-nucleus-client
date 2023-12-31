﻿using ei8.Cortex.Chat.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Chat.Nucleus.Client.Out
{
    public interface IMessageQueryClient
    {
        Task<IEnumerable<MessageData>> GetMessagesAsync(string baseUrl, string bearerToken, DateTimeOffset? maxTimestamp = null, int? pageSize = null, CancellationToken token = default);
    }
}
