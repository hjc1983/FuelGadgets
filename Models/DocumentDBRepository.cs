using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace FGapp.Models
{
    public static class DocumentDBRepository<T>
    {
        //Use the Database if it exists, if not create a new Database
        private static Database ReadOrCreateDatabase()
        {
            var db = Client.CreateDatabaseQuery()
                            .Where(d => d.Id == DatabaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (db == null)
            {
                db = Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }

            return db;
        }

        //Use the DocumentCollection if it exists, if not create a new Collection
        private static DocumentCollection ReadOrCreateCollection(string databaseLink)
        {
            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == CollectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (col == null)
            {
                var collectionSpec = new DocumentCollection { Id = CollectionId };
                var requestOptions = new RequestOptions { OfferType = "S1" };

                col = Client.CreateDocumentCollectionAsync(databaseLink, collectionSpec, requestOptions).Result;
            }

            return col;
        }

        //Expose the "database" value from configuration as a property for internal use
        private static string databaseId;
        private static String DatabaseId
        {
            get
            {
                if (string.IsNullOrEmpty(databaseId))
                {
                    databaseId = ConfigurationManager.AppSettings["database"];
                }

                return databaseId;
            }
        }

        //Expose the "collection" value from configuration as a property for internal use
        private static string collectionId;
        private static String CollectionId
        {
            get
            {
                if (string.IsNullOrEmpty(collectionId))
                {
                    collectionId = ConfigurationManager.AppSettings["collection"];
                }

                return collectionId;
            }
        }

        //Use the ReadOrCreateDatabase function to get a reference to the database.
        private static Database database;
        private static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = ReadOrCreateDatabase();
                }

                return database;
            }
        }

        //Use the ReadOrCreateCollection function to get a reference to the collection.
        private static DocumentCollection collection;
        private static DocumentCollection Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = ReadOrCreateCollection(Database.SelfLink);
                }

                return collection;
            }
        }

        //This property establishes a new connection to DocumentDB the first time it is used, 
        //and then reuses this instance for the duration of the application avoiding the
        //overhead of instantiating a new instance of DocumentClient with each request
        private static DocumentClient client;
        private static DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    string endpoint = ConfigurationManager.AppSettings["endpoint"];
                    string authKey = ConfigurationManager.AppSettings["authKey"];
                    Uri endpointUri = new Uri(endpoint);
                    client = new DocumentClient(endpointUri, authKey);
                }

                return client;
            }
        }

        public static IEnumerable<T> GetItems(Expression<Func<T, bool>> predicate)
        {
            return Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                .Where(predicate)
                .AsEnumerable();
        }
//public static IEnumerable<T> GetItems(Expression<Func<T, bool>> predicate, int from=0, int to=10)
//        {
//            return Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
//                .Where(predicate)
//                .Skip(to)xxxx
//                .AsEnumerable();
//        }
        public static async Task<Document> CreateItemAsync(T item)
        {
            return await Client.CreateDocumentAsync(Collection.SelfLink, item);
        }

        public static T GetItem(Expression<Func<T, bool>> predicate)
        {
            return Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                        .Where(predicate)
                        .AsEnumerable()
                        .FirstOrDefault();
        }

        public static async Task<Document> UpdateItemAsync(string id, T item)
        {
            Document doc = GetDocument(id);
            return await Client.ReplaceDocumentAsync(doc.SelfLink, item);
        }

        private static Document GetDocument(string id)
        {
            return Client.CreateDocumentQuery(Collection.DocumentsLink)
                .Where(d => d.Id == id)
                .AsEnumerable()
                .FirstOrDefault();
        }

        public static async Task<Document> DeleteDocument(string id)
        {
            var doc = GetDocument(id);
            return await Client.DeleteDocumentAsync(doc.SelfLink);
        }


        public static async Task<string> ExecuteStoredProcedureUpdatePriceAsync(string siteId, string fueltype, double price, string userId)
        {
            //https://social.msdn.microsoft.com/Forums/azure/en-us/d036afe2-78ec-45ee-8b0d-297f0f5320fe/azure-documentdb-bulk-insert-using-stored-procedure?prof=required
            //https://github.com/hjgraca/documentdbsamples/blob/master/DocumentDB.Samples.ServerSideScripts/Program.cs
            string storedProdName = "UpdatePriceTest";
            var sproc = Client.CreateStoredProcedureQuery(Collection.SelfLink).Where(s => s.Id == storedProdName).AsEnumerable().FirstOrDefault();
            var response = await client.ExecuteStoredProcedureAsync<string>(sproc.SelfLink, siteId, fueltype, price, userId);
            Console.WriteLine("Result from script: {0}\r\n", response.Response);
            return  response.Response;
           // return await Client.ExecuteStoredProcedureAsync<T>(Collection.SelfLink);
        }

        public static async Task<string> ExecuteStoredProcedureSelectLastPriceAsync(string state = "")
        {
            //https://social.msdn.microsoft.com/Forums/azure/en-us/d036afe2-78ec-45ee-8b0d-297f0f5320fe/azure-documentdb-bulk-insert-using-stored-procedure?prof=required
            //https://github.com/hjgraca/documentdbsamples/blob/master/DocumentDB.Samples.ServerSideScripts/Program.cs
            string storedProdName = "LatestUpdatePrice";
            var sproc = Client.CreateStoredProcedureQuery(Collection.SelfLink).Where(s => s.Id == storedProdName).AsEnumerable().FirstOrDefault();
            var response = await client.ExecuteStoredProcedureAsync<string>(sproc.SelfLink, state);
            //string res = await Task.FromResult<string>(response.Response);
            //Console.WriteLine("Result from script: {0}\r\n", res);
            return response.Response;
            // return await Client.ExecuteStoredProcedureAsync<T>(Collection.SelfLink);
        }

    }
}