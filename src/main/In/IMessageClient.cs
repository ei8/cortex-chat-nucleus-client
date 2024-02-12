using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Chat.Nucleus.Client.In
{
    public interface IMessageClient
    {
        Task CreateMessage(
            string avatarUrl, 
            string id, 
            string content, 
            string regionId, 
            string externalReferenceUrl, 
            IEnumerable<string> destinationRegionIds, 
            string bearerToken, 
            CancellationToken token = default
            );
    }
}