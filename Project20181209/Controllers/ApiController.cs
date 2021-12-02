using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using ProjectData;
using ProjectData.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using comparison_audio;
using MathWorks.MATLAB.NET.Arrays;
using Newtonsoft.Json;
using System.Security.Cryptography;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Project20181209.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Project20181209.Models;
using Project20181209.Encryption;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Project20181209.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class apiController : Controller
    {
        private IProject _assets;
        private readonly IHostingEnvironment iHostingEnvironment;

        public apiController(IProject assets, IHostingEnvironment he)
        {
            _assets = assets;
            iHostingEnvironment = he;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/recordMP3/{progressId}")]
        public FileContentResult recordMP3(int progressId)
        {
            string path = "wwwroot/audio/record/" + progressId + ".mp3";
            string filename = Path.GetFileName(path);
            var stream = new FileStream(path, FileMode.Open);
            long fileSize = stream.Length;
            byte[] fileBuffer = new byte[fileSize];
            stream.Read(fileBuffer, 0, (int)fileSize);
            stream.Close();

            return File(fileBuffer, "application/octet-stream", progressId + ".mp3");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("/api/englishId/{englishId}/mp3")]
        public FileContentResult englishMP3(int englishId)
        {
            string path = "wwwroot/audio/english/" + englishId + ".mp3";
            string filename = Path.GetFileName(path);
            var stream = new FileStream(path, FileMode.Open);
            long fileSize = stream.Length;
            byte[] fileBuffer = new byte[fileSize];
            stream.Read(fileBuffer, 0, (int)fileSize);
            stream.Close();

            return File(fileBuffer, "application/octet-stream", englishId + ".mp3");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("/api/englishId/{englishId}/tutorialVideo")]
        public FileContentResult tutorialVideo(int englishId)
        {
            string path = "wwwroot/video/tutorial/" + englishId + ".mp4";
            string filename = Path.GetFileName(path);
            var stream = new FileStream(path, FileMode.Open);
            long fileSize = stream.Length;
            byte[] fileBuffer = new byte[fileSize];
            stream.Read(fileBuffer, 0, (int)fileSize);
            stream.Close();

            return File(fileBuffer, "application/octet-stream", englishId + ".mp4");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("/api/englishId/{englishId}/exerciseVideo")]
        public FileContentResult exerciseVideo(int englishId)
        {
            string path = "wwwroot/video/exercise/" + englishId + ".mp4";
            string filename = Path.GetFileName(path);
            var stream = new FileStream(path, FileMode.Open);
            long fileSize = stream.Length;
            byte[] fileBuffer = new byte[fileSize];
            stream.Read(fileBuffer, 0, (int)fileSize);
            stream.Close();

            return File(fileBuffer, "application/octet-stream", englishId + ".mp4");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("/api/englishId/{englishId}/organVideo")]
        public FileContentResult organVideo(int englishId)
        {
            string path = "wwwroot/video/organ/" + englishId + ".mp4";
            string filename = Path.GetFileName(path);
            var stream = new FileStream(path, FileMode.Open);
            long fileSize = stream.Length;
            byte[] fileBuffer = new byte[fileSize];
            stream.Read(fileBuffer, 0, (int)fileSize);
            stream.Close();

            return File(fileBuffer, "application/octet-stream", englishId + ".mp4");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("/api/englishId/{id}/Frames")]
        public FileContentResult englishFrames(int id)
        {
            string path = "wwwroot/images/english/frames/" + id + ".zip";
            string filename = Path.GetFileName(path);
            var stream = new FileStream(path, FileMode.Open);
            long fileSize = stream.Length;
            byte[] fileBuffer = new byte[fileSize];
            stream.Read(fileBuffer, 0, (int)fileSize);
            stream.Close();

            return File(fileBuffer, "application/octet-stream", id + ".zip");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/refreshToken")]
        public JsonResult RefreshToken(string accessToken, string refreshToken, string grant_type)
        {
            var accessTokenPrincipal = JWTTokenOptions.GetPrincipalFromAccessToken(accessToken);
            if (accessTokenPrincipal is null)
            {
                return Json(new { Status = "Failed" });
            }
            var accessTokenUsername = accessTokenPrincipal.Identities.First().Claims.First().Value;

            var refreshTokenPrincipal = JWTTokenOptions.GetPrincipalFromAccessToken(refreshToken);
            if (refreshTokenPrincipal is null)
            {
                return Json(new { Status = "Failed" });
            }
            var refreshTokenUsername = refreshTokenPrincipal.Identities.First().Claims.First().Value;
            var refreshTokenValidTime = refreshTokenPrincipal.Identities.First().Claims.Take(3).Last().Value;
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);

            if (Convert.ToInt32(refreshTokenValidTime) > Convert.ToInt32(ts.TotalSeconds))
            {
                if (accessTokenUsername.Equals(refreshTokenUsername))
                {
                    var Jti = Guid.NewGuid().ToString();
                    var accessTokenClaims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.NameId, accessTokenUsername),
                        new Claim(JwtRegisteredClaimNames.Jti, Jti), // JWT ID,JWT的唯一标识
                        new Claim(ClaimTypes.Role, JWTTokenOptions.UserRole)
                    };

                    var jwtSecurityAccessToken = new JwtSecurityToken(
                            issuer: JWTTokenOptions.Issuer,
                            audience: JWTTokenOptions.Audience,
                            claims: accessTokenClaims,
                            expires: DateTime.Now.AddHours(1),
                            signingCredentials: JWTTokenOptions.Credentials()
                        );

                    var jwtAccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityAccessToken);
                    var tokenExpiration = jwtSecurityAccessToken.ValidTo;

                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { Status = "Succeeded", token = jwtAccessToken, expiration = tokenExpiration });
                }
                else
                {
                    // 不可以放Forbidden和Unauthorized
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { Status = "Failed" });
                }
            }
            else
            {
                // 不可以放Forbidden和Unauthorized
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { Status = "Failed" });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/login")]
        public JsonResult Login(string username, string password, string deviceId, string grant_type)
        {
            var rsa = RSAHelper.getInstance();
            string usernameDecryption = rsa.Decrypt(username);
            string passwordDecryption = rsa.Decrypt(password);
            string deviceIdDecryption = rsa.Decrypt(deviceId);

            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(deviceIdDecryption);
            string base64DeviceId = Convert.ToBase64String(bytes);

            int getUserId = 0;
            getUserId = _assets.UserLogin(usernameDecryption, MD5Helper.CreateMD5(passwordDecryption), base64DeviceId);
            if (getUserId != 0)
            {
                var Jti = Guid.NewGuid().ToString();
                var accessTokenClaims = new[]                       
                {
                    new Claim(JwtRegisteredClaimNames.NameId, usernameDecryption),
                    new Claim(JwtRegisteredClaimNames.Jti, Jti), // JWT ID,JWT的唯一标识
                    new Claim(ClaimTypes.Role, JWTTokenOptions.UserRole)
                };

                var refreshTokenClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.NameId, usernameDecryption),
                    new Claim(JwtRegisteredClaimNames.Jti, Jti), // JWT ID,JWT的唯一标识
                };

                var jwtSecurityAccessToken = new JwtSecurityToken(
                        issuer: JWTTokenOptions.Issuer,
                        audience: JWTTokenOptions.Audience,
                        claims: accessTokenClaims,
                        expires: DateTime.Now.AddHours(1),
                        signingCredentials: JWTTokenOptions.Credentials()
                    );

                var jwtSecurityRefreshToken = new JwtSecurityToken(
                        issuer: JWTTokenOptions.Issuer,
                        audience: JWTTokenOptions.Audience,
                        claims: refreshTokenClaims,
                        expires: DateTime.Now.AddDays(30),
                        signingCredentials: JWTTokenOptions.Credentials()
                    );

                var jwtAccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityAccessToken);
                var tokenExpiration = jwtSecurityAccessToken.ValidTo;

                var jwtRefreshToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityRefreshToken);

                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new {UserId = getUserId, Status = "Succeeded", token = jwtAccessToken, expiration = tokenExpiration, refresh_token = jwtRefreshToken });
            }
            else
            {
                if (_assets.getUserLoginTimes(usernameDecryption) > 3)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { UserId = getUserId, Status = "Failed", Error = "Frozen" });
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { UserId = getUserId, Status = "Failed", Error = "Wrong" });
                }
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/loginWithValidationCode")]
        public JsonResult LoginWithValidationCode(string username, string password, string deviceId, string grant_type, string validationCode)
        {
            var rsa = RSAHelper.getInstance();
            string usernameDecryption = rsa.Decrypt(username);
            string passwordDecryption = rsa.Decrypt(password);
            string validationCodeDecryption = rsa.Decrypt(validationCode);
            string deviceIdDecryption = rsa.Decrypt(deviceId);

            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(deviceIdDecryption);
            string base64DeviceId = Convert.ToBase64String(bytes);

            int getUserId = 0;
            getUserId = _assets.UserLoginWithValidationCode(usernameDecryption, MD5Helper.CreateMD5(passwordDecryption), base64DeviceId, validationCodeDecryption);
            if (getUserId != 0)
            {
                var Jti = Guid.NewGuid().ToString();
                var accessTokenClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.NameId, usernameDecryption),
                    new Claim(JwtRegisteredClaimNames.Jti, Jti), // JWT ID,JWT的唯一标识
                    new Claim(ClaimTypes.Role, JWTTokenOptions.UserRole)
                };

                var refreshTokenClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.NameId, usernameDecryption),
                    new Claim(JwtRegisteredClaimNames.Jti, Jti), // JWT ID,JWT的唯一标识
                };

                var jwtSecurityAccessToken = new JwtSecurityToken(
                        issuer: JWTTokenOptions.Issuer,
                        audience: JWTTokenOptions.Audience,
                        claims: accessTokenClaims,
                        expires: DateTime.Now.AddHours(1),
                        signingCredentials: JWTTokenOptions.Credentials()
                    );

                var jwtSecurityRefreshToken = new JwtSecurityToken(
                        issuer: JWTTokenOptions.Issuer,
                        audience: JWTTokenOptions.Audience,
                        claims: refreshTokenClaims,
                        expires: DateTime.Now.AddDays(30),
                        signingCredentials: JWTTokenOptions.Credentials()
                    );

                var jwtAccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityAccessToken);
                var tokenExpiration = jwtSecurityAccessToken.ValidTo;

                var jwtRefreshToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityRefreshToken);

                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { UserId = getUserId, Status = "Succeeded", token = jwtAccessToken, expiration = tokenExpiration, refresh_token = jwtRefreshToken });
            }
            else
            {
                if (_assets.getUserLoginTimes(usernameDecryption) > 3)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { UserId = getUserId, Status = "Failed", Error = "Frozen" });
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { UserId = getUserId, Status = "Failed", Error = "Wrong" });
                }
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/emailValidation")]
        public JsonResult EmailValidation(string username)
        {
            var rsa = RSAHelper.getInstance();
            string usernameDecryption = rsa.Decrypt(username);

            int getUserId = 0;
            getUserId = _assets.GetUserId(usernameDecryption);

            if (getUserId != 0)
            {
                try
                {
                    string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                    Random randrom = new Random((int)DateTime.Now.Ticks);
                    string validationCode = "";
                    for (int i = 0; i < 4; i++)
                    {
                        validationCode += chars[randrom.Next(chars.Length)];
                    }

                    TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    int expiredTime = Convert.ToInt32(ts.TotalSeconds) + 10 * 60;

                    _assets.UpdateValidationCode(usernameDecryption, validationCode, expiredTime);
                    string email = _assets.GetEmailByUserId(getUserId);

                    if(email != null)
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("Avtant", "jbawesomeavtant@gmail.com"));
                        message.To.Add(new MailboxAddress(usernameDecryption, email));
                        message.Subject = "AvTant Email Validation";
                        message.Body = new TextPart("plain")
                        {
                            Text = "Hello " + usernameDecryption + ", your email validation code is " + validationCode + ", please validate within 10 minutes."
                        };
                        using (var client = new SmtpClient())
                        {
                            // 注意，需要在服务器上安装Google浏览器，并登录当前Google账号
                            client.CheckCertificateRevocation = false;
                            // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                            client.Connect("smtp.gmail.com", 465, SecureSocketOptions.Auto);
                            // Note: since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism
                            client.AuthenticationMechanisms.Remove("XOAUTH2");
                            client.Authenticate("jbawesomeavtant@gmail.com", "199610240as");
                            client.Send(message);
                            client.Disconnect(true);
                        }
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        return Json(new { Status = "Succeeded" });
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        return Json(new { Status = "Email Empty" });
                    }
                }
                catch (Exception e)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json(new { Status = "Internal Server Error", Message = e });
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Status = "Failed" });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/deviceIdValidation")]
        public JsonResult DeviceIdValidation(string deviceId)
        {
            var rsa = RSAHelper.getInstance();
            string deviceIdDecryption = rsa.Decrypt(deviceId);

            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(deviceIdDecryption);
            string base64DeviceId = Convert.ToBase64String(bytes);

            if (_assets.IsDeviceIdSaved(base64DeviceId))
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { Status = "Succeeded" });
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Status = "Failed" });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/resetPassword")]
        public JsonResult ResetPassword(string username, string newPassword, string validationCode)
        {
            var rsa = RSAHelper.getInstance();
            string usernameDecryption = rsa.Decrypt(username);
            string newPasswordDecryption = rsa.Decrypt(newPassword);
            string validationCodeDecryption = rsa.Decrypt(validationCode);

            if (_assets.IsUserResetPassword(usernameDecryption, MD5Helper.CreateMD5(newPasswordDecryption), validationCodeDecryption))
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { Status = "Succeeded" });
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Status = "Failed" });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/getPublicKey")]
        public JsonResult getPublicKey()
        {
            var publicKey = RSAHelper.publicKey;
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { Status = "Succeeded", PublicKey = publicKey });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/backendMatching/getAccessToken")]
        public JsonResult GetAccessToken(IFormFile image2File)
        {
            string username = Request.Form["username"];
            string apiKey = Request.Form["apiKey"];
            string secretKey = Request.Form["secretKey"];

            var rsa = RSAHelper.getInstance();
            string usernameDecryption = rsa.Decrypt(username);
            string apiKeyDecryption = rsa.Decrypt(apiKey);
            string secretKeyDecryption = rsa.Decrypt(secretKey);

            int getUserId = 0;
            getUserId = _assets.GetUserId(usernameDecryption);

            string tempFile = getUserId + ".png";

            if (getUserId != 0)
            {
                FileInfo file = new FileInfo(Path.Combine(iHostingEnvironment.WebRootPath + "/images/face", tempFile));
                try
                {
                    using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
                    {
                        image2File.CopyTo(fs);
                        fs.Flush();
                        fs.Close();
                    }

                    String authHost = "https://aip.baidubce.com/oauth/2.0/token";
                    HttpClient client = new HttpClient();
                    List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
                    paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                    paraList.Add(new KeyValuePair<string, string>("client_id", apiKeyDecryption));
                    paraList.Add(new KeyValuePair<string, string>("client_secret", secretKeyDecryption));

                    HttpResponseMessage tokenResponse = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
                    String tokenResult = tokenResponse.Content.ReadAsStringAsync().Result;
                    var tokenJsonObject = JsonConvert.DeserializeObject<AccessTokenResult>(tokenResult);

                    string image1Path = "wwwroot/images/profile/" + getUserId + ".png";
                    var image1FileStream = System.IO.File.OpenRead(image1Path);
                    byte[] image1Bytes = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        image1FileStream.CopyTo(memoryStream);
                        image1Bytes = memoryStream.ToArray();
                        memoryStream.Flush();
                        memoryStream.Close();
                    }
                    image1FileStream.Flush();
                    image1FileStream.Close();
                    string image1Base64 = Convert.ToBase64String(image1Bytes);

                    string image2Path = "wwwroot/images/face/" + getUserId + ".png";
                    var image2FileStream = System.IO.File.OpenRead(image2Path);
                    byte[] image2Bytes = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        image2FileStream.CopyTo(memoryStream);
                        image2Bytes = memoryStream.ToArray();
                        memoryStream.Flush();
                        memoryStream.Close();
                    }
                    image2FileStream.Flush();
                    image2FileStream.Close();
                    string image2Base64 = Convert.ToBase64String(image2Bytes);

                    string faceRegHost = "https://aip.baidubce.com/rest/2.0/face/v3/match?access_token=" + tokenJsonObject.access_token;
                    Encoding encoding = Encoding.Default;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(faceRegHost); 
                    request.Method = "post";
                    request.KeepAlive = true;
                    String image1 = "[{\"image\":\"" + image1Base64 + "\",\"image_type\":\"BASE64\",\"face_type\":\"LIVE\",\"quality_control\":\"LOW\",\"liveness_control\":\"NONE\"},";
                    String image2 = "{\"image\":\"" + image2Base64 + "\",\"image_type\":\"BASE64\",\"face_type\":\"LIVE\",\"quality_control\":\"LOW\",\"liveness_control\":\"NONE\"}]";
                    byte[] buffer = encoding.GetBytes(image1 + image2);
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                    HttpWebResponse compareResponse = (HttpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(compareResponse.GetResponseStream(), Encoding.Default);
                    string compareResult = reader.ReadToEnd();
                    Console.WriteLine(compareResult);

                    var resultJsonObject = JsonConvert.DeserializeObject<FaceRecognitionResult>(compareResult);
                    double score = resultJsonObject.result.score;

                    if(score > 80)
                    {
                        var Jti = Guid.NewGuid().ToString();
                        var accessTokenClaims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.NameId, usernameDecryption),
                            new Claim(JwtRegisteredClaimNames.Jti, Jti), // JWT ID,JWT的唯一标识
                            new Claim(ClaimTypes.Role, JWTTokenOptions.UserRole)
                        };

                        var refreshTokenClaims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.NameId, usernameDecryption),
                            new Claim(JwtRegisteredClaimNames.Jti, Jti), // JWT ID,JWT的唯一标识
                        };

                        var jwtSecurityAccessToken = new JwtSecurityToken(
                                issuer: JWTTokenOptions.Issuer,
                                audience: JWTTokenOptions.Audience,
                                claims: accessTokenClaims,
                                expires: DateTime.Now.AddHours(1),
                                signingCredentials: JWTTokenOptions.Credentials()
                            );

                        var jwtSecurityRefreshToken = new JwtSecurityToken(
                                issuer: JWTTokenOptions.Issuer,
                                audience: JWTTokenOptions.Audience,
                                claims: refreshTokenClaims,
                                expires: DateTime.Now.AddDays(30),
                                signingCredentials: JWTTokenOptions.Credentials()
                            );

                        var jwtAccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityAccessToken);
                        var tokenExpiration = jwtSecurityAccessToken.ValidTo;

                        var jwtRefreshToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityRefreshToken);

                        Response.StatusCode = (int)HttpStatusCode.OK;
                        return Json(new { UserId = getUserId, Status = "Succeeded", token = jwtAccessToken, expiration = tokenExpiration, refresh_token = jwtRefreshToken });
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.NotFound;
                        return Json(new { Status = "Failed" });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Status = "Failed" });
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Status = "Failed" });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/frontendMatching/getAccessToken")]
        public JsonResult GetAccessToken()
        {
            string username = Request.Form["username"];

            var rsa = RSAHelper.getInstance();
            string usernameDecryption = rsa.Decrypt(username);

            int getUserId = 0;
            getUserId = _assets.GetUserId(usernameDecryption);

            if (getUserId != 0)
            {
                var Jti = Guid.NewGuid().ToString();
                var accessTokenClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.NameId, usernameDecryption),
                    new Claim(JwtRegisteredClaimNames.Jti, Jti), // JWT ID,JWT的唯一标识
                    new Claim(ClaimTypes.Role, JWTTokenOptions.UserRole)
                };

                var refreshTokenClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.NameId, usernameDecryption),
                    new Claim(JwtRegisteredClaimNames.Jti, Jti), // JWT ID,JWT的唯一标识
                };

                var jwtSecurityAccessToken = new JwtSecurityToken(
                        issuer: JWTTokenOptions.Issuer,
                        audience: JWTTokenOptions.Audience,
                        claims: accessTokenClaims,
                        expires: DateTime.Now.AddHours(1),
                        signingCredentials: JWTTokenOptions.Credentials()
                    );

                var jwtSecurityRefreshToken = new JwtSecurityToken(
                        issuer: JWTTokenOptions.Issuer,
                        audience: JWTTokenOptions.Audience,
                        claims: refreshTokenClaims,
                        expires: DateTime.Now.AddDays(30),
                        signingCredentials: JWTTokenOptions.Credentials()
                    );

                var jwtAccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityAccessToken);
                var tokenExpiration = jwtSecurityAccessToken.ValidTo;

                var jwtRefreshToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityRefreshToken);

                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { UserId = getUserId, Status = "Succeeded", token = jwtAccessToken, expiration = tokenExpiration, refresh_token = jwtRefreshToken });
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Status = "Failed" });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/userImage")]
        public JsonResult GetUserImage()
        {
            string username = Request.Form["username"];

            var rsa = RSAHelper.getInstance();
            string usernameDecryption = rsa.Decrypt(username);

            int getUserId = 0;
            getUserId = _assets.GetUserId(usernameDecryption);
            if (getUserId != 0)
            {
                try
                {
                    string image2Path = "wwwroot/images/profile/" + getUserId + ".png";
                    var image2FileStream = System.IO.File.OpenRead(image2Path);
                    byte[] image2Bytes = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        image2FileStream.CopyTo(memoryStream);
                        image2Bytes = memoryStream.ToArray();
                        memoryStream.Flush();
                        memoryStream.Close();
                    }
                    image2FileStream.Flush();
                    image2FileStream.Close();

                    string base64UserImage = Convert.ToBase64String(image2Bytes);

                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { Status = "Succeeded", userImage = base64UserImage });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json(new { Status = "Failed" });
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Status = "Failed" });
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("/api/userId/{userId}/schedule/progress")]
        public JsonResult TodaySchedule(int userId)
        {
            // var today = DateTime.Now.ToString("yyyy-MM-dd");
            IEnumerable<Schedule> schedule = _assets.GetTodaySchdule(userId);
            schedule = schedule.Select(result => new Schedule
            {
                Id = result.Id,
                StartDate = result.StartDate,
                EndDate = result.EndDate,
                User = result.User,
                English = _assets.GetEnglishBySchedule(result.Id)
            });
       
            IEnumerable<Progress> progress = _assets.GetTodayProgress(userId);
            progress = progress.Where(grade => grade.Grade > 90).Select(result => new Progress
            {
                Id = result.Id,
                User = result.User,
                English = _assets.GetEnglishByProgress(result.Id),
                FinishDate = result.FinishDate
            });

            IEnumerable<English> english = progress.Select(result => new English
            {
                Id = _assets.GetEnglishByProgress(result.Id).Id,
                Content = _assets.GetEnglishByProgress(result.Id).Content,
                Detail = _assets.GetEnglishByProgress(result.Id).Detail,
                Type = _assets.GetTypeByEnglish(_assets.GetEnglishByProgress(result.Id).Id)
            });

            IEnumerable<SchduleResult> schduleResult = schedule.Select(result => new SchduleResult
            {
                ScheduleId = result.Id,
                StartDate = result.StartDate,
                EndDate = result.EndDate,
                EnglishId = result.English.Id,
                Content = result.English.Content,
                ProgressTimes = english.Where(a => a.Id == result.English.Id).Count()
            });

            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(schduleResult);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("/api/english/{id}")]
        public JsonResult EnglishDetail(int id)
        {
            English english = _assets.AllEnglish().Where(a => a.Id == id).FirstOrDefault();

            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(english);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [Route("/api/finishProgress")]
        public JsonResult FinishProgress(int userId, int englishId, int grade)
        {
            var progressId = _assets.FinishProgress(userId, englishId, grade);
            if (progressId > -1)
            {
                System.IO.File.Copy(iHostingEnvironment.WebRootPath + "/audio/study/" + userId + "_temp.mp3", iHostingEnvironment.WebRootPath + "/audio/record/" + progressId + ".mp3", true);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { Status = "Succeeded" });
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Status = "Failed" });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/upload")]
        public JsonResult Upload(IFormFile audioFile)
        {
            string userId = Request.Form["userId"];
            string englishId = Request.Form["englishId"];
            string tempFile = userId + "_temp.mp3";

            FileInfo file = new FileInfo(Path.Combine(iHostingEnvironment.WebRootPath + "/audio/study", tempFile));

            try
            {
                CmpAudio cmpAudio = new CmpAudio();
                CmpResult cmpResult = new CmpResult();

                using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
                {
                    audioFile.CopyTo(fs);
                    fs.Flush();
                    fs.Close();
                }

                MWCharArray d1_path = Path.Combine(iHostingEnvironment.WebRootPath + "/audio/english", englishId + ".mp3");
                MWCharArray d2_path = Path.Combine(iHostingEnvironment.WebRootPath + "/audio/study", tempFile);

                var r = cmpAudio.comparison_audio(6, d1_path, d2_path);
                cmpResult.Diff = r[0].ToArray().Cast<double>().ToArray();
                cmpResult.D1 = r[1].ToArray().Cast<double>().ToArray();
                cmpResult.D2 = r[2].ToArray().Cast<double>().ToArray();
                cmpResult.Step1 = r[3].ToArray().Cast<Int32>().ToArray();
                cmpResult.Step2 = r[4].ToArray().Cast<Int32>().ToArray();
                cmpResult.StepDiff = r[5].ToArray().Cast<Int32>().ToArray();

                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(cmpResult);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { Comment = e });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/analyze")]
        public JsonResult Analyze(IFormFile captureFile)
        {
            string userId = Request.Form["userId"];
            string englishId = Request.Form["englishId"];
            string tempFile = userId + "_temp.mp4";

            FileInfo file = new FileInfo(Path.Combine(iHostingEnvironment.WebRootPath + "/video/capture", tempFile));

            try
            {
                using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
                {
                    captureFile.CopyTo(fs);
                    fs.Flush();
                    fs.Close();
                }

                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { Status = "Succeeded" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { Status = "Failed", Comment = e });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/jumpForFunStep")]
        public JsonResult JumpForFunStep()
        {
            string userId = Request.Form["userId"];
            string englishId = Request.Form["englishId"];
            string recordString = Request.Form["recordString"];

            string tempFile = userId + "_temp.wav";

            Byte[] recordBytes = Convert.FromBase64String(recordString);
            var recordfile = System.IO.File.Create(iHostingEnvironment.WebRootPath + "/audio/jumpForFun/" + tempFile, recordBytes.Length);

            int finalAvgScore = 0;

            try
            {
                CmpAudio cmpAudio = new CmpAudio();
                CmpResult cmpResult = new CmpResult();

                recordfile.Write(recordBytes, 0, recordBytes.Length);
                recordfile.Flush();
                recordfile.Close();

                MWCharArray d1_path = Path.Combine(iHostingEnvironment.WebRootPath + "/audio/english", englishId + ".mp3");
                MWCharArray d2_path = Path.Combine(iHostingEnvironment.WebRootPath + "/audio/jumpForFun", tempFile);

                var r = cmpAudio.comparison_audio(6, d1_path, d2_path);
                cmpResult.Diff = r[0].ToArray().Cast<double>().ToArray();
                cmpResult.D1 = r[1].ToArray().Cast<double>().ToArray();
                cmpResult.D2 = r[2].ToArray().Cast<double>().ToArray();
                cmpResult.Step1 = r[3].ToArray().Cast<Int32>().ToArray();
                cmpResult.Step2 = r[4].ToArray().Cast<Int32>().ToArray();
                cmpResult.StepDiff = r[5].ToArray().Cast<Int32>().ToArray();

                int[] StepDiff = cmpResult.StepDiff;
                double[] Diff = cmpResult.Diff;
                List<Double> scoreList = new List<double>();
                double finalSumScore = 0;

                for (int i = 0; i < StepDiff.Length; i++)
                {
                    if (i > 0 && i % 2 == 1)
                    {
                        double actualScore = 0;
                        double totalScore = 0;
                        for (int j = StepDiff[i - 1]; j < StepDiff[i]; j++)
                        {
                            totalScore++;
                            if (Diff[j] <= 6)
                            {
                                actualScore++;
                            }
                        }
                        scoreList.Add(actualScore / totalScore);
                    }
                }

                for (int k = 0; k < scoreList.Count; k++)
                {
                    finalSumScore = finalSumScore + scoreList[k];
                }
                finalAvgScore = Convert.ToInt32(finalSumScore / scoreList.Count * 100);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { Result = finalAvgScore, Status = -1, Comment = e });
            }

            if (finalAvgScore < 90)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { Result = finalAvgScore, Status = 1, Comment = "Your pronunciation is not correct" });
            }
            else if (finalAvgScore >= 80 && finalAvgScore <= 110)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { Result = finalAvgScore, Status = 0, Comment = "Your pronunciation is correct" });
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { Result = finalAvgScore, Status = 2, Comment = "Your pronunciation is not correct" });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/jumpForFunYourPronunciation/userId/{userId}")]
        public FileContentResult JumpForFunYourPronunciation(int userId)
        {
            string path = "wwwroot/audio/jumpForFun/" + userId + "_temp.mp3";
            string filename = Path.GetFileName(path);
            var stream = new FileStream(path, FileMode.Open);
            long fileSize = stream.Length;
            byte[] fileBuffer = new byte[fileSize];
            stream.Read(fileBuffer, 0, (int)fileSize);
            stream.Close();

            Response.StatusCode = (int)HttpStatusCode.OK;
            return File(fileBuffer, "application/octet-stream", userId + "_temp.mp3");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/jumpForFunStandardPronunciation/englishId/{englishId}")]
        public FileContentResult JumpForFunStandardPronunciation(int englishId)
        {
            string path = "wwwroot/audio/english/" + englishId + ".mp3";
            string filename = Path.GetFileName(path);
            var stream = new FileStream(path, FileMode.Open);
            long fileSize = stream.Length;
            byte[] fileBuffer = new byte[fileSize];
            stream.Read(fileBuffer, 0, (int)fileSize);
            stream.Close();

            Response.StatusCode = (int)HttpStatusCode.OK;
            return File(fileBuffer, "application/octet-stream", englishId + ".mp3");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/jumpForFunScore")]
        public JsonResult JumpForFunScore()
        {
            int userId = Convert.ToInt32(Request.Form["userId"]);
            int score = Convert.ToInt32(Request.Form["score"]);

            _assets.updateJumpForFunScore(userId, score);

            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { Status = "Succeeded" });
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [Route("/api/cubeHubScore")]
        public JsonResult cubeHubScore(string userId, string score)
        {
            var rsa = RSAHelper.getInstance();
            string userIdDecryption = rsa.Decrypt(userId);
            string scoreDecryption = rsa.Decrypt(score);

            int getUserId = Convert.ToInt32(userIdDecryption);
            int getScore = Convert.ToInt32(scoreDecryption);

            _assets.updateCubeHubScore(getUserId, getScore);

            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { Status = "Succeeded" });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/leaderboard")]
        public JsonResult Leaderboard()
        {
            List<User> userList = _assets.Leaderboard().ToList();
            List<LeaderboardResult> LeaderboardResultList = new List<LeaderboardResult>();

            for (int i = 0; i < userList.Count; i++)
            {
                LeaderboardResult leaderboardResult = new LeaderboardResult();
                leaderboardResult.rank = i + 1;
                leaderboardResult.name = userList[i].Username;
                leaderboardResult.step = userList[i].JumpForFunScore;

                LeaderboardResultList.Add(leaderboardResult);
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(LeaderboardResultList);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("/api/cubeHubLeaderboard")]
        public JsonResult CubeHubLeaderboard()
        {
            List<User> userList = _assets.CubeHubLeaderboard().ToList();
            List<CubeHubLeaderboardResult> CubeHubLeaderboardResultList = new List<CubeHubLeaderboardResult>();

            for (int i = 0; i < userList.Count; i++)
            {
                CubeHubLeaderboardResult cubeHubLeaderboardResult = new CubeHubLeaderboardResult();
                cubeHubLeaderboardResult.rank = i + 1;
                cubeHubLeaderboardResult.name = userList[i].Username;
                cubeHubLeaderboardResult.score = userList[i].CubeHubScore;

                CubeHubLeaderboardResultList.Add(cubeHubLeaderboardResult);
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(CubeHubLeaderboardResultList);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/correctDemo")]
        public JsonResult correctDemo()
        {
            string path = "wwwroot/demo/correct.json";
            CmpResult cmpResult = new CmpResult();

            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                cmpResult = JsonConvert.DeserializeObject<CmpResult>(json);
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(cmpResult);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/wrongDemo")]
        public JsonResult wrongDemo()
        {
            string path = "wwwroot/demo/wrong.json";
            CmpResult cmpResult = new CmpResult();

            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                cmpResult = JsonConvert.DeserializeObject<CmpResult>(json);
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(cmpResult);
        }

        public class AccessTokenResult
        {
            public string refresh_token { get; set; }
            public int expires_in { get; set; }
            public string session_key { get; set; }
            public string access_token { get; set; }
            public string scope { get; set; }
            public string session_secret { get; set; }
        }

        public class FaceRecognitionResult
        {
            public FaceRecognitionScore result { get; set; }
        }

        public class FaceRecognitionScore
        {
            public double score { get; set; }
        }

        private class SchduleResult
        {
            public int ScheduleId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int EnglishId { get; set; }
            public string Content { get; set; }
            public int ProgressTimes { get; set; }
        }

        public class CmpResult
        {
            public double[] Diff { get; set; }
            public double[] D1 { get; set; }
            public double[] D2 { get; set; }
            public Int32[] Step1 { get; set; }
            public Int32[] Step2 { get; set; }
            public Int32[] StepDiff { get; set; }
        }

        public class LeaderboardResult
        {
            public int rank { get; set; }
            public string name { get; set; }
            public int step { get; set; }
        }

        public class CubeHubLeaderboardResult
        {
            public int rank { get; set; }
            public string name { get; set; }
            public int score { get; set; }
        }
    }
}
