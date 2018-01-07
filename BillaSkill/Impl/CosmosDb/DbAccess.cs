using BillaSkill.Models;
using BillaSkill.Services;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BillaSkill.Impl.CosmosDb
{
    public class DbAccess : IDbAccess
    {
        private DbConnectionOptions options;
        public const string DBName = "billa_alexa";
        public const string WarenkorbCollectionName = "WarenkorbCollection";
        public const string SucheCollectionName = "SucheCollection";
        public DbAccess(IOptions<DbConnectionOptions> optionsAccessor)
        {
            options = optionsAccessor.Value;
        }

        public DbAccess(DbConnectionOptions options)
        {
            this.options = options;
        }

        public DocumentClient GetClient()
        {
            var client = new DocumentClient(new Uri(options.DB_URI), options.DB_KEY);
            return client;
        }

        public async Task<DocumentClient> InitializeDb()
        {
            var client = GetClient();
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = DBName });
            await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DBName),
                new DocumentCollection { Id = WarenkorbCollectionName });
            await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DBName),
                new DocumentCollection { Id = SucheCollectionName });
            return client;
        }

        public Uri GetWarenkorbCollectionUri()
        {
            return UriFactory.CreateDocumentCollectionUri(DBName, WarenkorbCollectionName);
        }
    }
}
