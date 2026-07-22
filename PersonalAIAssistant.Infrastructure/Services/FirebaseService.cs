using FirebaseAdmin.Auth;
using PersonalAIAssistant.Application.Interfaces.Infrastructure;

namespace PersonalAIAssistant.Infrastructure.Services
{
    public class FirebaseService : IFirebaseService
    {
        public FirebaseService() { }
        public async Task<bool> CreatePhoneUserAsync(string phoneNumber)
        {
            var args = new UserRecordArgs
            {
                PhoneNumber = phoneNumber,
                Disabled = false
            };

            await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

            return true;
        }

        public async Task<bool> UpdatePhoneNumberAsync(string uid, string phoneNumber)
        {
            var args = new UserRecordArgs
            {
                Uid = uid,
                PhoneNumber = phoneNumber
            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
            return true;
        }

        public async Task<bool> DeleteUserAsync(string uid)
        {
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
            return true;
        }

        public async Task<bool> VerifyPhoneNumber(string uid, string phoneNumber)
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            if (userRecord.PhoneNumber == phoneNumber)
            {
                return true;
            }
            return false;
        }
    }
}
