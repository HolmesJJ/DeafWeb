using Microsoft.EntityFrameworkCore;
using ProjectData;
using ProjectData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectServices
{
    public class ProjectService : IProject
    {
        private ProjectContext _context;

        public ProjectService(ProjectContext context)
        {
            _context = context;
        }

        public void AddSchedule(int english, int user, DateTime start_date, DateTime end_date)
        {
            var newSchedule = new Schedule
            {
                StartDate = start_date,
                EndDate = end_date,
                English = _context.English.FirstOrDefault(a => a.Id == english),
                User = _context.User.FirstOrDefault(a => a.Id == user)
            };

            _context.Schedule.Add(newSchedule);
            _context.SaveChanges();
        }

        public void updateJumpForFunScore(int user, int score)
        {
            var getUser = _context.User.FirstOrDefault(a => a.Id == user);
            getUser.JumpForFunScore = score;
            _context.User.Update(getUser);
            _context.SaveChanges();
        }

        public void updateCubeHubScore(int user, int score)
        {
            var getUser = _context.User.FirstOrDefault(a => a.Id == user);
            getUser.CubeHubScore = score;
            _context.User.Update(getUser);
            _context.SaveChanges();
        }

        public IEnumerable<Assistant> AllAssistant()
        {
            return _context.Assistant;
        }

        public IEnumerable<English> AllEnglish()
        {
            return _context.English;
        }

        public IEnumerable<Progress> AllProgress()
        {
            return _context.Progress;
        }

        public IEnumerable<Schedule> AllSchedule()
        {
            return _context.Schedule;
        }

        public IEnumerable<ProjectData.Models.Type> AllType()
        {
            return _context.Type;
        }

        public IEnumerable<User> AllUser()
        {
            return _context.User;
        }

        public IEnumerable<User> Leaderboard()
        {
            return _context.User.OrderByDescending(score => score.JumpForFunScore);
        }

        public IEnumerable<User> CubeHubLeaderboard()
        {
            return _context.User.OrderByDescending(score => score.CubeHubScore);
        }

        public string AssistantName(int id)
        {
            return _context.Assistant.FirstOrDefault(a => a.Id == id).Username;
        }

        public IEnumerable<English> EnglishProgress(int id)
        {
            IEnumerable<Progress> progress = _context.Progress.Where(a => a.User.Id == id).Select(result => new Progress
            {
                Id = result.Id,
                English = _context.English.FirstOrDefault(e => e.Id == _context.Progress.FirstOrDefault(a => a.Id == result.Id).English.Id)
            });
            IEnumerable<English> english = progress.Select(result => new English
            {
                Id = result.English.Id,
                Content = result.English.Content,
                Type = result.English.Type,
                Detail = result.English.Detail
            }).Select(a => a.Id).Distinct().Select(a => new English
            {
                Id = a,
                Content = _context.English.FirstOrDefault(i => i.Id == a).Content,
                Type = _context.English.FirstOrDefault(i => i.Id == a).Type,
                Detail = _context.English.FirstOrDefault(i => i.Id == a).Detail
            });
            return english;
        }

        public int CountEnglish(int user, int id)
        {
            IEnumerable<Progress> progress = _context.Progress.Where(a => a.User.Id == user).Select(result => new Progress
            {
                Id = result.Id,
                English = _context.English.FirstOrDefault(e => e.Id == _context.Progress.FirstOrDefault(a => a.Id == result.Id).English.Id)
            });
            IEnumerable<English> english = progress.Select(result => new English
            {
                Id = result.English.Id,
                Content = result.English.Content,
                Type = result.English.Type,
                Detail = result.English.Detail
            });
            return english.Where(e => e.Id == id).Count();
        }

        public English GetEnglishBySchedule(int id)
        {
            return _context.English.FirstOrDefault(e => e.Id == _context.Schedule.FirstOrDefault(a => a.Id == id).English.Id);
        }

        public English GetEnglishByProgress(int id)
        {
            // return _context.Progress.FirstOrDefault(a => a.Id == id).English;
            return _context.English.FirstOrDefault(e => e.Id == _context.Progress.FirstOrDefault(a => a.Id == id).English.Id);
        }

        public IEnumerable<English> GetEnglishByType(int id)
        {
            return _context.English.Where(a => a.Type.Id == id);
        }

        public IEnumerable<Progress> GetProgress(int id)
        {
            return _context.Progress
                .Include(progress => progress.English)
                .ThenInclude(english => english.Type)
                .Include(progress => progress.User)
                .ThenInclude(user => user.Assistant)
                .Where(a => a.User.Id == id);
        }

        public IEnumerable<Schedule> GetSchedule(int id)
        {
            return _context.Schedule
                .Include(schedule => schedule.English)
                .ThenInclude(english => english.Type)
                .Where(a => a.User.Id == id);
        }

        public IEnumerable<Progress> GetProgressByEnglish(int userId, int englishId)
        {
            return _context.Progress.Where(a => a.User.Id == userId && a.English.Id == englishId);
        }

        public IEnumerable<User> GetUser(int id)
        {
            return _context.User.Where(a => a.Assistant.Id == id);
        }

        public bool IsDeviceIdSaved(string base64DeviceId)
        {
            if (_context.User.Where(a => a.DeviceId == base64DeviceId).Any())
                return true;
            return false;

        }

        public bool IsUserResetPassword(string username, string newPassword, string validationCode)
        {
            if (_context.User.Where(a => a.Username == username).Any())
            {
                var getUser = _context.User.FirstOrDefault(a => a.Username == username);
                if (validationCode.Equals(getUser.ValidationCode))
                {
                    TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    if (Convert.ToInt32(ts.TotalSeconds) <= getUser.ExpiredTime)
                    {
                        getUser.Password = newPassword;
                        getUser.UserStatus = "IsValidated";
                        getUser.LoginTimes = 0;
                        _context.User.Update(getUser);
                        _context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        public void UpdateValidationCode(string username, string validationCode, int expiredTime)
        {
            if (_context.User.Where(a => a.Username == username).Any())
            {
                var getUser = _context.User.FirstOrDefault(a => a.Username == username);
                getUser.ValidationCode = validationCode;
                getUser.ExpiredTime = expiredTime;
                _context.User.Update(getUser);
                _context.SaveChanges();
            }
        }

        public int GetLastUserId()
        {
            return _context.User.LastOrDefault().Id;
        }

        public bool AddUser(string username, string email, string password, string profile, string gender, string age, string hearing, int assistantId)
        {
            var user = new User
            {
                Username = username,
                Password = password,
                Profile = profile,
                Gender = gender,
                Age = age,
                Hearing = hearing,
                Assistant = _context.Assistant.Where(a => a.Id == assistantId).FirstOrDefault(),
                JumpForFunScore = 0,
                CubeHubScore = 0,
                UserStatus = "IsValidated",
                Email = email,
                LoginTimes = 0,
                DeviceId = "abc",
                ValidationCode = "AAAA",
                ExpiredTime = 0
            };
            _context.User.Add(user);
            _context.SaveChanges();
            return true;
        }

        public int Login(string username, string password)
        {
            int AssistantId = 0;
            if (_context.Assistant.Where(a => a.Username == username && a.Password == password).Any())
            {
                var getAssistant = _context.Assistant.FirstOrDefault(a => a.Username == username && a.Password == password);
                if (getAssistant.UserStatus.Equals("IsValidated"))
                {
                    AssistantId = _context.Assistant.FirstOrDefault(a => a.Username == username && a.Password == password).Id;
                }
            }  
            return AssistantId;
        }

        public int getAssistantLoginTimes(string username)
        {
            var currentLoginTimes = 0;
            if (_context.Assistant.Where(a => a.Username == username).Any())
            {
                var getAssistant = _context.Assistant.FirstOrDefault(a => a.Username == username);
                currentLoginTimes = getAssistant.LoginTimes + 1;
                if (currentLoginTimes > 3)
                {
                    getAssistant.UserStatus = "isFrozen";
                    _context.Assistant.Update(getAssistant);
                }
                else
                {
                    getAssistant.LoginTimes = currentLoginTimes;
                    _context.Assistant.Update(getAssistant);
                }
                _context.SaveChanges();
            }
            return currentLoginTimes;
        }

        public int getUserLoginTimes(string username)
        {
            var currentLoginTimes = 0;
            if (_context.User.Where(a => a.Username == username).Any())
            {
                var getUser = _context.User.FirstOrDefault(a => a.Username == username);
                currentLoginTimes = getUser.LoginTimes + 1;
                if (currentLoginTimes > 3)
                {
                    getUser.UserStatus = "isFrozen";
                    _context.User.Update(getUser);
                }
                else
                {
                    getUser.LoginTimes = currentLoginTimes;
                    _context.User.Update(getUser);
                }
                _context.SaveChanges();
            }
            return currentLoginTimes;
        }

        public void AddPrgoress(int english, int userId)
        {
            var newProgress = new Progress
            {
                English = new English
                {
                    Id = _context.English.FirstOrDefault(a => a.Id == english).Id,
                    Content = _context.English.FirstOrDefault(a => a.Id == english).Content,
                    Detail = _context.English.FirstOrDefault(a => a.Id == english).Detail,
                    Type = _context.Type.FirstOrDefault(a => a.Id == _context.English.FirstOrDefault(e => e.Type.Id == english).Id)
                },
                User = new User
                {
                    Id = _context.User.FirstOrDefault(a => a.Id == userId).Id,
                    Username = _context.User.FirstOrDefault(a => a.Id == userId).Username,
                    Password = _context.User.FirstOrDefault(a => a.Id == userId).Password,
                    Profile = _context.User.FirstOrDefault(a => a.Id == userId).Profile,
                    Gender = _context.User.FirstOrDefault(a => a.Id == userId).Gender,
                    Age = _context.User.FirstOrDefault(a => a.Id == userId).Age,
                    Hearing = _context.User.FirstOrDefault(a => a.Id == userId).Hearing,
                    Assistant = _context.Assistant.Where(a => a.Id == _context.User.FirstOrDefault(u => u.Id == userId).Id).FirstOrDefault(),
                    JumpForFunScore = _context.User.FirstOrDefault(a => a.Id == userId).JumpForFunScore,
                    CubeHubScore = _context.User.FirstOrDefault(a => a.Id == userId).CubeHubScore,
                    UserStatus = _context.User.FirstOrDefault(a => a.Id == userId).UserStatus,
                    Email = _context.User.FirstOrDefault(a => a.Id == userId).Email,
                    LoginTimes = _context.User.FirstOrDefault(a => a.Id == userId).LoginTimes,
                    DeviceId = _context.User.FirstOrDefault(a => a.Id == userId).DeviceId,
                    ValidationCode = _context.User.FirstOrDefault(a => a.Id == userId).ValidationCode,
                    ExpiredTime = _context.User.FirstOrDefault(a => a.Id == userId).ExpiredTime
                },
                FinishDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"))
            };

            _context.Progress.Add(newProgress);
            _context.SaveChanges();
        }

        public int GetUserId(string Username)
        {
            if (_context.User.Where(a => a.Username == Username).Any())
            {
                return _context.User.FirstOrDefault(a => a.Username == Username).Id;
            }
            else
            {
                return 0;
            }
        }

        public string GetEmailByUserId(int userId)
        {
            if (_context.User.Where(a => a.Id == userId).Any())
            {
                return _context.User.FirstOrDefault(a => a.Id == userId).Email;
            }
            else
            {
                return null;
            }
        }

        public int UserLogin(string username, string password, string deviceId)
        {
            if (_context.User.Where(a => a.Username == username && a.Password == password).Any())
            {
                var getUser = _context.User.FirstOrDefault(a => a.Username == username && a.Password == password);
                if(getUser.UserStatus.Equals("IsValidated"))
                {
                    var userId = getUser.Id;
                    getUser.DeviceId = deviceId;
                    _context.User.Update(getUser);
                    _context.SaveChanges();
                    return userId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public int UserLoginWithValidationCode(string username, string password, string deviceId, string validationCode)
        {
            if (_context.User.Where(a => a.Username == username && a.Password == password).Any())
            {
                var getUser = _context.User.FirstOrDefault(a => a.Username == username && a.Password == password);
                if (validationCode.Equals(getUser.ValidationCode))
                {
                    TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    if (Convert.ToInt32(ts.TotalSeconds) <= getUser.ExpiredTime && getUser.UserStatus.Equals("IsValidated"))
                    {
                        var userId = getUser.Id;
                        getUser.UserStatus = "IsValidated";
                        getUser.LoginTimes = 0;
                        getUser.DeviceId = deviceId;
                        _context.User.Update(getUser);
                        _context.SaveChanges();
                        return userId;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public int FinishProgress(int userId, int englishId, int grade)
        {
            if (_context.User.Where(a => a.Id == userId).Any() && _context.English.Where(a => a.Id == englishId).Any())
            {
                Progress newProgress = new Progress
                {
                    English = new English
                    {
                        Id = _context.English.Where(a => a.Id == englishId).FirstOrDefault().Id,
                        Content = _context.English.Where(a => a.Id == englishId).FirstOrDefault().Content,
                        Detail = _context.English.Where(a => a.Id == englishId).FirstOrDefault().Detail,
                        Type = _context.Type.Where(t => t.Id == _context.English.Where(a => a.Id == englishId).FirstOrDefault().Type.Id).FirstOrDefault()
                    },
                    FinishDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")),
                    User = new User
                    {
                        Id = _context.User.FirstOrDefault(a => a.Id == userId).Id,
                        Username = _context.User.FirstOrDefault(a => a.Id == userId).Username,
                        Password = _context.User.FirstOrDefault(a => a.Id == userId).Password,
                        Profile = _context.User.FirstOrDefault(a => a.Id == userId).Profile,
                        Gender = _context.User.FirstOrDefault(a => a.Id == userId).Gender,
                        Age = _context.User.FirstOrDefault(a => a.Id == userId).Age,
                        Hearing = _context.User.FirstOrDefault(a => a.Id == userId).Hearing,
                        Assistant = _context.Assistant.Where(a => a.Id == _context.User.FirstOrDefault(u => u.Id == userId).Id).FirstOrDefault(),
                        JumpForFunScore = _context.User.FirstOrDefault(a => a.Id == userId).JumpForFunScore,
                        CubeHubScore = _context.User.FirstOrDefault(a => a.Id == userId).CubeHubScore,
                        UserStatus = _context.User.FirstOrDefault(a => a.Id == userId).UserStatus,
                        Email = _context.User.FirstOrDefault(a => a.Id == userId).Email,
                        LoginTimes = _context.User.FirstOrDefault(a => a.Id == userId).LoginTimes,
                        DeviceId = _context.User.FirstOrDefault(a => a.Id == userId).DeviceId,
                        ValidationCode = _context.User.FirstOrDefault(a => a.Id == userId).ValidationCode,
                        ExpiredTime = _context.User.FirstOrDefault(a => a.Id == userId).ExpiredTime
                    },
                    Grade = grade
                };

                _context.Add(newProgress);
                _context.SaveChanges();
                
                return newProgress.Id;
            }
            else
            {
                return -1;
            }
        }

        public IEnumerable<Schedule> GetTodaySchdule(int id)
        {
            IEnumerable<Schedule> schedule = _context.Schedule.Where(a => a.User.Id == id && DateTime.Now >= a.StartDate && a.EndDate >= DateTime.Now);
            return schedule;
        }

        public IEnumerable<Progress> GetTodayProgress(int id)
        {
            IEnumerable<Progress> progress = _context.Progress.Where(a => a.User.Id == id && DateTime.Now.ToString("yyyy-MM-dd") == a.FinishDate.ToString("yyyy-MM-dd"));
            return progress;
        }

        public ProjectData.Models.Type GetTypeByEnglish(int id)
        {
            return _context.Type.Where(a => a.Id == id).FirstOrDefault();
        }

        public void StartGame()
        {
            var item = _context.Game.FirstOrDefault();
            _context.Update(item);
            item.IsStart = 1;
            _context.SaveChanges();
            Thread.Sleep(60000);
            EndGame();
        }

        public void EndGame()
        {
            var item = _context.Game.FirstOrDefault();
            _context.Update(item);
            item.IsStart = 0;
            _context.SaveChanges();
        }

        public int GameStatus()
        {
            return _context.Game.FirstOrDefault().IsStart;
        }
    }
}
