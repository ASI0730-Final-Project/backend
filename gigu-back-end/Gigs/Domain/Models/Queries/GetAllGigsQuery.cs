namespace Gigs.Domain.Models.Queries
{
    public class GetAllGigsQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = string.Empty;

        public GetAllGigsQuery(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            Page = page;
            PageSize = pageSize;
            SearchTerm = searchTerm;
        }
    }
}