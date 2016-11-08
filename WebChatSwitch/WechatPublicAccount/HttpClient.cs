using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WechatPublicAccount
{
    public class HttpClient
    {
        #region Public Methods

        #region Delete

        /// <summary>
        /// Makes a DELETE request to the specified URL.
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Delete(string apiUrl)
        {
            return Execute("Delete", apiUrl);
        }

        /// <summary>
        /// Makes a DELETE request to the specified URL.
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="oauthToken">OAuth Token for the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Delete(string apiUrl, string oauthToken)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "OAuth " + oauthToken);

            return Delete(apiUrl, headers);
        }

        /// <summary>
        /// Makes a DELETE request to the specified URL.
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="headers">Dictionary with all the HTTP headers needed for the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Delete(string apiUrl, Dictionary<string, string> headers)
        {
            if (headers == null || headers.Count == 0)
                throw new ArgumentException("You must specify at least one HTTP header.");

            return Execute("Delete", apiUrl, headers);
        }

        #endregion

        #region Get

        /// <summary>
        /// Makes a GET request to the specified URL.
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Get(string apiUrl)
        {
            return Execute("Get", apiUrl);
        }

        /// <summary>
        /// Makes a GET request to the specified URL with a token
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="oauthToken">OAuth token for the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Get(string apiUrl, string oauthToken)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "OAuth " + oauthToken);

            return Get(apiUrl, headers);
        }

        /// <summary>
        /// Makes a GET request to the specified URL specifying headers
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="headers">Dictionary with all the HTTP headers needed for the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Get(string apiUrl, Dictionary<string, string> headers)
        {
            if (headers == null || headers.Count == 0)
                throw new ArgumentException("You must specify at least one HTTP header.");

            return Execute("Get", apiUrl, headers);
        }

        #endregion

        #region Post		

        /// <summary>
        /// Makes a POST request to the specified URL with a string content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Post(string apiUrl, string contentBody)
        {
            byte[] contentBodyBytes = Encoding.UTF8.GetBytes(contentBody);

            return Execute("Post", apiUrl, null, contentBodyBytes);
        }

        /// <summary>
        /// Makes a POST request to the specified URL with an XML content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Post(string apiUrl, XmlDocument contentBody)
        {
            byte[] contentBodyBytes = Encoding.UTF8.GetBytes(contentBody.OuterXml);

            return Execute("Post", apiUrl, null, contentBodyBytes);
        }

        /// <summary>
        /// Makes a POST request to the specified URL with a byte array content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Post(string apiUrl, byte[] contentBody)
        {
            return Execute("Post", apiUrl, null, contentBody);
        }

        /// <summary>
        /// Makes a POST request to the specified URL with a token and string content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="oauthToken">OAuth Token for the Request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Post(string apiUrl, string oauthToken, string contentBody)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "OAuth " + oauthToken);

            byte[] contentBodyBytes = Encoding.UTF8.GetBytes(contentBody);

            return Post(apiUrl, headers, contentBodyBytes);
        }

        /// <summary>
        /// Makes a POST request to the specified URL.
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="headers">Dictionary with all the HTTP headers needed for the request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Post(string apiUrl, Dictionary<string, string> headers, string contentBody)
        {
            if (headers == null || headers.Count == 0)
                throw new ArgumentException("You must specify at least one HTTP header.");

            byte[] contentBodyBytes = Encoding.UTF8.GetBytes(contentBody);

            return Post(apiUrl, headers, contentBodyBytes);
        }

        /// <summary>
        /// Makes a POST request to the specified URL with a token and XML content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="oauthToken">OAuth Token for the Request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Post(string apiUrl, string oauthToken, XmlDocument contentBody)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "OAuth " + oauthToken);

            byte[] contentBodyBytes = Encoding.UTF8.GetBytes(contentBody.OuterXml);

            return Post(apiUrl, headers, contentBodyBytes);
        }

        /// <summary>
        /// Makes a POST request to the specified URL with headers and a byte array content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="headers">Dictionary with all the HTTP headers needed for the request</param>
        /// <param name="contentBody">Body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Post(string apiUrl, Dictionary<string, string> headers, byte[] contentBody)
        {
            return Execute("Post", apiUrl, headers, contentBody);
        }

        #endregion

        #region Put

        /// <summary>
        /// Makes a PUT request to the specified URL with a string content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Put(string apiUrl, string contentBody)
        {
            byte[] contentBodyBytes = Encoding.UTF8.GetBytes(contentBody);

            return Execute("Put", apiUrl, null, contentBodyBytes);
        }

        /// <summary>
        /// Makes a PUT request to the specified URL with an XML content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Put(string apiUrl, XmlDocument contentBody)
        {
            byte[] contentBodyBytes = Encoding.UTF8.GetBytes(contentBody.OuterXml);

            return Execute("Put", apiUrl, null, contentBodyBytes);
        }

        /// <summary>
        /// Makes a PUT request to the specified URL with a byte array content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Put(string apiUrl, byte[] contentBody)
        {
            return Execute("Put", apiUrl, null, contentBody);
        }

        /// <summary>
        /// Makes a PUT request to the specified URL with an OAuth token and string content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="oauthToken">OAuth token for the request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Put(string apiUrl, string oauthToken, string contentBody)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "OAuth " + oauthToken);

            byte[] contentBodyBytes = Encoding.UTF8.GetBytes(contentBody);

            return Put(apiUrl, headers, contentBodyBytes);
        }

        /// <summary>
        /// Makes a PUT request to the specified URL with an OAuth token and XML content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="oauthToken">OAuth token for the request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Put(string apiUrl, string oauthToken, XmlDocument contentBody)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "OAuth " + oauthToken);

            byte[] contentBodyBytes = Encoding.UTF8.GetBytes(contentBody.OuterXml);

            return Put(apiUrl, headers, contentBodyBytes);
        }

        /// <summary>
        /// Makes a PUT request to the specified URL with headers and string content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="headers">Dictionary with all the HTTP headers needed for the request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Put(string apiUrl, Dictionary<string, string> headers, string contentBody)
        {
            if (headers == null || headers.Count == 0)
                throw new ArgumentException("You must specify at least one HTTP header.");

            byte[] contentBodyBytes = Encoding.UTF8.GetBytes(contentBody);

            return Put(apiUrl, headers, contentBodyBytes);
        }

        /// <summary>
        /// Makes a PUT request to the specified URL with headers and a byte array content body
        /// </summary>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="headers">Dictionary with all the HTTP headers needed for the request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static public HttpWebResponse Put(string apiUrl, Dictionary<string, string> headers, byte[] contentBody)
        {
            if (headers == null || headers.Count == 0)
                throw new ArgumentException("You must specify at least one HTTP header.");

            return Execute("Put", apiUrl, headers, contentBody);
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Executes the request and returns the response.
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="apiUrl">URL to send the request</param>
        /// <returns>Response from service</returns>
        static private HttpWebResponse Execute(string method, string apiUrl)
        {
            return Execute(method, apiUrl, null);
        }

        /// <summary>
        /// Executes the request and returns the response.
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="headers">Dictionary with all the HTTP headers needed for the request</param>
        /// <returns>Response from service</returns>
        static private HttpWebResponse Execute(string method, string apiUrl, Dictionary<string, string> headers)
        {
            byte[] contentBody = new byte[0];

            return Execute(method, apiUrl, headers, contentBody);
        }

        /// <summary>
        /// Executes the request and returns the response.
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="apiUrl">URL to send the request</param>
        /// <param name="headers">Dictionary with all the HTTP headers needed for the request</param>
        /// <param name="contentBody">Content body of the request</param>
        /// <returns>Response from service</returns>
        static private HttpWebResponse Execute(string method, string apiUrl, Dictionary<string, string> headers, byte[] contentBody)
        {
            //Exception handling
            if (String.IsNullOrEmpty(method))
                throw new ArgumentNullException("method");

            if (String.IsNullOrEmpty(apiUrl))
                throw new ArgumentNullException("apiUrl");

            if (method.Equals("Post", StringComparison.CurrentCultureIgnoreCase) ||
                method.Equals("Put", StringComparison.CurrentCultureIgnoreCase))
            {
                if (contentBody.Length == 0)
                    throw new ArgumentException("You must specify a content body when using POST or PUT.");
            }

            HttpWebResponse response = null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Timeout = System.Threading.Timeout.Infinite;
            request.UseDefaultCredentials = true;
            request.Method = method;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            //Iterate through headers dictionary and add headers to the request
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    if (String.Equals(header.Key, "ContentType"))
                    {
                        request.ContentType = header.Value;
                        request.Accept = header.Value;
                    }
                    else if (String.Equals(header.Key, "Accept"))
                    {
                        request.ContentType = header.Value;
                        request.Accept = header.Value;
                    }
                    else
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            //If request is PUT or POST, set content body to the request
            if (method.Equals("Post", StringComparison.CurrentCultureIgnoreCase) ||
                method.Equals("Put", StringComparison.CurrentCultureIgnoreCase))
            {
                request.ContentLength = contentBody.Length;
                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(contentBody, 0, contentBody.Length);
                    newStream.Flush();
                }
            }

            return response = (HttpWebResponse)request.GetResponse();
        }

        #endregion

        #region Utility Methods		

        /// <summary>
        /// Gets an XML string from the response stream.
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <returns>XML string</returns>
        public static string GetStringFromStream(Stream stream)
        {
            //Adjust settings so XML will be formatted (indented) properly

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.ConformanceLevel = ConformanceLevel.Fragment;

            //Write the XML response to a string builder
            StringBuilder strBuilder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(strBuilder, writerSettings))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                doc.WriteTo(writer);
            }

            return strBuilder.ToString();
        }

        public static string GetStringFromStream(Stream stream, long Length)
        {
            string result = string.Empty;
            using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("utf-8")))
            {
                result = reader.ReadToEnd();
            }
            stream.Dispose();
            return result;

        }

        public static string GetStringFromStream2(Stream stream, long Length)
        {
            string result = string.Empty;
            using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            stream.Dispose();
            return result;

        }

        /// <summary>
        /// Gets the specified tag element and returns inner text.
        /// </summary>
        /// <param name="elementName">Tag element</param>
        /// <param name="xml">Element XML</param>
        /// <returns>Element value</returns>
        public static string GetElementValue(string elementName, string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode element = doc.SelectSingleNode("//*[local-name()='" + elementName + "']");

            return element.InnerText;
        }

        #endregion

        #region Extend Methods

        public static T Get<T>(string strURL, Dictionary<string, string> header)
        {
            HttpWebResponse response = HttpClient.Get(strURL, header);
            string responseString = HttpClient.GetStringFromStream(response.GetResponseStream(), response.ContentLength);
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public static T Get<T>(string strURL)
        {
            HttpWebResponse response = HttpClient.Get(strURL);
            string responseString = HttpClient.GetStringFromStream(response.GetResponseStream(), response.ContentLength);
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public static T Post<T, DATA>(string strURL, DATA strData)
        {
            HttpWebResponse response = HttpClient.Post(strURL, JsonConvert.SerializeObject(strData));
            string responseString = HttpClient.GetStringFromStream(response.GetResponseStream(), response.ContentLength);
            return JsonConvert.DeserializeObject<T>(responseString);
        }
        public static T Post<T, DATA>(string strURL, DATA strData, Dictionary<string, string> header)
        {
            HttpWebResponse response = HttpClient.Post(strURL, header, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(strData)));
            string responseString = HttpClient.GetStringFromStream(response.GetResponseStream(), response.ContentLength);
            return JsonConvert.DeserializeObject<T>(responseString);
        }
        public static T Post<T>(string strURL, string strData)
        {
            HttpWebResponse response = HttpClient.Post(strURL, strData);
            string responseString = HttpClient.GetStringFromStream(response.GetResponseStream(), response.ContentLength);
            return JsonConvert.DeserializeObject<T>(responseString);
        }


        #endregion
    }
}
