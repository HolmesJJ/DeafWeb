using ProjectData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectData
{
    public interface IProject
    {
        IEnumerable<Assistant> AllAssistant();
        IEnumerable<English> AllEnglish();
        IEnumerable<Progress> AllProgress();
        IEnumerable<Schedule> AllSchedule();
        IEnumerable<Models.Type> AllType();
        IEnumerable<User> AllUser();
        IEnumerable<User> Leaderboard();
        IEnumerable<User> CubeHubLeaderboard();

        // pass user id
        IEnumerable<Progress> GetProgress(int userId);
        IEnumerable<Schedule> GetSchedule(int userId);
        IEnumerable<User> GetUser(int userId);
        IEnumerable<English> EnglishProgress(int userId);

        int CountEnglish(int userId, int progressId); // pass progress id

        int Login(string username, string password);

        bool IsDeviceIdSaved(string base64DeviceId); // device id

        int getAssistantLoginTimes(string username); // Assistant Login times

        int getUserLoginTimes(string username); // User Login times

        string AssistantName(int assistantId); // pass assistant id

        bool IsUserResetPassword(string username, string newPassword, string validationCode); // user reset pssword

        int GetLastUserId();
        bool AddUser(string username, string email, string password, string profile, string gender, string age, string hearing, int assistantId);
        void UpdateValidationCode(string username, string validationCode, int expiredTime); // update validation code
        void AddSchedule(int englishId, int userId, DateTime startDate, DateTime endDate);
        void AddPrgoress(int englishId, int userId);
        void updateJumpForFunScore(int userId, int score);
        void updateCubeHubScore(int userId, int score);

        // get english
        English GetEnglishBySchedule(int scheduleId); // pass schedule id
        English GetEnglishByProgress(int progressId); // pass progress id
        IEnumerable<English> GetEnglishByType(int typeId); // pass type id

        // pass english id
        IEnumerable<Progress> GetProgressByEnglish(int userId, int englishId);
        Models.Type GetTypeByEnglish(int englishId);

        // api
        int GetUserId(string username);
        int UserLogin(string username, string password, string deviceId);
        int UserLoginWithValidationCode(string username, string password, string deviceId, string validationCode);
        int FinishProgress(int userId, int englishId, int grade);
        IEnumerable<Schedule> GetTodaySchdule(int userId); // pass user id
        IEnumerable<Progress> GetTodayProgress(int userId); // pass user id
        string GetEmailByUserId(int userId);

        // game status
        void StartGame();
        void EndGame();
        int GameStatus();
    }
}
