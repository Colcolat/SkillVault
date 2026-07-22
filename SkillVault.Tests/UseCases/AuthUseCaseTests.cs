using Application.DTOs;
using Application.Ports.Output;
using Application.UseCases;
using Moq;

namespace SkillVault.Tests.UseCases;

public class AuthUseCaseTests
{
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly AuthUseCase _sut;

    public AuthUseCaseTests()
    {
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _sut = new AuthUseCase(_jwtTokenGeneratorMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTokenResponse()
    {
        // Arrange
        var request = new LoginRequest { Email = "jj@skillvault.dev", Password = "accenture2026" };
        var expectedToken = "fake-jwt-token";
        
        _jwtTokenGeneratorMock
            .Setup(g => g.GenerateToken(request.Email))
            .Returns(expectedToken);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedToken, result.AccessToken);
        Assert.Equal("Bearer", result.TokenType);
    }

    [Theory]
    [InlineData("", "password123")]
    [InlineData("test@test.com", "")]
    [InlineData(null, "password123")]
    [InlineData("test@test.com", null)]
    public async Task LoginAsync_EmptyCredentials_ThrowsArgumentException(string? email, string? password)
    {
        // Arrange
        var request = new LoginRequest { Email = email, Password = password };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _sut.LoginAsync(request));
        Assert.Equal("Email and password are required", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest { Email = "wrong@skillvault.dev", Password = "wrongpassword" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync(request));
        Assert.Equal("Invalid email or password", exception.Message);
    }
}
