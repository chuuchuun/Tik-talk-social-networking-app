using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tik_talk.Dtos;
using tik_talk.Models;

namespace tik_talk.Interfaces;

public interface IPostRepository
{
    Task<List<Post>> GetAllAsync();
    Task<Post?> CreateAsync(PostCreateDto post);
    Task<List<Post>?> GetByUserIdAsync(int user_id);
    Task<Post?> GetByIdAsync(int post_id);
    Task<Post?> UpdateAsync(int post_id, PostUpdateDto post);
    Task<Post?> DeleteAsync(int post_id);
    Task<Post?> CreateLike(int post_id);
    Task<Post?> DeleteLike(int post_id);
}
