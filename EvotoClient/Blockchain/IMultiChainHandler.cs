using System;
using System.Threading.Tasks;

namespace EvotoClient.Blockchain
{
    public interface IMultiChainHandler
    {
        bool Connected { get; }
        event EventHandler<EventArgs> OnConnect;
        Task Connect();
        void DisconnectAndClose();
        void Close();
    }
}