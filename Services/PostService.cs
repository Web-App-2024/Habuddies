using HaBuddies.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HaBuddies.Services;

public class PostService
{
    private MongoService _mongoSevice;
    private IMongoCollection<Post> _postsCollection;

    public PostService(
        MongoService mongoService)
    {
        _mongoSevice = mongoService;
        _postsCollection = _mongoSevice._postsCollection;
    }

    public async Task<PaginationResponse<Post>> GetAllAsync(int page, int perPage, string category) {
            var paginationParams = Pagination.BuildPaginationLimit(page, perPage, 32);

            FilterDefinition<Post> filter;
            if (!string.IsNullOrEmpty(category))
            {
                filter = Builders<Post>.Filter.Eq(post => post.category, category);
            }
            else
            {
                filter = Builders<Post>.Filter.Empty;
            } 
            
            var data = await _postsCollection.Find(filter)
                                        .Skip(paginationParams.Skip)
                                        .Limit(paginationParams.PerPage)
                                        .ToListAsync();
            var totalCount = await _postsCollection.CountDocumentsAsync(filter);

            var paginationResponse = Pagination.BuildResponsePagination(data, page, perPage, (int)totalCount);

            return paginationResponse;
        }

    public async Task<Post?> GetOneAsync(string id) =>
        await _postsCollection.Find(x => x.id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Post newPost) =>
        await _postsCollection.InsertOneAsync(newPost);

    public async Task UpdateAsync(string id, Post updatedPost) =>
        await _postsCollection.ReplaceOneAsync(x => x.id == id, updatedPost);

    public async Task RemoveAsync(string id) =>
        await _postsCollection.DeleteOneAsync(x => x.id == id);
}