namespace Fanex.NAP.DCS.Model.Auth
{
    public interface IEncrypter
    {
        string GetSalt();
        string GetHash(string value, string salt);
    }
}
