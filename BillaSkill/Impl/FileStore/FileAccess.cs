using BillaSkill.Services;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace BillaSkill.Impl.FileStore
{
    public class FileStoreAccess : IFileAccess
    {
        private readonly IFileProvider provider;
        private readonly string rootPath;

        public FileStoreAccess(IFileProvider provider, string rootPath)
        {
            this.provider = provider;
            this.rootPath = rootPath;
        }

        public async Task DeleteAsync(string id, string collection)
        {
            var fileInfo = provider.GetFileInfo($"{id}-{collection}.json");
            if (fileInfo.Exists)
            {
                FileInfo info = new FileInfo(fileInfo.PhysicalPath);
                info.Delete();
            }
        }

        public async Task<T> GetAsync<T>(string id, string collection) where T : class
        {
            var fileInfo = provider.GetFileInfo($"{id}-{collection}.json");
            if (!fileInfo.Exists)
            {
                return null;
            }
            else
            {
                using (var stream = fileInfo.CreateReadStream())
                using (var reader = new StreamReader(stream))
                {
                    var str = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<T>(str);
                }

            }
        }

        public async Task InitializeAsync()
        {
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
        }

        public async Task<string> SaveAsync<T>(string id, string collection, T t)
        {
            var fileInfo = provider.GetFileInfo($"{id}-{collection}.json");
            using (var stream = new FileStream(fileInfo.PhysicalPath, FileMode.Create, FileAccess.Write))
                using(var writer = new StreamWriter(stream))
            {
                var str = JsonConvert.SerializeObject(t);
                await writer.WriteAsync(str);                
            }
            return fileInfo.PhysicalPath;
        }
    }
}
