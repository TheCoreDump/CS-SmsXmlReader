using Elasticsearch.Net;
using Nest;
using System;
using System.Text;

namespace SmsXmlReader
{
    public class ElasticSearchMessageCache : IMessageCache
    {
        private ElasticClient client;

        public ElasticSearchMessageCache() { }


        public ElasticClient InitializeClient()
        {
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
            var connectionSettings = new ConnectionSettings(pool)
                    .PrettyJson()
                    .DisableDirectStreaming()
                    .OnRequestCompleted(response =>
                    {
                        // log out the request
                        if (response.RequestBodyInBytes != null)
                        {
                            Console.WriteLine($"{response.HttpMethod} {response.Uri} \n" + $"{Encoding.UTF8.GetString(response.RequestBodyInBytes)}");
                        }
                        else
                        {
                            Console.WriteLine($"{response.HttpMethod} {response.Uri}");
                        }

                        Console.WriteLine();

                        // log out the response
                        if (response.ResponseBodyInBytes != null)
                        {
                            Console.WriteLine($"Status: {response.HttpStatusCode}\n" + $"{Encoding.UTF8.GetString(response.ResponseBodyInBytes)}\n" + $"{new string('-', 30)}\n");
                        }
                        else
                        {
                            Console.WriteLine($"Status: {response.HttpStatusCode}\n" + $"{new string('-', 30)}\n");
                        }
                    });

            return new ElasticClient(connectionSettings);
        }


        public void CacheMessage<TMessage>(TMessage message)
            where TMessage : MessageBase
        {
            client.Index<TMessage>(message);
        }

        public void Initialize()
        {
            client = InitializeClient();
            SetupIndex<SmsNode>();
            SetupIndex<MmsNode>();
        }


        private string GetIndexName<TMessage>()
            where TMessage : MessageBase
        {
            return typeof(TMessage).Name;
        }

        private void SetupIndex<TMessage>()
            where TMessage : MessageBase
        {
            // The name of the index is the name of the type.
            string indexName = GetIndexName<TMessage>();

            // Remove the index if it already exists
            if (client.IndexExists(indexName).Exists)
                client.DeleteIndex(indexName);

            // Setup the index
            client.CreateIndex(indexName, c => c.Mappings(m => m.Map<TMessage>(p => p.AutoMap())));
        }
    }
}
