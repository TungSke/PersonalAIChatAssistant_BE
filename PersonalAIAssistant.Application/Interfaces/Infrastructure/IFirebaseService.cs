namespace PersonalAIAssistant.Application.Interfaces.Infrastructure
{
    public interface IFirebaseService
    {
        Task<bool> CreatePhoneUserAsync(string phoneNumber);

        Task<bool> UpdatePhoneNumberAsync(string uid, string phoneNumber);

        Task<bool> VerifyPhoneNumber(string uid, string phoneNumber);
    }
}
