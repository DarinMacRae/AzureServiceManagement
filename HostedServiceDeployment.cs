namespace Interfaces
{
    public class HostedServiceDeployment
    {
        public HostedServiceDeployment(string id, string slot)
        {
            Id = id;
            Slot = slot;
        }

        public string Id { get; private set; }

        public string Slot { get; private set; }
    }
}