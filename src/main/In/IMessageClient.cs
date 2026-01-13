using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Chat.Nucleus.Client.In
{
    public interface IMessageClient
    {
        Task CreateMessage(
            string baseUrl,
            string bearerToken, 
            string id, 
            string content, 
            string regionId, 
            string externalReferenceUrl,
            IEnumerable<string> recipientAvatarIds = null,
            CancellationToken token = default
            );
    }
}