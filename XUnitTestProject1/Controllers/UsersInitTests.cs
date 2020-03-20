using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace XUnitTestProject1.Controllers
{
    public class UsersInitTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;


        public UsersInitTests()
        {
            _server = ServerFactory.GetServerInstance();
            _client = _server.CreateClient();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                _db.User.Add(new User
                {
                    IdUser = 11,
                    Email = "jd@pja.edu.pl",
                    Name = "Daniel",
                    Surname = "Jabłoński",
                    Login = "jd",
                    Password = "ASNDKWQOJRJOP!JO@JOP"
                });

                _db.SaveChanges();

            }
        }


        [Fact]
        public async Task GetUsers_200Ok()
        {
            var httpResponse = await _client.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/users");

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(content);

            Assert.True(users.Count() == 1);
            Assert.True(users.ElementAt(0).Login == "jd");
        }

        [Fact]
        public async Task GetUsersById_200Ok()
        {
            var httpResponse = await _client.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/users/11");

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);
            Assert.True(user.IdUser == 11);
        }

        [Fact]
        public async Task PostUser()
        {
            var newUser = new User
            {
                IdUser = 12,
                Email = "ut@pja.edu.pl",
                Name = "User",
                Surname = "Testowy",
                Login = "ut",
                Password = "pass"
            };
            var httpResponse = await _client.PostAsync($"{_client.BaseAddress.AbsoluteUri}api/users",
                new StringContent(
                    JsonConvert.SerializeObject(newUser),
                    Encoding.UTF8,
                    "application/json"
                ));

            httpResponse.EnsureSuccessStatusCode();

            var content = await httpResponse.Content.ReadAsStringAsync();
            var savedUser = JsonConvert.DeserializeObject<User>(content);


            Assert.True(savedUser.Login == "ut");

        }
    }
}