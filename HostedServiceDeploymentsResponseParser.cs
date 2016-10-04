using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Interfaces;
using JetBrains.Annotations;

namespace Providers
{
    public class HostedServiceDeploymentsResponseParser
    {
        public static string AzureNamespace = "http://schemas.microsoft.com/windowsazure";

        private readonly XmlReader reader;

        public HostedServiceDeploymentsResponseParser([NotNull] XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            this.reader = reader;
        }

        public IDictionary<string, HostedServiceDeployment> GetDeployments()
        {
            if (reader?.NameTable == null)
                return new Dictionary<string, HostedServiceDeployment>();

            var namespaceManager = new XmlNamespaceManager(reader.NameTable);
            namespaceManager.AddNamespace("azure", AzureNamespace);

            var data = XElement.Load(reader);
            var deploymentElements = data.XPathSelectElements("//azure:Deployments/azure:Deployment", namespaceManager);

            var deployments = new Dictionary<string, HostedServiceDeployment>(StringComparer.OrdinalIgnoreCase);
            foreach (var deploymentElement in deploymentElements)
            {
                var deploymentIdElement = deploymentElement.XPathSelectElement("azure:PrivateID", namespaceManager);
                var deploymentSlotElement = deploymentElement.XPathSelectElement("azure:DeploymentSlot", namespaceManager);

                var deployment = new HostedServiceDeployment(deploymentIdElement.Value, deploymentSlotElement.Value);
                deployments.Add(deployment.Id, deployment);
            }

            return deployments;
        }
    }
}