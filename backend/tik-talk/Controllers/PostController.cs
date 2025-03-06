using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using tik_talk.Data;
using tik_talk.Dtos;
using tik_talk.Interfaces;

namespace tik_talk.Controllers;

[Route("api/post")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly ApplicationDBContext _context;
    private readonly IAccountRepository _accountRepo;
    private readonly ITokenService _tokenservice;
    private readonly IPostRepository _postRepo;
    public PostController(ApplicationDBContext context, IAccountRepository accountRepository, ITokenService tokenService, IPostRepository postRepo){
        _context = context;
        _accountRepo = accountRepository;
        _tokenservice = tokenService;
        _postRepo = postRepo;
    }
   
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostCreateDto post){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
            
        await _postRepo.CreateAsync(post);
        return Ok(post);
    }

    [HttpGet]
    public async Task<IActionResult> GetPostsByUserId([FromQuery] int user_id){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        var posts = await _postRepo.GetByUserIdAsync(user_id);
        if(posts == null){
            return NotFound();
        }
        return Ok(posts);
    }

    [HttpGet("{post_id}")]
    public async Task<IActionResult> GetPost([FromRoute] int post_id){
        var post = await _postRepo.GetByIdAsync(post_id);
        if(post == null) return NotFound();
        return Ok(post);
    }

    [HttpPatch("{post_id}")]
    public async Task<IActionResult> UpdatePost([FromRoute] int post_id, [FromBody] PostUpdateDto postDto){
        var post = await  _postRepo.UpdateAsync(post_id, postDto);
        if(post == null) return BadRequest();
        return Ok(post);
    }

    [HttpDelete]
    [Route("{post_id}")]
    public async Task<IActionResult> DeletePost([FromRoute] int post_id){
        var post = await  _postRepo.DeleteAsync(post_id);
        if(post == null) return BadRequest();
        return NoContent();
    }


   [HttpPost("like/{post_id}")]
    public async Task<IActionResult> CreateLike([FromRoute] int post_id)

    {
        var post = await _postRepo.CreateLike(post_id);
        if (post == null) return BadRequest("Failed to like the post.");
        return Ok(post);
    }

    [HttpDelete("like/{post_id}")]
    public async Task<IActionResult> DeleteLike([FromRoute] int post_id)

    {
        var post = await _postRepo.DeleteLike(post_id);
        if (post == null) return BadRequest("Failed to dislike the post.");
        return Ok(post);
    }

}
