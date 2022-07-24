using Microsoft.EntityFrameworkCore;
using Tweetbook.Data;
using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public class PostTagService : IPostTagService
    {
        private readonly DataContext _dataContext;

        public PostTagService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            return await _dataContext.Tags.ToListAsync();
        }

        public async Task<Tag> GetAsync(Guid Id)
        {
            return await _dataContext.Tags.FindAsync(Id);
        }

        public async Task<Tag> GetAsync(string name)
        {
            return await _dataContext.Tags.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<Tag> CreateAsync(Tag post)
        {
            var res = await _dataContext.Tags.AddAsync(post);

            await _dataContext.SaveChangesAsync();

            return res.Entity;
        }
    }
}
