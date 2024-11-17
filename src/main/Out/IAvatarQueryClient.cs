using ei8.Cortex.Chat.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ei8.Cortex.Chat.Nucleus.Client.Out
{
    public interface IAvatarQueryClient
    {
        Task<IEnumerable<AvatarResult>> GetAvatarsAsync(
            string baseUrl,
            string bearerToken,
            IEnumerable<Guid> ids = null,
            CancellationToken token = default
            );
    }
}
