using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using tik_talk.Data;
using tik_talk.Dtos;
using tik_talk.Interfaces;
using tik_talk.Models;

namespace tik_talk.Repositories;

public class PostRepository : IPostRepository
{
     private readonly ApplicationDBContext _context;

    public PostRepository(ApplicationDBContext context)
    {
        _context = context;
    }
  public async Task<Post?> CreateAsync(PostCreateDto post)
  {
    var indexOfWhitespace = post.content.IndexOf(" ");
    string? possibleTitle;
    if (indexOfWhitespace == -1){
      possibleTitle = post.content;
    }
    else{
      possibleTitle = post.content.Substring(0, indexOfWhitespace);
    }
    var postModel = new Post{
        title = possibleTitle,
        content = post.content,
        authorId = post.authorId,
        communityId = 0,
        createdAt = DateTime.Now,
        updatedAt = DateTime.Now,
        likes = 0
    };
    await _context.Posts.AddAsync(postModel);
    await _context.SaveChangesAsync();
    return postModel;
  }

  public async Task<Post?> CreateLike(int post_id)
  {
    var post = await _context.Posts.FirstOrDefaultAsync(p => p.id == post_id);
    if(post == null) return null;
    post.likes += 1;
    await _context.SaveChangesAsync();
    return post;
  }

  public async Task<Post?> DeleteAsync(int post_id)
  {
    var post = await _context.Posts.FirstOrDefaultAsync(p => p.id == post_id);
    if(post == null) return null;
    _context.Posts.Remove(post);
    await _context.SaveChangesAsync();
    return post;
  }

  public async Task<Post?> DeleteLike(int post_id)
  {
    var post = await _context.Posts.FirstOrDefaultAsync(p => p.id == post_id);
    if(post == null) return null;
    if(post.likes > 0) post.likes -= 1;
    await _context.SaveChangesAsync();
    return post;
  }

  public async Task<List<Post>> GetAllAsync()
  {
    var posts = await _context.Posts.ToListAsync();
    return posts;
  }

  public async Task<Post?> GetByIdAsync(int post_id)
  {
    var post = await _context.Posts.FirstOrDefaultAsync(p => p.id == post_id);
    return post;
  }

  public async Task<List<Post>?> GetByUserIdAsync(int user_id)
  {
    var posts = await _context.Posts.Where(p => p .authorId == user_id).ToListAsync();
    return posts;
  }

  public async Task<Post?> UpdateAsync(int post_id, PostUpdateDto postDto)
  {
    var post = await _context.Posts.FirstOrDefaultAsync(p => p.id == post_id);
    if(post == null) return null;
    if(postDto.content != null) post.content = postDto.content;
    if(postDto.title != null) post.title = postDto.title;
    post.updatedAt = DateTime.Now;
    await _context.SaveChangesAsync();
    return post;
  }


}
