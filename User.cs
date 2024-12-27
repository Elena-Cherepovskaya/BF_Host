namespace BF_Host
{
    public class User
    {
        public string Name { get; set; } = string.Empty;

        public string Rule { get; set; } = string.Empty;
    }

    public class UserLogin
    {
        public string Name { get; set; } = string.Empty;

        public string Rule { get; set; } = "Client";

        public string Password { get; set; } = string.Empty;
    }
}
