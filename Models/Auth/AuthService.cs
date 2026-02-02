using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using static Jamghat.Models.Auth.models;

namespace Jamghat.Models.Auth
{
    public class AuthService : IAuth
    {
        private readonly string _connectionString;
        private readonly IConfiguration _config;

        public AuthService(IConfiguration configuration, IConfiguration config)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _config = config;
        }

     
        public CommonUserResponse GetUser(string username, string password)
        {
            var response = new CommonUserResponse()
            {
                success = false
            };

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                response.message = "Enter credentials";
                return response;
            }

            using var con = new SqlConnection(_connectionString);
            con.Open();

            using var cmd = new SqlCommand("sp_login", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Email", username);
            cmd.Parameters.AddWithValue("@action", "getuserdetail");

            using var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();

                // Map user data
                var dbPassword = reader["Password"].ToString();
                var userId = reader["userid"] != DBNull.Value ? Convert.ToInt32(reader["userid"]) : 0;
                var email = reader["email"].ToString();
                var role = reader["role"]?.ToString(); 

                if (dbPassword == password)
                {
                    response.success = true;
                    response.message = "Login successful";
                    response.userId = userId;
                    response.email = email;
                    response.role = role;

                    // Optionally generate JWT here
                    var payload = new TokenPayload
                    {
                        UserId = userId.ToString(),
                        Role = role,
                        Id = userId.ToString() // optional extra
                    };
                    response.token = GenerateToken(payload);
                }
                else
                {
                    response.success = false;
                    response.message = "Invalid credentials!";
                }
            }
            else
            {
                response.message = "User not found!";
            }

            return response;
        }

   
        public string GenerateToken(TokenPayload payload)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, payload.UserId),
                new Claim(ClaimTypes.Role, payload.Role)
            };

            if (!string.IsNullOrEmpty(payload.Id))
            {
                claims.Add(new Claim("Id", payload.Id));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
