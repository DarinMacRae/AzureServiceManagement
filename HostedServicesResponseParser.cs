using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Interfaces;

namespace Providers
{
    public class HostedServicesResponseParser
    {
        public static string AzureNamespace = "http://schemas.microsoft.com/windowsazure";

        private readonly XmlReader reader;

        public HostedServicesResponseParser(XmlReader reader)
        {
            this.reader = reader;
        }

        public IEnumerable<HostedService> GetHostedServices()
        {
            if (reader?.NameTable == null)
                return Enumerable.Empty<HostedService>();

            var namespaceManager = new XmlNamespaceManager(reader.NameTable);
            namespaceManager.AddNamespace("azure", AzureNamespace);

            var data = XElement.Load(reader);
            var nameElements = data.XPathSelectElements("//azure:ServiceName", namespaceManager)
                .ToList();

            var hostedServices = new List<HostedService>();
            foreach (var nameElement in nameElements)
            {
                var resourceLocationElement = nameElement.XPathSelectElement("../azure:HostedServiceProperties/azure:ExtendedProperties/azure:ExtendedProperty[azure:Name = \"ResourceLocation\"]/azure:Value", namespaceManager);
                var resourceGroupElement = nameElement.XPathSelectElement("../azure:HostedServiceProperties/azure:ExtendedProperties/azure:ExtendedProperty[azure:Name = \"ResourceGroup\"]/azure:Value", namespaceManager);
                var hostedService = new HostedService(nameElement.Value, resourceLocationElement.Value, resourceGroupElement.Value);
                hostedServices.Add(hostedService);
            }

            return hostedServices;
        }

    }
}