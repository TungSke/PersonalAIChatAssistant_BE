using Mapster;
using WaifuAIAssistant.Application.DTOs.Response;
using WaifuAIAssistant.Domain.Entities;

namespace WaifuAIAssistant.API
{
    public class MapObject : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Message, MessageResponse>()
                  .Map(dest => dest.role, src => src.UserId == null ? "AI" : "User");
        }
    }
}
