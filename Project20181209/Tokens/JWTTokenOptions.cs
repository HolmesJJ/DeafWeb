using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project20181209.Tokens
{
    public class JWTTokenOptions
    {
        public static string Issuer = "TestIssuer";
        public static string Audience = "TestAudience";
        public static string UserRole = "User";
        public static string AssistantRole = " Assistant";
        public static string RefreshTokenRole = " RefreshToken";
        private static string signingKey = "q9vtCK2Uu7FVE11yicMJdUd79zPacA3m";

        public static SymmetricSecurityKey Key()
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            return symmetricSecurityKey;
        }

        public static SigningCredentials Credentials()
        {
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(Key(), SecurityAlgorithms.HmacSha256);
            return signingCredentials;
        }

        /// <summary>
        /// 生成刷新Token
        /// </summary>
        /// <returns></returns>
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// 从Token中获取用户身份
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ClaimsPrincipal GetPrincipalFromAccessToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                return handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidateLifetime = false
                }, out SecurityToken validatedToken);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
