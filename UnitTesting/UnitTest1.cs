﻿using GoKartUnite.Data;
using GoKartUnite.DataFilterOptions;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace UnitTesting
{
    public class BlogHandlerTests
    {
        private readonly GoKartUniteContext _context;
        private readonly BlogHandler _blogHandler;
        private readonly KarterHandler _karterHandler;

        public BlogHandlerTests()
        {

            var options = new DbContextOptionsBuilder<GoKartUniteContext>()
                          .UseInMemoryDatabase(databaseName: "uniqueDbName")
                          .Options;

            _context = new GoKartUniteContext(options);
            _karterHandler = new KarterHandler(_context);
            _blogHandler = new BlogHandler(_context, _karterHandler);
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
                    DateTimePosted = DateTime.Now.AddDays(-i),
                    TaggedTrack = new Track
                    {
                        Title = trackNameExample + "i",
                        Location = location
                    }
                })
                .ToList();

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
            };

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
            Assert.True(result.Count == Math.Min(numRows, 10));
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].DateTimePosted < result[i + 1].DateTimePosted,
                    $"returned orderer of blog posts in incorrect order : {result[i + 1].DateTimePosted} after {result[i].DateTimePosted}"); ;
                Assert.True(result[i].DateTimePosted >= minDate
                    && result[i].DateTimePosted <= maxDate,
                    $"Dat e {result[i].DateTimePosted.Date} not within range {minDate.Date} to {maxDate.Date}.");
            }
        }

        [Fact]
        public async Task GetAllPosts_WithTenPerPage_OnlyAFewPostsCreated()
        {
            int numRows = 5;
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
            var maxDate = DateTime.Now.AddDays(-numRows + numRows - 1);
            // Assert
            Assert.True(result.Count == numRows);
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].DateTimePosted < result[i + 1].DateTimePosted,
                    $"returned orderer of blog posts in incorrect order : {result[i + 1].DateTimePosted} after {result[i].DateTimePosted}"); ;
                Assert.True(result[i].DateTimePosted >= minDate
                    && result[i].DateTimePosted <= maxDate,
                    $"Dat e {result[i].DateTimePosted.Date} not within range {minDate.Date} to {maxDate.Date}.");
            }
        }

        [Fact]
        public async Task GetAllPosts_WithTenPerPage_TrySecondPageWithOnlyTenCreated()
        {

            for (int j = 8; j < 15; j++)
            {
                int numRows = j;
                _context.ChangeTracker.Clear();
                CreateDatabase_WithXNumberOfRows(numRows);
                var filterOptions = new BlogFilterOptions
                {
                    IncludeTrack = false,
                    IncludeUpvotes = false,
                    PageNo = 2,
                    PageSize = 10,
                    SortByAscending = true
                };

                // act
                var result = await _blogHandler.GetAllPosts(filterOptions);


                var minDate = DateTime.Now.AddDays(-numRows - 1);
                var maxDate = DateTime.Now.AddDays(-numRows + numRows - 1);
                // Assert
                Assert.True(result.Count == (Math.Max(numRows - filterOptions.PageSize, 0)));
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

        [Fact]
        public async Task GetAllPosts_WithTenPerPage_TrySecondPageWithOnlyTenCreated_DifferingPageSize()
        {
            for (int j = 5; j < 8; j++)
            {
                int numRows = j;
                _context.ChangeTracker.Clear();
                CreateDatabase_WithXNumberOfRows(numRows);
                var filterOptions = new BlogFilterOptions
                {
                    IncludeTrack = false,
                    IncludeUpvotes = false,
                    PageNo = 2,
                    PageSize = 6,
                    SortByAscending = true
                };

                // act
                var result = await _blogHandler.GetAllPosts(filterOptions);


                var minDate = DateTime.Now.AddDays(-numRows - 1);
                var maxDate = DateTime.Now.AddDays(-numRows + numRows - 1);
                // Assert
                Assert.True(result.Count == (Math.Max(numRows - filterOptions.PageSize, 0)));
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

        [Fact]
        public async Task AddPost_WithTaggedTrack()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();


            string tempKarterName = "John Doe";
            string tempTrackName = "Track Title";

            var karter = new Karter
            {
                Name = tempKarterName,
                YearsExperience = 10
            };

            var track = new Track
            {
                Title = tempTrackName,
                Location = Locations.NORTH,
                Description = "Description",
            };

            _context.Karter.Add(karter);
            _context.Track.Add(track);
            _context.SaveChanges();
            karter = _context.Karter.Single(x => x.Name == tempKarterName);

            var trackNameExample = tempTrackName;
            var location = Locations.EAST;
            var blogPostsView = new BlogPostView
            {
                Title = "My First Blog Post",
                Description = "This is a sample blog post description.",
                Author = karter,
                authorId = 123,
                Upvotes = 10,
                TaggedTrackTitle = trackNameExample,
                TaggedTrack = track,
                blogType = BlogType.Post,
                ReleaseDateTime = DateTime.Now.AddDays(1)
            };

            int result = await _blogHandler.AddPost(blogPostsView);
            Assert.True(result > -1);
            Assert.True(_context.BlogPosts.Count() == 1, "More than 1 Record Created");
            BlogPost post = _context.BlogPosts.Include(x => x.Karter).Include(x => x.TaggedTrack).Single(x => x.Id >= -1);
            Assert.True(result == post.Id);
            Assert.True(post.Title == blogPostsView.Title, "Post Title and PostsViewTitle Mismatch");
            Assert.True(post.Karter == karter, "Karter created and set as author mismatch");
            Assert.True(post.TaggedTrack == track, "TaggedTrack created and set as TaggedTrack mismatch");
        }

        [Fact]
        public async Task AddPost_NoTaggedTrack()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();


            string tempKarterName = "John Doe";

            var karter = new Karter
            {
                Name = tempKarterName,
                YearsExperience = 10
            };

            _context.Karter.Add(karter);
            _context.SaveChanges();

            karter = _context.Karter.Single(x => x.Name == tempKarterName);

            var location = Locations.EAST;
            var blogPostsView = new BlogPostView
            {
                Title = "My First Blog Post",
                Description = "This is a sample blog post description.",
                Author = karter,
                authorId = karter.Id,
                Upvotes = 10,
                blogType = BlogType.Post,
                ReleaseDateTime = DateTime.Now.AddDays(1)
            };

            int result = await _blogHandler.AddPost(blogPostsView);

            Assert.True(result > -1);
            Assert.True(_context.BlogPosts.Count() == 1, "More than 1 Record Created");

            BlogPost post = _context.BlogPosts.Include(x => x.Karter).Single(x => x.Id >= -1);

            Assert.True(result == post.Id);
            Assert.True(post.Title == blogPostsView.Title, "Title added and created mismatch");
            Assert.True(post.Karter == karter, "Karter added and created mismatch");
        }

        [Fact]
        public async Task AddPost_IncompletePostView()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();


            string tempKarterName = "John Doe";

            var karter = new Karter
            {
                Name = tempKarterName,
                YearsExperience = 10
            };

            _context.Karter.Add(karter);
            _context.SaveChanges();

            karter.Id = -1;

            var location = Locations.EAST;
            var blogPostsView = new BlogPostView
            {
                Title = "My First Blog Post",
                Description = "This is a sample blog post description.",
                Author = karter,
                authorId = karter.Id, // Invalid
                Upvotes = 10,
                blogType = BlogType.Post,
                ReleaseDateTime = DateTime.Now.AddDays(1)
            };

            int result = await _blogHandler.AddPost(blogPostsView);


            Assert.True(_context.BlogPosts.Count() == 0, "1 invalid Record Created");

            Assert.True(result == -1);
        }

        [Fact]
        public async Task GetModelToView_ValidArgumentForValidReturn()
        {
            var karter = new Karter
            {
                Name = "TestKarter",
                YearsExperience = 10
            };
            var blogPosts = Enumerable.Range(1, 4)
                .Select(i => new BlogPost
                {
                    Karter = karter,
                    DateTimePosted = DateTime.Now.AddDays(-i),
                    TaggedTrack = new Track
                    {
                        Title = "Test" + i,
                        Location = Locations.NORTHEAST,

                    }
                }).ToList();
            var blogPostsViews = await _blogHandler.GetModelToView(blogPosts); ;

            Assert.True(blogPostsViews.Count == blogPosts.Count);

            for (int i = 0; i < blogPostsViews.Count; i++)
            {
                Assert.True(blogPostsViews[i].Title == blogPosts[i].Title, "Order has been changed of posts and postsViews");
            }
        }

        [Fact]
        public async Task GetModelToView_EmptyListForEmptyReturn()
        {
            var blogPostsViews = await _blogHandler.GetModelToView(new List<BlogPost>());
            var blogPostsNull = await _blogHandler.GetModelToView((BlogPost)null);

            Assert.True(blogPostsViews.Count == 0, "Invalid Return");
            Assert.True(blogPostsNull == null, "Invalid Return when passed NULL");

        }

        [Fact]
        public async Task GetPost_ValidReturnOfPost()
        {
            CreateDatabase_WithXNumberOfRows(10);

            BlogPostFilterOptions filterOnlyComments = new BlogPostFilterOptions
            {
                IncludeUpvotes = false,
                IncludeComments = true,
            };
            BlogPostFilterOptions filterOnlyUpvotes = new BlogPostFilterOptions
            {
                IncludeUpvotes = true,
                IncludeComments = false,
            };
            BlogPostFilterOptions filterIncludeBoth = new BlogPostFilterOptions
            {
                IncludeUpvotes = true,
                IncludeComments = true,
            };

            BlogPost post = await _blogHandler.GetPost(1);
            BlogPost postWithComments = await _blogHandler.GetPost(2, filterOnlyComments);
            BlogPost postWithUpvotes = await _blogHandler.GetPost(3, filterOnlyComments);
            BlogPost postWithBoth = await _blogHandler.GetPost(4, filterIncludeBoth);


            Assert.True(post != null);
            Assert.True(post.Id == 1);

            Assert.True(postWithComments.Id == 2);
            Assert.True(postWithComments.Comments != null);
            Assert.True(postWithComments.Comments.Count >= 0);


            Assert.True(postWithUpvotes.Id == 3);
            Assert.True(postWithUpvotes.Upvotes != null);
            Assert.True(postWithUpvotes.Upvotes.Count >= 0);


            Assert.True(postWithBoth.Id == 4);
            Assert.True(postWithBoth.Comments != null);
            Assert.True(postWithBoth.Comments.Count >= 0);
            Assert.True(postWithBoth.Upvotes != null);
            Assert.True(postWithBoth.Upvotes.Count >= 0);

        }

        [Fact]
        public async Task GetPost_InvalidId()
        {
            CreateDatabase_WithXNumberOfRows(2);

            BlogPostFilterOptions filterOnlyComments = new BlogPostFilterOptions
            {
                IncludeUpvotes = false,
                IncludeComments = true,
            };
            BlogPostFilterOptions filterOnlyUpvotes = new BlogPostFilterOptions
            {
                IncludeUpvotes = true,
                IncludeComments = false,
            };
            BlogPostFilterOptions filterIncludeBoth = new BlogPostFilterOptions
            {
                IncludeUpvotes = true,
                IncludeComments = true,
            };

            BlogPost post = await _blogHandler.GetPost(1);
            BlogPost postWithComments = await _blogHandler.GetPost(2, filterOnlyComments);
            BlogPost postWithUpvotes = await _blogHandler.GetPost(3, filterOnlyComments);
            BlogPost postWithBoth = await _blogHandler.GetPost(4, filterIncludeBoth);


            Assert.True(post != null);
            Assert.True(post.Id == 1);

            Assert.True(postWithComments.Id == 2);
            Assert.True(postWithComments.Comments != null);
            Assert.True(postWithComments.Comments.Count >= 0);


            Assert.True(postWithUpvotes == null);
            Assert.True(postWithBoth == null);

        }

        [Fact]
        public async Task GetAllCommentsForPost_ValidPostWithComments()
        {

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            int numComments = 20;

            var comments = Enumerable.Range(1, numComments).Select(i => new Comment
            {
                Text = "WOOOT"
            }).ToList();

            var trackNameExample = "Example Track";
            var location = Locations.EAST;
            var blogPosts = Enumerable.Range(1, 2)
                .Select(i => new BlogPost
                {
                    DateTimePosted = DateTime.Now.AddDays(-i),
                    Comments = comments,
                    TaggedTrack = new Track
                    {
                        Title = trackNameExample + "i",
                        Location = location
                    }
                })
                .ToList();

            _context.BlogPosts.AddRange(blogPosts);
            _context.SaveChanges();

            List<BlogPost> posts = _context.BlogPosts.ToList();
            int lastIdSent = 3;

            foreach (var post in posts)
            {
                Assert.True(post.Comments != null, "Comment Value Are Null");
                Assert.True(post.Comments.Count > 0);
                Assert.True(post.Comments.Count <= numComments);

                List<Comment> commentsFetched = await _blogHandler.GetAllCommentsForPostAfterId(post.Id, lastIdSent);

                Assert.True(commentsFetched.Count == 10);
                Assert.True(commentsFetched[0].Id == lastIdSent + 1);
            }
        }

        [Fact]
        public async Task GetAllCommentsForPost_ValidPostWithNoComments()
        {

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            int numComments = 0;

            var comments = Enumerable.Range(1, numComments).Select(i => new Comment
            {
                Text = "WOOOT"
            }).ToList();

            var trackNameExample = "Example Track";
            var location = Locations.EAST;
            var blogPosts = Enumerable.Range(1, 2)
                .Select(i => new BlogPost
                {
                    DateTimePosted = DateTime.Now.AddDays(-i),
                    Comments = comments,
                    TaggedTrack = new Track
                    {
                        Title = trackNameExample + "i",
                        Location = location
                    }
                })
                .ToList();

            _context.BlogPosts.AddRange(blogPosts);
            _context.SaveChanges();

            List<BlogPost> posts = _context.BlogPosts.ToList();

            foreach (var post in posts)
            {
                Assert.True(post.Comments != null, "Comment Value Are Null");
                Assert.True(post.Comments.Count >= 0);
                Assert.True(post.Comments.Count <= numComments);

                List<Comment> commentsFetched = await _blogHandler.GetAllCommentsForPostAfterId(post.Id, 3);

                Assert.True(commentsFetched.Count == 0);
            }
        }

        [Fact]
        public async Task GetAllCommentsForPost_InvalidPostId()
        {

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            int numComments = 20;

            var comments = Enumerable.Range(1, numComments).Select(i => new Comment
            {
                Text = "WOOOT"
            }).ToList();

            var trackNameExample = "Example Track";
            var location = Locations.EAST;
            var blogPosts = Enumerable.Range(1, 2)
                .Select(i => new BlogPost
                {
                    DateTimePosted = DateTime.Now.AddDays(-i),
                    Comments = comments,
                    TaggedTrack = new Track
                    {
                        Title = trackNameExample + "i",
                        Location = location
                    }
                })
                .ToList();

            _context.BlogPosts.AddRange(blogPosts);
            _context.SaveChanges();

            List<BlogPost> posts = _context.BlogPosts.ToList();

            foreach (var post in posts)
            {
                Assert.True(post.Comments != null, "Comment Value Are Null");
                Assert.True(post.Comments.Count > 0);
                Assert.True(post.Comments.Count <= numComments);

                List<Comment> commentsFetched = await _blogHandler.GetAllCommentsForPostAfterId(100, 3);

                Assert.True(commentsFetched.Count == 0);
            }
        }

        [Fact]
        public async Task GetPostsForTrack_ValidInputs()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var trackNameExample = "Example Track";
            var location = Locations.EAST;
            var blogPosts = Enumerable.Range(1, 30)
                .Select(i => new BlogPost
                {
                    DateTimePosted = DateTime.Now.AddDays(-i),
                    TaggedTrack = new Track
                    {
                        Title = trackNameExample + i % 2,
                        Location = location
                    }
                })
                .ToList();

            _context.BlogPosts.AddRange(blogPosts);
            _context.SaveChanges();

            List<BlogPost> posts = await _blogHandler.GetPostsForTrack(trackNameExample + "1", 10);

            Assert.Equal(10, posts.Count);
            foreach (var post in posts)
            {
                Assert.True(post.TaggedTrack.Title == trackNameExample + "1");
            }
        }


        [Fact]
        public async Task GetPostsForTrack_InValidTrackName()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var trackNameExample = "Example Track";
            var location = Locations.EAST;
            var blogPosts = Enumerable.Range(1, 10)
                .Select(i => new BlogPost
                {
                    DateTimePosted = DateTime.Now.AddDays(-i),
                    TaggedTrack = new Track
                    {
                        Title = trackNameExample + i % 2,
                        Location = location
                    }
                })
                .ToList();

            _context.BlogPosts.AddRange(blogPosts);
            _context.SaveChanges();

            List<BlogPost> posts = await _blogHandler.GetPostsForTrack("InvalidName", 10);

            Assert.Equal(0, posts.Count);
        }

        [Fact]
        public async Task GetPostsForTrack_InValidCountReques()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var trackNameExample = "Example Track";
            var location = Locations.EAST;
            var blogPosts = Enumerable.Range(1, 10)
                .Select(i => new BlogPost
                {
                    DateTimePosted = DateTime.Now.AddDays(-i),
                    TaggedTrack = new Track
                    {
                        Title = trackNameExample + i % 2,
                        Location = location
                    }
                })
                .ToList();

            _context.BlogPosts.AddRange(blogPosts);
            _context.SaveChanges();

            List<BlogPost> posts = await _blogHandler.GetPostsForTrack(trackNameExample + "1", -2);

            Assert.Equal(0, posts.Count);
        }
    }
}

