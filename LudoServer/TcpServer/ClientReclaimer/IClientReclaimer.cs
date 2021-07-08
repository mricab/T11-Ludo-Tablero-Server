using System;
namespace server
{
    public interface IClientReclaimer
    {
        void OnClientReclaimerStateChanged(ReclaimerStateEvent e);
        void OnClientConnectionReclaimed(ReclaimEvent e);
    }
}
