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


            config.NewConfig<List<Message>, MessageListResponse>()
                    .Map(dest => dest.FirstMessageId, src => src.FirstOrDefault().Id)
                    .Map(dest => dest.ModelId, src => src.FirstOrDefault(x => x.ModelCharacterId != null).ModelCharacterId)
                    .Map(dest => dest.ModelName, src => src.FirstOrDefault(x => x.ModelCharacterId != null).ModelsCharacter.Name)
                    .Map(dest => dest.ModelAvatarUrl, src => src.FirstOrDefault(x => x.ModelCharacterId != null).ModelsCharacter.AvatarUrl)
                    .Map(dest => dest.Messages, src => src.Adapt<List<MessageResponse>>());
        }
    }
}
