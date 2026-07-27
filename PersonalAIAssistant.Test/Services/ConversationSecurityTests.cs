using FluentAssertions;
using Moq;
using PersonalAIAssistant.Application.DTOs.Request;
using PersonalAIAssistant.Application.Interfaces.Infrastructure;
using PersonalAIAssistant.Application.Services;
using PersonalAIAssistant.Domain;
using PersonalAIAssistant.Domain.Entities;
using PersonalAIAssistant.Domain.Enums;
using Xunit;
using MockQueryable;
using MockQueryable.Moq;

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

        #region GetConversationAsync

        [Fact]
        public async Task GetConversationAsync_ShouldOnlyReturnCurrentUserConversations_DataLeakPrevention()
        {
            // Arrange
            int currentUserId = 1;

            _tokenServiceMock.Setup(x => x.GetUserId()).ReturnsAsync(currentUserId);

            // DB contains conversations for multiple users
            var conversations = new List<Conversation>
            {
                new Conversation { Id = 1, UserId = 1, Title = "My Chat 1", Status = ConversationStatus.Active, ModelsCharacter = CreateDummyCharacter() },
                new Conversation { Id = 2, UserId = 2, Title = "Other's Chat", Status = ConversationStatus.Active, ModelsCharacter = CreateDummyCharacter() },
                new Conversation { Id = 3, UserId = 1, Title = "My Chat 2", Status = ConversationStatus.Active, ModelsCharacter = CreateDummyCharacter() }
            };

            var mockQueryable = conversations.BuildMock();

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

        [Fact]
        public async Task GetConversationAsync_NoConversations_ReturnsEmptyList()
        {
            // Arrange
            _tokenServiceMock.Setup(x => x.GetUserId()).ReturnsAsync(1);
            var conversations = new List<Conversation>().BuildMock();
            _unitOfWorkMock.Setup(u => u.ConversationRepository.GetAll()).Returns(conversations);

            // Act
            var result = await _conversationService.GetConversationAsync();

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }

        #endregion

        #region CreateConversation

        [Fact]
        public async Task CreateConversation_ModelCharacterNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            _tokenServiceMock.Setup(x => x.GetUserId()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(u => u.ModelRepository.FindAsync(It.IsAny<object[]>()))
                           .ReturnsAsync((ModelsCharacter?)null);

            var request = new ConversationRequest { ModelCharacterId = 999 };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _conversationService.CreateConversation(request));
        }

        [Fact]
        public async Task CreateConversation_AlreadyExists_ReturnsExistingConversation()
        {
            // Arrange
            int userId = 1;
            int modelCharacterId = 5;

            _tokenServiceMock.Setup(x => x.GetUserId()).ReturnsAsync(userId);
            _unitOfWorkMock.Setup(u => u.ModelRepository.FindAsync(It.IsAny<object[]>()))
                           .ReturnsAsync(CreateDummyCharacter());

            var existingConversation = new Conversation
            {
                Id = 42,
                UserId = userId,
                ModelCharacterId = modelCharacterId,
                Title = "Existing Chat",
                Status = ConversationStatus.Active,
                ModelsCharacter = CreateDummyCharacter()
            };
            var conversations = new List<Conversation> { existingConversation }.BuildMock();

            _unitOfWorkMock.Setup(u => u.ConversationRepository.GetAll()).Returns(conversations);

            var request = new ConversationRequest { ModelCharacterId = modelCharacterId };

            // Act
            var result = await _conversationService.CreateConversation(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Conversation already exists!");
            result.Data!.Id.Should().Be(42);

            // Should never create a duplicate
            _unitOfWorkMock.Verify(u => u.ConversationRepository.AddAsync(It.IsAny<Conversation>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task CreateConversation_Success_CreatesNewConversation()
        {
            // Arrange
            int userId = 1;
            int modelCharacterId = 7;
            var character = CreateDummyCharacter();

            _tokenServiceMock.Setup(x => x.GetUserId()).ReturnsAsync(userId);
            _unitOfWorkMock.Setup(u => u.ModelRepository.FindAsync(It.IsAny<object[]>()))
                           .ReturnsAsync(character);

            var conversations = new List<Conversation>().BuildMock();
            _unitOfWorkMock.Setup(u => u.ConversationRepository.GetAll()).Returns(conversations);
            _unitOfWorkMock.Setup(u => u.ConversationRepository.AddAsync(It.IsAny<Conversation>()))
                           .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

            var request = new ConversationRequest { ModelCharacterId = modelCharacterId };

            // Act
            var result = await _conversationService.CreateConversation(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Created success!");
            result.Data!.Title.Should().Be(character.Name);

            _unitOfWorkMock.Verify(u => u.ConversationRepository.AddAsync(
                It.Is<Conversation>(c => c.UserId == userId && c.ModelCharacterId == modelCharacterId && c.Status == ConversationStatus.Active)),
                Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        #endregion

        #region DeleteConversation

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
                new Conversation { Id = targetConversationId, UserId = currentUserId, Title = "Private Chat", Status = ConversationStatus.Active }
            };

            var mockQueryable = conversations.BuildMock();

            // Mock the repository to return the conversations
            _unitOfWorkMock.Setup(u => u.ConversationRepository.GetAll())
                           .Returns(mockQueryable);

            // Act: Fake delete attempt by hacker (User 2) on User 1's conversation
            var result = await _conversationService.DeleteConversation(targetConversationId);

            // Assert
            // DeleteConversation silently no-ops (still returns Success = true) when the
            // conversation isn't found for this user. The important part: Remove() must
            // never be called for a conversation the caller doesn't own.
            result.Success.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.ConversationRepository.Remove(It.IsAny<Conversation>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task DeleteConversation_OwnConversation_RemovesAndSaves()
        {
            // Arrange
            int userId = 1;
            int conversationId = 10;

            _tokenServiceMock.Setup(x => x.GetUserId()).ReturnsAsync(userId);

            var conversation = new Conversation { Id = conversationId, UserId = userId, Title = "My Chat", Status = ConversationStatus.Active };
            var conversations = new List<Conversation> { conversation }.BuildMock();

            _unitOfWorkMock.Setup(u => u.ConversationRepository.GetAll()).Returns(conversations);
            _unitOfWorkMock.Setup(u => u.ConversationRepository.Remove(It.IsAny<Conversation>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _conversationService.DeleteConversation(conversationId);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Delete success!");
            _unitOfWorkMock.Verify(u => u.ConversationRepository.Remove(It.Is<Conversation>(c => c.Id == conversationId)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteConversation_UnexpectedException_ReturnsFailureResponse()
        {
            // Arrange: force an exception inside the try block
            _tokenServiceMock.Setup(x => x.GetUserId()).ThrowsAsync(new InvalidOperationException("token service unavailable"));

            // Act
            var result = await _conversationService.DeleteConversation(1);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("token service unavailable");
            _unitOfWorkMock.Verify(u => u.ConversationRepository.Remove(It.IsAny<Conversation>()), Times.Never);
        }

        #endregion

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