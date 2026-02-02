using static Jamghat.Models.Auth.models;

namespace Jamghat.Models.Auth
{
    public interface IAuth
    {
         public CommonUserResponse GetUser(string username, string password);
        string GenerateToken(TokenPayload payload);
    }
}
