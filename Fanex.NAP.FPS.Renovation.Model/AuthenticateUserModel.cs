namespace Fanex.NAP.FPS.Renovation.Model
{
    public class AuthenticateUserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }
}
