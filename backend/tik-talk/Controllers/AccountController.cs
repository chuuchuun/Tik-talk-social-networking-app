using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using tik_talk.Data;
using tik_talk.Interfaces;
using tik_talk.Models;

namespace tik_talk.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ApplicationDBContext _context;
    private readonly IAccountRepository _accountRepo;
    public AccountController(ApplicationDBContext context, IAccountRepository accountRepository){
        _context = context;
        _accountRepo = accountRepository;

    }

    [HttpGet]
    public async Task<IActionResult> GetAll(){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        var accounts = await _accountRepo.GetAllAsync();
        //var stockDTO = accounts.Select(s => s.ToStockDto()).ToList();
        return Ok(accounts);
    }


    [HttpPost]
        public async Task<IActionResult> Create([FromBody] Account account){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            //var stockModel = stockDto.ToStockFromCreateRequestDto();
            await _accountRepo.CreateAsync(account);
            return Ok(account);
        }
    [HttpPatch]
    [Route("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Account accountDTO){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        var account = await _accountRepo.UpdateAsync(id, accountDTO);
        if(account == null){
            return NotFound();
        }
        return Ok(account);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        var account = await _accountRepo.DeleteAsync(id);
        return NoContent();
    }
}
