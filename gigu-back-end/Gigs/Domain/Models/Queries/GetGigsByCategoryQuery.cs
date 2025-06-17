namespace Gigs.Domain.Models.Queries
{
    public class GetGigsByCategoryQuery
    {
        public string Category { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public GetGigsByCategoryQuery(string category, int page = 1, int pageSize = 10)
        {
            Category = category;
            Page = page;
            PageSize = pageSize;
        }
    }
}