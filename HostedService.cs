namespace Interfaces
{
    public class HostedService
    {
        public HostedService(string name, string location, string @group)
        {
            Name = name;
            Location = location;
            Group = @group;
        }

        public string Name { get; private set; }

        public string Location { get; private set; }

        public string Group { get; private set; }
    }
}