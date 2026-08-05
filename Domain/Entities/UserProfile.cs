using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class UserProfile
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime LastActiveDate { get; set; }
    
    public bool RemindersEnabled { get; set; } = true;
}
