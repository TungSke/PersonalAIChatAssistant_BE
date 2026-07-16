using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using MockQueryable.Moq;
using PersonalAIAssistant.Application.Interfaces.Infrastructure;
using PersonalAIAssistant.Application.Services;
using PersonalAIAssistant.Domain;
using PersonalAIAssistant.Domain.Entities;
using Xunit;


namespace PersonalAIAssistant.Test.Services
{
    public class ConversationSecurityTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly ConversationService _conversationService;

        public ConversationSecurityTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _tokenServiceMock = new Mock<ITokenService>();
            _cacheServiceMock = new Mock<ICacheService>();

            _conversationService = new ConversationService(
                _unitOfWorkMock.Object,
                _tokenServiceMock.Object,
                _cacheServiceMock.Object);
        }

        [Fact]
        public async Task DeleteConversation_BelongingToAnotherUser_ShouldFail_IDOR_Prevention()
        {
            // Arrange
            int currentUserId = 1;
            int hackerUserId = 2;
            int targetConversationId = 100;

            // Mock token service to return hacker's user ID
            _tokenServiceMock.Setup(x => x.GetUserId()).ReturnsAsync(hackerUserId);

            // DB contains conversation of User 1
            var conversations = new List<Conversation>
            {
                new Conversation { Id = targetConversationId, UserId = currentUserId, Title = "Private Chat", Status = PersonalAIAssistant.Domain.Enums.ConversationStatus.Active }
            };

            var mockQueryable = conversations.AsQueryable();

            // Mock the repository to return the conversations
            _unitOfWorkMock.Setup(u => u.ConversationRepository.GetAll())
                           .Returns(mockQueryable);

            // Act: Fake delete attempt by hacker (User 2) on User 1's conversation
            var result = await _conversationService.DeleteConversation(targetConversationId);

            // Assert
            // The DeleteConversation method currently returns an ApiResponse which may have Success = true with Message "Delete success!" 
            // IF the conversation does not exist (see logic in ConversationService) it will not Remove but still report success.
            // The most important thing: Ensure Remove() is NEVER called.

            _unitOfWorkMock.Verify(u => u.ConversationRepository.Remove(It.IsAny<Conversation>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task GetConversationAsync_ShouldOnlyReturnCurrentUserConversations_DataLeakPrevention()
        {
            // Arrange
            int currentUserId = 1;

            _tokenServiceMock.Setup(x => x.GetUserId()).ReturnsAsync(currentUserId);

            // DB contains conversations for multiple users
            var conversations = new List<Conversation>
            {
                new Conversation { Id = 1, UserId = 1, Title = "My Chat 1", Status = PersonalAIAssistant.Domain.Enums.ConversationStatus.Active, ModelsCharacter = CreateDummyCharacter() },
                new Conversation { Id = 2, UserId = 2, Title = "Other's Chat", Status = PersonalAIAssistant.Domain.Enums.ConversationStatus.Active, ModelsCharacter = CreateDummyCharacter() },
                new Conversation { Id = 3, UserId = 1, Title = "My Chat 2", Status = PersonalAIAssistant.Domain.Enums.ConversationStatus.Active, ModelsCharacter = CreateDummyCharacter() }
            };

            var mockQueryable = conversations.AsQueryable();

            _unitOfWorkMock.Setup(u => u.ConversationRepository.GetAll())
                           .Returns(mockQueryable);

            // Act
            var result = await _conversationService.GetConversationAsync();

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2); // Can get only 2 conversations of current user
            result.Data.All(c => c.Title.Contains("My Chat")).Should().BeTrue();
            result.Data.Any(c => c.Title == "Other's Chat").Should().BeFalse(); // Do not return other user's conversation
        }

        private ModelsCharacter CreateDummyCharacter()
        {
            return new ModelsCharacter
            {
                Name = "Test",
                Backstory = "Test",
                Personality = "Test",
                AvatarUrl = "Test",
                SpeakingStyle = "Test",
                IntelligenceLevel = "Test",
                ResponseStyle = "Test",
                ExampleDialogue = "Test"
            };
        }
    }
}
