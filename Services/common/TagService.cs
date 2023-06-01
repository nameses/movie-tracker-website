using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;

namespace movie_tracker_website.Services.common
{
    public class TagService : ITagService
    {
        private readonly ILogger<TagService> _logger;
        private readonly Data.AuthDBContext _context;
        private readonly IConfiguration _config;

        public TagService(ILogger<TagService> logger,
                Data.AuthDBContext context,
                IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
        }

        public void AddTagsForUser(AppUser user, int movieId)
        {
            using (TMDbClient client = new TMDbClient(_config["APIKeys:TMDBAPI"]))
            {
                Movie movie = client.GetMovieAsync(movieId: movieId,
                    language: "en", includeImageLanguage: null, MovieMethods.Keywords).Result;

                //add tags
                foreach (var keyword in movie.Keywords.Keywords)
                {
                    //add tag model if not exists
                    if (!_context.Tags.Any(t => t.Name == keyword.Name))
                    {
                        _context.Tags.Add(new Models.Tag()
                        {
                            Name = keyword.Name,
                            ApiId = keyword.Id
                        });
                    }
                }
                _context.SaveChanges();
                //add AppUserTag model if not exists or increase valueImportance if exists
                foreach (var keyword in movie.Keywords.Keywords)
                {
                    //get Tag
                    var tag = _context.Tags.FirstOrDefault(t => t.Name == keyword.Name)
                        ?? throw new InvalidDataException();
                    //get AppUserTag
                    var userTag = _context.AppUserTags
                        .FirstOrDefault(e => e.UserId == user.Id && e.TagId == tag.Id);
                    if (userTag == null)
                    {
                        _context.AppUserTags.Add(new Models.AppUserTag()
                        {
                            UserId = user.Id,
                            TagId = tag.Id,
                            ValueImportance = 1
                        });
                    }
                    else
                    {
                        userTag.ValueImportance += 1;
                    }
                }
                _context.SaveChanges();
            }
        }

        public List<string?> GetImportantTags(AppUser user, int amount)
        {
            var list = _context.AppUserTags
                .Where(tag => tag.UserId == user.Id)
                .OrderByDescending(tag => tag.ValueImportance)
                .Take(amount)
                .ToList();
            var tagList = new List<string?>();
            foreach (var tag in list)
            {
                tagList.Add(_context.Tags.First(t => t.Id == tag.TagId).Name);
            }
            return tagList;
        }
    }
}