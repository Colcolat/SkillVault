using Application.DTOs;
using Application.Ports.Input;
using Application.Ports.Output;

namespace Application.UseCases;

/// <summary>
/// Implements authentication use cases.
/// Decoupled from infrastructure using IJwtTokenGenerator output port.
/// </summary>
public class AuthUseCase : IAuthUseCase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly Application.Interfaces.IEmailService _emailService;

    public AuthUseCase(IJwtTokenGenerator jwtTokenGenerator, IUserProfileRepository userProfileRepository, Application.Interfaces.IEmailService emailService)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userProfileRepository = userProfileRepository;
        _emailService = emailService;
    }

    /// <summary>
    /// Processes user login. Compares with stored credentials and generates a JWT.
    /// </summary>
    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Email and password are required");

        var user = await _userProfileRepository.GetByEmailAsync(request.Email);
        
        // Very basic "hash" for demonstration (In production use BCrypt/Argon2)
        var hash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.Password));

        if (user == null || user.PasswordHash != hash)
        {
            // Allow default fallback for demonstration in case DB is fresh
            if (request.Email == "jj@skillvault.dev" && request.Password == "accenture2026" && user == null)
            {
                user = new Domain.Entities.UserProfile { Email = request.Email, PasswordHash = hash, LastActiveDate = DateTime.UtcNow };
                await _userProfileRepository.AddAsync(user);
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
        }
        else
        {
            user.LastActiveDate = DateTime.UtcNow;
            await _userProfileRepository.UpdateAsync(user);
        }

        var token = _jwtTokenGenerator.GenerateToken(request.Email);

        return new TokenResponse
        {
            AccessToken = token,
            ExpiresIn = 24 * 60 * 60, // 24 hours in seconds
            TokenType = "Bearer"
        };
    }

    /// <summary>
    /// Registers a new user dynamically.
    /// </summary>
    public async Task<bool> RegisterAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Email and password are required");

        var existingUser = await _userProfileRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            return false;

        var hash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.Password));
        var newUser = new Domain.Entities.UserProfile
        {
            Email = request.Email,
            PasswordHash = hash,
            LastActiveDate = DateTime.UtcNow
        };

        await _userProfileRepository.AddAsync(newUser);
        
        // Send Welcome Email
        _ = _emailService.SendEmailAsync(
            request.Email,
            "Welcome to SkillVault",
            $"Hello {request.Email},<br><br>Your SkillVault account has been created successfully. Start logging your study hours now!"
        );

        return true;
    }
}
