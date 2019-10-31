using System;

namespace Fanex.NAP.FPS.Renovation.Model.Auth
{
    public class UserModel
    {
        public Guid Id { get; protected set; }
        public string Email { get; protected set; }
        public string Name { get; protected set; }
        public string Password { get; protected set; }
        public string Salt { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected UserModel()
        {
        }

        public UserModel(string email, string name)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("User email can not be empty.");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("User name can not be empty.");
             
            }
            Id = Guid.NewGuid();
            Email = email.ToLowerInvariant();
            Name = name;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetPassword(string password, IEncrypter encrypter)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Password can not be empty.");                
            }
            Salt = encrypter.GetSalt();
            Password = encrypter.GetHash(password, Salt);
        }

        public bool ValidatePassword(string password, IEncrypter encrypter)
        {
            return Password.Equals(encrypter.GetHash(password, Salt));
        }
    }
}
