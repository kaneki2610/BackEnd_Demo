using BackendSession2.Core.Models;
using System.Threading.Tasks;

namespace BackendSession2.Service
{
    public interface IHubClient
    {
        Task BroadcastMessage(string msg);
        Task SendMessageFromServer(string msg);
    }
}
