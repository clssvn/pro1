using Library.Entities;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProject1.Controllers
{
    public class BookBorrowsInitTests
    {

        private readonly TestServer _server;
        private readonly HttpClient _client;

        public BookBorrowsInitTests()
        {
            _server = ServerFactory.GetServerInstance();
            _client = _server.CreateClient();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                _db.BookBorrow.Add(new BookBorrow
                {


                    IdBookBorrow = 1,
                    IdUser = 1,
                    IdBook = 1,
                    BorrowDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(1),
                    Comments = "Borrowed"


                });

                _db.SaveChanges();

            }
        }


        [Fact]
        public async Task PostBookBorrow()
        {

            var newBorrow = new BookBorrow
            {
                IdUser = 11,
                IdBook = 2,
                BorrowDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(1),
                Comments = "Borrowed"
            };
            var httpResponse = await _client.PostAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows",
                new StringContent(
                    JsonConvert.SerializeObject(newBorrow),
                    Encoding.UTF8,
                    "application/json"
                ));

            httpResponse.EnsureSuccessStatusCode();

            var content = await httpResponse.Content.ReadAsStringAsync();
            var bookBorrow = JsonConvert.DeserializeObject<BookBorrow>(content);

            Assert.True(bookBorrow.IdBook == 2);
        }

        [Fact]
        public async Task PutBookBorrow()
        {
            int BookBorrowId = 1;
            string newComment = "NewComment";

            var updatedBorrow = new BookBorrow
            {
                IdUser = 1,
                IdBookBorrow = BookBorrowId,
                IdBook = 2,
                BorrowDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(1),
                Comments = newComment
            };
            var httpResponse = await _client.PutAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows/{BookBorrowId}", new StringContent(
                    JsonConvert.SerializeObject(updatedBorrow),
                    Encoding.UTF8,
                    "application/json"
                ));

            httpResponse.EnsureSuccessStatusCode();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
                Assert.True(_db.BookBorrow.Any(e => e.IdBookBorrow == 1 && e.Comments == newComment));
            }


        }
    }
}
