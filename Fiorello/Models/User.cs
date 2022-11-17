using Microsoft.AspNetCore.Identity;

namespace Fiorello.Models
{
    public class User:IdentityUser
    {
        public string Fullname { get; set; }
        public Basket Basket { get; set; }
    }
}
