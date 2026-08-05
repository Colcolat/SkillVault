namespace Application.Interfaces;

public interface ICoachService
{
    Task<string> GetStudyTipsAsync(string courseTitle, string courseDescription);
}
