using Microsoft.EntityFrameworkCore;
using Tweetbook.Data;
using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContext;

        public PostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Post>> GetAllAsync()
        {
            return await _dataContext.Posts.Include(x => x.Tags).ToListAsync();
        }

        public async Task<Post> GetAsync(Guid Id)
        {
            return await _dataContext.Posts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Post> CreateAsync(Post post)
        {
            var res = await _dataContext.Posts.AddAsync(post);

            await _dataContext.SaveChangesAsync();

            return res.Entity;
        }

        public async Task<bool> UpdateAsync(Post post)
        {
            var exists = _dataContext.Posts.Any(x => x.Id == post.Id);

            if (!exists)
            {
                return false;
            }

            _dataContext.Posts.Update(post);

            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid Id)
        {
            var post = await _dataContext.Posts.FindAsync(Id);

            if(post == null)
            {
                return false;
            }

            _dataContext.Posts.Remove(post);

            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UserOwnsPostAsync(Guid id, string userId)
        {
            var post = await _dataContext.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if(post == null)
            {
                return false;
            }

            return post.UserId == userId;
        }
    }
}