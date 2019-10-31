namespace Fanex.NAP.FPS.Renovation.Model.Auth
{
    public interface IEncrypter
    {
        string GetSalt();
        string GetHash(string value, string salt);
    }
}
