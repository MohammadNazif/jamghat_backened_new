namespace Jamghat.Models.Auth
{
    public class models
    {

        public class TokenPayload
        {
            public string UserId { get; set; }
            public string Role { get; set; }
            public string? Id { get; set; }
        }

        public class CommonUserResponse
        {
            public string? message { get; set; }
            public string? User_type { get; set; }
            public Boolean success { get; set; }
            public string? name { get; set; }
            public string? role { get; set; }
            public string? user_id { get; set; }
            public string? username { get; set; }
            public string? oldpassword { get; set; }
            public string? password { get; set; }
            public string? email { get; set; }

            public string? token { get; set; }
            public int userId { get; set; }
        }
        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
