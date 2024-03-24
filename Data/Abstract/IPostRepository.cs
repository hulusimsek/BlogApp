using BlogAppProjesi.Entity;

namespace BlogAppProjesi.Data.Abstract 
{
    public interface IPostReposistory 
    {
        IQueryable<Post> Posts {get;} // filrelenmiş şekilde almak için IQueryable yazdık
        //innumerable yazsaydık bütün verileri çekip filtre yapardık
        void CreatePost(Post post);
    }
}