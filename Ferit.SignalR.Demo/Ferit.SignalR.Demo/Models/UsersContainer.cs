using System.Collections.Generic;

namespace Ferit.SignalR.Demo.Models
{
    public class UsersContainer
    {
        private List<User> Users { get; set; }
        public UsersContainer()
        {
            Users = new();
        }
        public void AddUser(string name, string username) {
            Users.Add(new User(name, username));
        }
        public IEnumerable<User> GetAllUsers() {
            return Users;
        }
        public void DeleteAllUsers() {
            Users.Clear();
        }
        

    }
}
