using System;
using System.Threading.Tasks;

namespace AgentService.Interfaces
{
    public interface IAgentService
    {
        /// <summary>
        /// Executes the implementation of the agent service
        /// </summary>
        /// <param name="agentId">The id of the agent processing orders</param>
        /// <param name="magicNumber">A random number between 1 and 10</param>
        Task RunAsync(Guid agentId, int magicNumber);
    }
}
