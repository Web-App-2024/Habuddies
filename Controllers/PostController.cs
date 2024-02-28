using HaBuddies.Models;
using HaBuddies.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaBuddies.Controllers
{
    public class PostController : Controller
    {
        private readonly PostService _postService;

        public PostController(PostService postService) =>
            _postService = postService;

        public async Task<IActionResult> Index(string category = null!, int page = 1, int perPage = 10)
        {
            var paginationResponse = await _postService.GetAllAsync(page, perPage, category);
            return View(paginationResponse);
        }

        public async Task<IActionResult> Details(string id)
        {
            var post = await _postService.GetOneAsync(id);

            if (post is null)
            {
                return NotFound("Post Not Found");
            }

            return View(post);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Post newPost)
        {
            await _postService.CreateAsync(newPost);

            return RedirectToAction(nameof(Details), new { id = newPost.id });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var post = await _postService.GetOneAsync(id);

            if (post is null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPut]
        public async Task<IActionResult> Edit(string id, [FromBody] Post updatedPost)
        {
            var post = await _postService.GetOneAsync(id);

            if (post is null)
            {
                return NotFound();
            }

            updatedPost.id = post.id;

            await _postService.UpdateAsync(id, updatedPost);

            return RedirectToAction(nameof(Details), new { id = updatedPost.id });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var post = await _postService.GetOneAsync(id);

            if (post is null)
            {
                return NotFound();
            }

            await _postService.RemoveAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
