using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace BillaSkill.Services
{
    public interface IDbAccess
    {
        DocumentClient GetClient();
        Task<DocumentClient> InitializeDb();
        Uri GetWarenkorbCollectionUri();
    }
}