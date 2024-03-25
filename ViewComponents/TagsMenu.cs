using BlogAppProjesi.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlogAppProjesi.ViewComponents
{
    public class TagsMenu : ViewComponent
    {
        private readonly BlogContext _context;
        public TagsMenu(BlogContext context) {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("Default", await _context.Tags.ToListAsync());
        }
    }
}