using System.ComponentModel.DataAnnotations;

namespace BankAPI.Data.DTOs;

public class AdminDto
{

  [Required]
  public string Email { get; set; } = null!;
  
  [Required]
  public string Pwd { get; set; } = null!;
}