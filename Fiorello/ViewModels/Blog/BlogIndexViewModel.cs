namespace Fiorello.ViewModels.Blog
{
    public class BlogIndexViewModel
    {
        public List<Models.Blog> Blogs { get; set; }

        public int Page { get; set; } = 1;

        public int PageCount { get; set; }

        public int Take { get; set; } = 3;
    }
}
