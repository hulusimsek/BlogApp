using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlogAppProjesi.Data.Concrete.EfCore;
using BlogAppProjesi.Entity;
using BlogAppProjesi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlogAppProjesi.Controllers
{
    public class PostController : Controller
    {
        private readonly BlogContext _context;
        public PostController(BlogContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string url)
        {
            var claims = User.Claims;
            var posts =  _context.Posts.AsQueryable();
            if(!string.IsNullOrEmpty(url)) {
                posts = posts.Where( x => x.Tags.Any(t => t.Url == url));
            }
            return View(
                new PostsViewModel {
                    Posts = await posts.ToListAsync(),
                    Tags = await _context.Tags.ToListAsync()
                }
                );
        }

        public async Task<IActionResult> Details(string url)
        {
            return View(
                await _context
                        .Posts
                        .Include(x=>x.Tags)
                        .Include(x=>x.Comments)
                        .ThenInclude(x=>x.User)
                        .FirstOrDefaultAsync(p => p.Url == url)
                );
        }

        [HttpPost]
        public async Task<JsonResult> AddComment(int PostId, string Text) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var avatar = User.FindFirstValue(ClaimTypes.UserData);
            var entity = new Comment {
                Text = Text,
                PublishedOn = DateTime.Now,
                PostId = PostId,
                UserId = int.Parse(userId ?? "")
            };
            _context.Comments.Add(entity);
             await _context.SaveChangesAsync();

            return Json(new {
                username,
                Text,
                entity.PublishedOn,

            });
        }
    }
}