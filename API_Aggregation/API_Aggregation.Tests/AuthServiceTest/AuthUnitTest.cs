using API_Aggregation.Controllers;
using API_Aggregation.Models;
using API_Aggregation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace API_Aggregation.Tests.AuthServiceTest
{
    public class AuthUnitTest
    {
        [Fact]
        public void Login_ShouldRejectInvalidCredentials()
        {
            // Arrange
            var settings = new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = "ThisIsATestSecretKeyThatIsLongEnough123!",
                ["JwtSettings:Issuer"] = "TestIssuer",
                ["JwtSettings:Audience"] = "TestAudience",
                ["TestUser:Username"] = "user",
                ["TestUser:Password"] = "pass"
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var authService = new AuthService(configuration);

            var controller = new AuthController(authService, configuration);

            var request = new LoginRequest
            {
                Username = "wronguser",
                Password = "wrongpassword"
            };

            // Act
            var result = controller.Login(request);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
