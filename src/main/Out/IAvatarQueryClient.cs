using ei8.Cortex.Chat.Common;
using ei8.Cortex.Coding.Mirrors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Chat.Nucleus.Client.Out
{
    public interface IAvatarQueryClient
    {
        Task<IEnumerable<IMirrorImageSeries<AvatarInfo>>> GetAvatarsAsync(
            string baseUrl,
            string bearerToken,
            IEnumerable<Guid> ids = null,
            CancellationToken token = default
        );
    }
}
