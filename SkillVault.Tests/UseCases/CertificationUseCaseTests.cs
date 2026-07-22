using Application.DTOs;
using Application.Ports.Output;
using Application.UseCases;
using Domain.Entities;
using Moq;

namespace SkillVault.Tests.UseCases;

public class CertificationUseCaseTests
{
    private readonly Mock<ICertificationRepository> _repositoryMock;
    private readonly CertificationUseCase _sut;

    public CertificationUseCaseTests()
    {
        _repositoryMock = new Mock<ICertificationRepository>();
        _sut = new CertificationUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task RegisterCertificationAsync_ValidRequest_ReturnsDto()
    {
        // Arrange
        var request = new CreateCertificationRequest
        {
            Title = "AWS Certified Developer",
            Provider = "AWS",
            CompletedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CredentialUrl = "https://aws.amazon.com"
        };

        var expectedEntity = new Certification
        {
            Id = 1,
            Title = request.Title,
            Provider = request.Provider,
            CompletedDate = request.CompletedDate,
            CredentialUrl = request.CredentialUrl
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Certification>()))
            .ReturnsAsync(expectedEntity);

        // Act
        var result = await _sut.RegisterCertificationAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Provider, result.Provider);
        
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Certification>(c => 
            c.Title == request.Title && 
            c.Provider == request.Provider)), Times.Once);
    }

    [Fact]
    public async Task GetCertificationByIdAsync_ExistingId_ReturnsDto()
    {
        // Arrange
        var certificationId = 1;
        var entity = new Certification
        {
            Id = certificationId,
            Title = "Test Cert",
            Provider = "Test Provider",
            CompletedDate = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(certificationId))
            .ReturnsAsync(entity);

        // Act
        var result = await _sut.GetCertificationByIdAsync(certificationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(certificationId, result.Id);
        Assert.Equal("Test Cert", result.Title);
    }

    [Fact]
    public async Task GetCertificationByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var certificationId = 999;
        
        _repositoryMock
            .Setup(r => r.GetByIdAsync(certificationId))
            .ReturnsAsync((Certification)null);

        // Act
        var result = await _sut.GetCertificationByIdAsync(certificationId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCertificationAsync_ExistingIdAndValidRequest_UpdatesAndReturnsDto()
    {
        // Arrange
        var certificationId = 1;
        var existingEntity = new Certification
        {
            Id = certificationId,
            Title = "Old Title",
            Provider = "Old Provider",
            CompletedDate = DateTime.UtcNow
        };

        var request = new UpdateCertificationRequest
        {
            Title = "New Title",
            CredentialUrl = "https://new-url.com"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(certificationId))
            .ReturnsAsync(existingEntity);

        _repositoryMock
            .Setup(r => r.UpdateAsync(certificationId, It.IsAny<Certification>()))
            .ReturnsAsync((int id, Certification c) => c); // Return the updated instance

        // Act
        var result = await _sut.UpdateCertificationAsync(certificationId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Title", result.Title);
        Assert.Equal("Old Provider", result.Provider); // Untouched
        Assert.Equal("https://new-url.com", result.CredentialUrl);
        
        _repositoryMock.Verify(r => r.UpdateAsync(certificationId, It.Is<Certification>(c => 
            c.Title == "New Title" && c.CredentialUrl == "https://new-url.com")), Times.Once);
    }
}
