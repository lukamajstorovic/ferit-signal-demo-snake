namespace Ferit.SignalR.Demo.Models
{
    public class User
    {
        public string Name { get; private set; }
        public string Username { get; private set; }

        public User(string name, string username) {
            this.Name = name;
            this.Username = username;
        }
    }
}
