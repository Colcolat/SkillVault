using Application.Interfaces;
using Application.Ports.Output;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SkillVault.BackgroundServices;

public class InactivityReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InactivityReminderService> _logger;

    public InactivityReminderService(IServiceProvider serviceProvider, ILogger<InactivityReminderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("InactivityReminderService running check at: {time}", DateTimeOffset.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var inactiveThreshold = DateTime.UtcNow.AddDays(-7);

                var inactiveUsers = await userProfileRepository.GetInactiveUsersAsync(inactiveThreshold);

                foreach (var user in inactiveUsers)
                {
                    _logger.LogInformation("Sending reminder to {email}", user.Email);
                    await emailService.SendEmailAsync(
                        user.Email,
                        "Reminder: You have pending goals on SkillVault",
                        $"Hello {user.Email},<br><br>We've noticed you haven't logged any study hours in the last 7 days. Keep up the momentum!<br><br>Log in to SkillVault to continue your learning journey."
                    );

                    // Update LastActiveDate slightly to prevent spamming them every minute
                    user.LastActiveDate = DateTime.UtcNow; 
                    await userProfileRepository.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in InactivityReminderService.");
            }

            // Run check every 24 hours (use 1 hour for demo purposes if needed, here we use 24h)
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
