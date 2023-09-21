using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Repositories;
public class PostRepository : IPostRepository
{
    private readonly DatabaseContextFactory _databaseContextFactory;

    public PostRepository(DatabaseContextFactory databaseContextFactory)
    {
        _databaseContextFactory = databaseContextFactory;
    }

    public async Task CreateAsync(PostEntity post)
    {
        //await using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        //await context.Posts.AddAsync(post);
        using (DatabaseContext context = _databaseContextFactory.CreateDbContext())
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
        };
        //await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        var post = await GetByIdAsync(postId);

        if (post == null)
            return;

        context.Posts.Remove(post);
        await context.SaveChangesAsync();
    }

    public async Task<PostEntity> GetByIdAsync(Guid postId)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        return await context.Posts.Include(x => x.Comments).FirstOrDefaultAsync(p => p.Id == postId);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Include(x => x.Comments)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Include(x => x.Comments)
            .AsNoTracking()
            .Where(p => p.Author.Contains(author))
            .ToListAsync();    
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync()
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Include(x => x.Comments)
            .AsNoTracking()
            .Where(p => p.Comments != null && p.Comments.Any())
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Include(x => x.Comments)
            .AsNoTracking()
            .Where(p => p.Likes >= numberOfLikes)
            .ToListAsync();
    }

    public async Task UpdateAsync(PostEntity post)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        context.Posts.Update(post);
        await context.SaveChangesAsync();
    }
}
