using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using BankAPI.Tools;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class LoginService
{
  private readonly BankContext _context;
  public LoginService(BankContext context)
  {
    _context = context;
  }

  public async Task<Administrator?> GetAdmin(AdminDto admin)
  {
    string sPassword = Encrypt.GetSha512Hash(admin.Pwd);
    return await _context.Administrators.SingleOrDefaultAsync(x => x.Email == admin.Email && x.Pwd == sPassword);
  }
}