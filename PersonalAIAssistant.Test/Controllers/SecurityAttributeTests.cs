using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalAIAssistant.API.Controllers;
using System.Linq;
using System.Reflection;
using Xunit;

namespace PersonalAIAssistant.Test.Controllers
{
    public class SecurityAttributeTests
    {
        [Theory]
        [InlineData(typeof(ConversationController))]
        [InlineData(typeof(MessageController))]
        [InlineData(typeof(ModelsCharacterController))]
        // [InlineData(typeof(CharacterEmotionController))] // Thêm vào nếu có
        public void SensitiveControllers_ShouldHaveAuthorizeAttribute(System.Type controllerType)
        {
            // Act
            var hasAuthorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Any();

            // Assert
            hasAuthorizeAttribute.Should().BeTrue($"Bảo mật: Controller {controllerType.Name} thao tác với dữ liệu nhạy cảm nên bắt buộc phải có [Authorize] ở cấp độ class.");
        }

        [Fact]
        public void UsersController_MeEndpoint_ShouldHaveAuthorizeAttribute()
        {
            // Act
            var meMethod = typeof(UsersController).GetMethod("Me");
            var hasAuthorizeAttribute = meMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Any();

            // Assert
            hasAuthorizeAttribute.Should().BeTrue("Bảo mật: API Get /me của UsersController bắt buộc phải yêu cầu xác thực (cần có [Authorize]).");
        }
    }
}
