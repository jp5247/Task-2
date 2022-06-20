using System.Collections.Generic;

namespace JwtWebAPI.Models
{
    public static class RegisteredUsers
    {
        public static List<UserModel> Users { get; set; } = new List<UserModel>()
        {
            new UserModel() { UserName = "User 1", Email="user@gamil.com", Password="pass1", Role="Manager"},
            new UserModel() { UserName = "User 2", Email="user@gamil.com", Password="pass2", Role="Developer"},
        };
    }
}
