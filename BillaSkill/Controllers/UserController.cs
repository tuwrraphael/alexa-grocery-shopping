using BillaSkill.Models;
using BillaSkill.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace BillaSkill.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ICredentialEncryption credentialEncryption;
        private readonly IUserRepository userRepository;

        public UserController(ICredentialEncryption credentialEncryption, IUserRepository userRepository)
        {
            this.credentialEncryption = credentialEncryption;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string key, string email, string password, string storeId)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(storeId))
            {
                storeId = "00-2808";
            }
            await userRepository.CreateAsync(new User()
            {
                Credentials = await credentialEncryption.Encrypt(new LieferantCredentials() { L_MAIL = email, L_PASSWORD = password }),
                Key = key,
                StoreId = storeId
            });
            return Ok();
        }
    }
}
