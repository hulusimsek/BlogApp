using BlogAppProjesi.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlogAppProjesi.ViewComponents
{
    public class NewPosts : ViewComponent
    {
        private readonly BlogContext _context;
        public NewPosts(BlogContext context) {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _context.Posts.OrderByDescending(p=>p.PublishedOn).Take(5).ToListAsync());
        }
    }
}