using Application.DTOs;
using Application.Ports.Output;
using Application.UseCases;
using Domain.Entities;
using Moq;

namespace SkillVault.Tests.UseCases;

public class ProgressUseCaseTests
{
    private readonly Mock<IProgressRepository> _progressRepositoryMock;
    private readonly Mock<ICertificationRepository> _certificationRepositoryMock;
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly ProgressUseCase _sut;

    public ProgressUseCaseTests()
    {
        _progressRepositoryMock = new Mock<IProgressRepository>();
        _certificationRepositoryMock = new Mock<ICertificationRepository>();
        _courseRepositoryMock = new Mock<ICourseRepository>();
        
        _sut = new ProgressUseCase(
            _progressRepositoryMock.Object,
            _certificationRepositoryMock.Object,
            _courseRepositoryMock.Object);
    }

    [Fact]
    public async Task RegisterProgressAsync_ValidRequestWithCertification_ReturnsDto()
    {
        // Arrange
        var request = new CreateProgressRequest
        {
            CertificationId = 1,
            Hours = 2.5M,
            Notes = "Studied AWS"
        };

        var certification = new Certification { Id = 1, Title = "AWS" };
        var expectedEntity = new Progress
        {
            Id = 1,
            CertificationId = 1,
            Hours = request.Hours,
            Notes = request.Notes
        };

        _certificationRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(certification);

        _progressRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Progress>()))
            .ReturnsAsync(expectedEntity);

        // Act
        var result = await _sut.RegisterProgressAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(request.Hours, result.Hours);
        Assert.Equal(request.CertificationId, result.CertificationId);
        
        _progressRepositoryMock.Verify(r => r.AddAsync(It.Is<Progress>(p => 
            p.Hours == request.Hours && p.CertificationId == request.CertificationId)), Times.Once);
    }

    [Fact]
    public async Task RegisterProgressAsync_CertificationNotFound_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateProgressRequest { CertificationId = 999, Hours = 1 };

        _certificationRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Certification)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _sut.RegisterProgressAsync(request));
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task RegisterProgressAsync_NoTargetIdProvided_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateProgressRequest { Hours = 1 }; // No CertificationId, CourseId, or SkillId

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _sut.RegisterProgressAsync(request));
        Assert.Contains("must be linked to at least a Certification, Course, or Skill", exception.Message);
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(0.0)]
    [InlineData(25.0)]
    public async Task RegisterProgressAsync_InvalidHours_ThrowsExceptionFromDomainValidation(double invalidHours)
    {
        // Arrange
        var request = new CreateProgressRequest
        {
            SkillId = 1,
            Hours = (decimal)invalidHours
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _sut.RegisterProgressAsync(request));
        Assert.Contains("Hours", exception.Message);
    }
}
