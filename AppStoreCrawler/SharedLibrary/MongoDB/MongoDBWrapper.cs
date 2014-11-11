using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.MongoDB
{
    public class MongoDBWrapper
    {
        #region ** Private Attributes **

        private string           _connString;
        private string           _collectionName;
        private MongoServer      _server;
        private MongoDatabase    _database;

        private string           _entity;

        #endregion

        /// <summary>
        /// Executes the configuration needed in order to start using MongoDB
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">User Password</param>
        /// <param name="authSrc">Database used as Authentication Provider. Default is 'admin'</param>
        /// <param name="serverAddress">IP Address of the DB Server. Format = ip:port</param>
        /// <param name="timeout">Connection Timeout</param>
        /// <param name="databaseName">Database Name</param>
        /// <param name="collectionName">Collection Name</param>
        /// <param name="entity">Not Used. Use Empty</param>
        public void ConfigureDatabase (string username, string password, string authSrc, string serverAddress, int timeout, string databaseName, string collectionName, string entity = "")
        {
            // Reading App Config for config data            
            _connString     = MongoDbContext.BuildConnectionString (username, password, authSrc, true, false, serverAddress, timeout, timeout);                        
            _server         = new MongoClient (_connString).GetServer ();
            _database       = _server.GetDatabase (databaseName);
            _collectionName = collectionName;
            _entity         = entity;            
        }

        /// <summary>
        /// Inserts an element to the MongoDB.
        /// The type T must match the type of the target collection
        /// </summary>
        /// <typeparam name="T">Type of the object to be inserted</typeparam>
        /// <param name="record">Record that will be inserted in the database</param>
        public bool Insert<T> (T record, string collection = "")
        {
            collection = String.IsNullOrEmpty (collection) ? _collectionName : collection;

            return _database.GetCollection<T> (collection).SafeInsert (record);
        }

        public bool IsAppOnDatabase<T> (string url)
        {
            var mongoQuery = Query.EQ ("url", url);

            return _database.GetCollection<T> (_collectionName).Count (mongoQuery) > 0;
        }
    }
}
