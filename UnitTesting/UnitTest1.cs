using GoKartUnite.Data;
using GoKartUnite.DataFilterOptions;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace UnitTesting
{
    public class BlogHandlerTests
    {
        private readonly GoKartUniteContext _context;
        private readonly BlogHandler _blogHandler;

        public BlogHandlerTests()
        {
            // Set up the in-memory database for testing
            var options = new DbContextOptionsBuilder<GoKartUniteContext>()
                          .UseInMemoryDatabase(databaseName: "GoKartUniteTestDb")
                          .Options;

            _context = new GoKartUniteContext(options); // Using the in-memory database
            _blogHandler = new BlogHandler(_context);
        }

        // Setup Methods =================================
        public void CreateDatabase_WithXNumberOfRows(int x)
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var trackNameExample = "Example Track";
            var location = Locations.EAST;
            var blogPosts = Enumerable.Range(1, x)
                .Select(i => new BlogPost
                {
                    Id = i,
                    DateTimePosted = DateTime.Now.AddDays(-i),
                    TaggedTrack = new Track
                    {
                        Title = trackNameExample + "i",
                        Location = location
                    }
                })
                .ToList().AsQueryable();

            _context.BlogPosts.AddRange(blogPosts);
            _context.SaveChanges();
        }

        // Setup Methods =================================

        [Fact]
        public async Task GetAllPosts_WithTrackNameFilter_ReturnsFilteredPosts()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            // Arrange
            var trackNameFilter = "Track A";
            var filterOptions = new BlogFilterOptions
            {
                TrackNameFilter = trackNameFilter,
                IncludeTrack = false,
                IncludeUpvotes = false,
                PageNo = 1,
                PageSize = 10,
                SortByAscending = true
            };

            var blogPosts = new List<BlogPost>
            {
                new BlogPost { Id = 1, DateTimePosted = DateTime.Now.AddDays(-1), TaggedTrack = new Track { Title = trackNameFilter, Location = Locations.EAST }},
                new BlogPost { Id = 2, DateTimePosted = DateTime.Now.AddDays(-2), TaggedTrack = new Track { Title = "Track B", Location = Locations.EAST }},
            }.AsQueryable();

            _context.BlogPosts.AddRange(blogPosts);
            await _context.SaveChangesAsync();

            // Act
            var result = await _blogHandler.GetAllPosts(filterOptions);

            // Assert
            Assert.Single(result);
            Assert.Equal(trackNameFilter, result[0].TaggedTrack.Title);
        }

        [Fact]
        public async Task GetAllPosts_WithTenPerPage_SortByDescending()
        {
            CreateDatabase_WithXNumberOfRows(15);

            var filterOptions = new BlogFilterOptions
            {
                IncludeTrack = false,
                IncludeUpvotes = false,
                PageNo = 1,
                PageSize = 10,
                SortByAscending = false
            };

            // act
            var result = await _blogHandler.GetAllPosts(filterOptions);

            // Assert
            var minDate = DateTime.Now.AddDays(-10);
            var maxDate = DateTime.Now;

            Assert.True(result.Count == 10);
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].DateTimePosted > result[i + 1].DateTimePosted,
                    $"returned orderer of blog posts in incorrect order : {result[i].DateTimePosted} before {result[i + 1].DateTimePosted}");
                Assert.True(result[i].DateTimePosted >= minDate
                    && result[i].DateTimePosted <= maxDate,
                    $"{result[i].DateTimePosted.Date} not in range {minDate} to {maxDate}");
            }
        }

        [Fact]
        public async Task GetAllPosts_WithTenPerPage_SortByAscending()
        {
            int numRows = 15;
            CreateDatabase_WithXNumberOfRows(numRows);

            var filterOptions = new BlogFilterOptions
            {
                IncludeTrack = false,
                IncludeUpvotes = false,
                PageNo = 1,
                PageSize = 10,
                SortByAscending = true
            };

            // act
            var result = await _blogHandler.GetAllPosts(filterOptions);


            var minDate = DateTime.Now.AddDays(-numRows - 1);
            var maxDate = DateTime.Now.AddDays(-numRows + 9);
            // Assert
            Assert.True(result.Count == 10);
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].DateTimePosted < result[i + 1].DateTimePosted,
                    $"returned orderer of blog posts in incorrect order : {result[i + 1].DateTimePosted} after {result[i].DateTimePosted}"); ;
                Assert.True(result[i].DateTimePosted >= minDate
                    && result[i].DateTimePosted <= maxDate,
                    $"Dat e {result[i].DateTimePosted.Date} not within range {minDate.Date} to {maxDate.Date}.");
            }
        }



    }
}

