using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Providers
{
    public class ServiceManagementWebRequest
    {
        private const string HostedServicesUriFormatter = "https://management.core.windows.net/{0}/services/hostedservices";
        private const string HostedServicePropertiesUriFormatter = "https://management.core.windows.net/{0}/services/hostedservices/{1}?embed-detail=true";

        private readonly string subscriptionId;
        private readonly X509Certificate2 certificate;

        public ServiceManagementWebRequest(string subscriptionId, X509Certificate2 certificate)
        {
            this.subscriptionId = subscriptionId;
            this.certificate = certificate;
        }

        public XmlReader GetHostedServices()
        {
            XmlReader reader = null;

            try
            {
                reader = ExecuteWebRequest(string.Format(HostedServicesUriFormatter, subscriptionId), certificate);
            }
            catch (Exception e)
            {
                //Log Exception here but we do not want to halt execution of the application
                var msg = e.Message;
            }

            return reader;
        }

        public XmlReader GetHostedServiceDetail(string hostedServiceName)
        {
            XmlReader reader = null;

            try
            {
                reader = ExecuteWebRequest(string.Format(HostedServicePropertiesUriFormatter, subscriptionId, hostedServiceName), certificate);
            }
            catch (Exception e)
            {
                //Log Exception here but we do not want to halt execution of the application
                var msg = e.Message;
            }

            return reader;
        }

        private static XmlReader ExecuteWebRequest(string uri, X509Certificate2 certificate)
        {

            var request = CreateWebRequest(new Uri(uri), certificate);
            XmlReader reader;

            if (request == null) return null;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseStream = response.GetResponseStream();
                if (responseStream == null)
                    return null;

                using (var streamReader = new StreamReader(responseStream))
                {
                    var result = streamReader.ReadToEnd();
                    reader = XmlReader.Create(new StringReader(result));
                }
            }
            return reader;
        }

        public static HttpWebRequest CreateWebRequest(Uri uri, X509Certificate2 certificate)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = HttpMethod.Get.Method;
            request.ClientCertificates.Add(certificate);
            request.Headers.Add("x-ms-version", "2014-06-01");
            request.ContentType = "application/xml";

            return request;
        }
    }
}