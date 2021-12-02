using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project20181209.Encryption;
using Project20181209.Models;
using Project20181209.Tokens;
using ProjectData;
using ProjectData.Models;

namespace Project20181209.Controllers
{
    public class ProjectController : Controller
    {
        private IProject _assets;

        public ProjectController(IProject assets)
        {
            _assets = assets;
        }

        public IActionResult AddUser(AddUserModel model)
        {
            // read cookie
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                int AssistantId = int.Parse(HttpContext.User.Claims.First().Value);
                ViewData["publicKey"] = RSAHelper.publicKey;
                ViewData["AssistantId"] = AssistantId;
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", new AssistantLoginModel { Message = "Please login first!" });
            }
        }

        public IActionResult UploadUser(AddUserModel model)
        {
            // read cookie
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                IFormFile profile = model.Profile;
                string fileExtension = System.IO.Path.GetExtension(profile.FileName);
                if (!fileExtension.Equals(".jpg") && !fileExtension.Equals(".png"))
                {
                    return RedirectToAction("AddUser", new AddUserModel { Message = "Image must be .jpg / .png!" });
                }

                var rsa = RSAHelper.getInstance();

                string usernameDecryption = rsa.Decrypt(model.Username);
                if (_assets.GetUserId(usernameDecryption) > 0)
                {
                    return RedirectToAction("AddUser", new AddUserModel { Message = "user existed!" });
                }
                string emailDecryption = rsa.Decrypt(model.Email);
                string passwordDecryption = rsa.Decrypt(model.Password);
                string genderDecryption = rsa.Decrypt(model.Gender);
                string ageDecryption = rsa.Decrypt(model.Age);
                string hearingDecryption = rsa.Decrypt(model.Hearing);
                string assistantIdDecryption = rsa.Decrypt(model.AssistantId);
                
                int userId = _assets.GetLastUserId() + 1;
                string savedPath = "profile/" + userId + System.IO.Path.GetExtension(profile.FileName);
                string filePath = "wwwroot/images/profile/" + userId + System.IO.Path.GetExtension(profile.FileName);
                FileInfo file = new FileInfo(filePath);
                try
                {
                    using (FileStream fs = new FileStream(file.ToString(), System.IO.FileMode.Create))
                    {
                        profile.CopyTo(fs);
                        fs.Flush();
                        fs.Close();
                    }

                    if (_assets.AddUser(usernameDecryption, emailDecryption, MD5Helper.CreateMD5(passwordDecryption), savedPath, genderDecryption, ageDecryption, hearingDecryption, Convert.ToInt32(assistantIdDecryption)))
                    {
                        return RedirectToAction("Home", new HomeModel { AssistantName = _assets.AssistantName(Convert.ToInt32(assistantIdDecryption)), User = _assets.GetUser(Convert.ToInt32(assistantIdDecryption)) });
                    }
                    else
                    {
                        return RedirectToAction("AddUser", new AddUserModel { Message = "Server Error!" });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return RedirectToAction("AddUser", new AddUserModel { Message = "Server Error!" });
                }
            }
            else
            {
                return RedirectToAction("Login", new AssistantLoginModel { Message = "Server Error!" });
            }
        }

        public IActionResult Login(AssistantLoginModel model)
        {
            ViewData["publicKey"] = RSAHelper.publicKey;
            HttpContext.SignOutAsync();
            return View(model);
        }

        public IActionResult Home(AssistantLoginModel model)
        {
            // read cookie
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                int AssistantId = int.Parse(HttpContext.User.Claims.First().Value);
                return View(new HomeModel { AssistantName = _assets.AssistantName(AssistantId), User = _assets.GetUser(AssistantId) });
            }
            else
            {
                if(model.Username != null)
                {
                    var rsa = RSAHelper.getInstance();
                    string usernameDecryption = rsa.Decrypt(model.Username);
                    string passwordDecryption = rsa.Decrypt(model.Password);

                    // login
                    int AssistantId = _assets.Login(usernameDecryption, MD5Helper.CreateMD5(passwordDecryption));

                    if (AssistantId != 0)
                    {
                        // add user to cookie
                        var claims = new[] { new Claim("AssistantId", AssistantId.ToString()) };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal user = new ClaimsPrincipal(claimsIdentity);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user).Wait();
                        return View(new HomeModel { AssistantName = _assets.AssistantName(AssistantId), User = _assets.GetUser(AssistantId) });
                    }
                    else
                    {
                        // return login page
                        if (_assets.getAssistantLoginTimes(usernameDecryption) > 3)
                        {
                            return RedirectToAction("Login", new AssistantLoginModel { IsValidated = false, Message = "Your account has been froen, please reset your password" });
                        }
                        else
                        {
                            return RedirectToAction("Login", new AssistantLoginModel { Message = "User does not exist or password is incorrect" });
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login", new AssistantLoginModel { Message = "Please login first!" });
                }
            }
        }

        public IActionResult UserDetail(int id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var model = new UserDetailModel
                {
                    User = _assets.AllUser().FirstOrDefault(user => user.Id == id),
                    Schedule = _assets.GetSchedule(id),
                    Progress = _assets.GetProgress(id),
                    Type = _assets.AllType(),
                    English = _assets.AllEnglish(),
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult Chart(int userId, int englishId)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var model = new ChartModel
                {
                    User = _assets.AllUser().FirstOrDefault(user => user.Id == userId),
                    English = _assets.AllEnglish().FirstOrDefault(english => english.Id == englishId),
                    Progress = _assets.GetProgressByEnglish(userId, englishId),
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult AddSchedule(UserDetailModel model)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                _assets.AddSchedule(model.NewScheduleEnglish,model.NewScheduleUser,model.NewScheduleStartDate,model.NewScheduleEndDate);
                return RedirectToAction("UserDetail", new { id = model.NewScheduleUser });
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}
