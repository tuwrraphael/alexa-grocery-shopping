namespace BillaSkill.Models
{
    public class User
    {
        public string Key { get; set; }
        public string StoreId { get; set; }
        public EncryptedCredentials Credentials { get; set; }
    }
}
