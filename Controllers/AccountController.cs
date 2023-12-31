using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BankAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]

public class AccountController : ControllerBase
{
  private readonly AccountService accountService;
  private readonly AccountTypeService accountTypeService;
  private readonly ClientService clientService;

  public AccountController(AccountService accountService, AccountTypeService accountTypeService, ClientService clientService)
  {
    this.accountService = accountService;
    this.accountTypeService = accountTypeService;
    this.clientService = clientService;

  }

  [HttpGet("getAll")]
  public async Task<IEnumerable<AccountDtoOut>> Get()
  {
    return await accountService.GetAll();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<AccountDtoOut>> GetById(int id)
  {
    var account = await accountService.GetDtoById(id);

    if (account is null)
      return AccountNotFound(id);

    return account;
  }

  [Authorize(Policy = "SuperAdmin")]
  [HttpPost("create")]
  public async Task<IActionResult> Create(AccountDtoIn account)
  {
    string validateResult = await ValidateAccount(account);

    if (!validateResult.Equals("Valid"))
      return BadRequest(new { message = validateResult });

    var newAccount = await accountService.Create(account);

    return CreatedAtAction(nameof(GetById), new { id = newAccount.Id }, newAccount);
  }

  [Authorize(Policy = "SuperAdmin")]
  [HttpPut("update/{id}")]
  public async Task<IActionResult> Update(int id, AccountDtoIn account)
  {
    if (id != account.Id) return BadRequest(new { ErrorMessage = $"El ID {id} de la URL no coincide con el ID({account.Id}) del cuerpo de la solicitud" });

    var accountToUpdate = await accountService.GetById(id);

    if (accountToUpdate is not null)
    {
      string validateResult = await ValidateAccount(account);

      if (!validateResult.Equals("Valid"))
        return BadRequest(new { message = validateResult });

      await accountService.Update(id, account);
      return NoContent();
    }
    else
    {
      return AccountNotFound(id);
    }
  }

  [Authorize(Policy = "SuperAdmin")]
  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var accountToDelete = await accountService.GetById(id);
    if (accountToDelete is null) return AccountNotFound(id);

    await accountService.Delete(id);
    return Ok();
  }

  public NotFoundObjectResult AccountNotFound(int id)
  {
    return NotFound(new { ErrorMessage = $"El campo Account con ID = {id} no existe!" });
  }

  private async Task<string> ValidateAccount(AccountDtoIn account)
  {
    string result = "Valid";
    var accountType = await accountTypeService.GetById(account.AccountType);

    if (accountType is null) result = $"El tipo de cuenta {account.AccountType} no existe";

    var clientID = account.ClientId.GetValueOrDefault();

    var client = await clientService.GetById(clientID);

    if (client is null) result = $"El cliente {clientID} no existe.";

    return result;
  }
}