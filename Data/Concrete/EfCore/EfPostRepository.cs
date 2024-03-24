using BlogAppProjesi.Data.Abstract;
using BlogAppProjesi.Data.Concrete.EfCore;
using BlogAppProjesi.Entity;

namespace BlogAppProjesi.Data.Concrete
{
    public class EfPostRepository : IPostReposistory
    {
        private BlogContext _context;
        public EfPostRepository(BlogContext context)
        {
            _context = context;
        }
        public IQueryable<Post> Posts => _context.Posts;

        public void CreatePost(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
        }
    }
}