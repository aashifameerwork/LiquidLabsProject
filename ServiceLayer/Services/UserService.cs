using Core.Helpers;
using Core.Model;
using DbLayer.Interfaces;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;

namespace ServiceLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly HttpClient _httpClient;

        public UserService(IUserRepository userRepository, HttpClient httpClient)
        {
            _userRepository = userRepository;
            _httpClient = httpClient;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var usersList = await _userRepository.GetAllUsersAsync();

                if (usersList.Count > 0)
                {
                    return usersList;
                }

                var usersFromApi = await GetUsersFromApiAsync();

                if (usersFromApi.Any())
                {
                    foreach (var user in usersFromApi)
                        await _userRepository.AddUserToDbAsync(user);                 
                }

                return usersFromApi;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving users: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<User> GetUserByIdAsync(long id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);

                if(user.Id != 0)
                {
                    return user;
                }               

                var usersListFromApi = await GetUsersFromApiAsync();
                
                if (!usersListFromApi.Any())
                    return new User();


                foreach (var userObj in usersListFromApi)
                {
                    await _userRepository.AddUserToDbAsync(userObj);
                }

                var requestedUser = usersListFromApi.FirstOrDefault(u => u.Id == id);

                return requestedUser ?? new User();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving user: {ex.Message}");
                return new User();
            }
        }
        
        private async Task<List<User>> GetUsersFromApiAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(Constants.UserApiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to retrieve users from API. Status code: {response.StatusCode}");
                    return new List<User>();
                }
                
                var users = await response.Content.ReadFromJsonAsync<List<User>>();

                return users ?? new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving users from API: {ex.Message}");
                return new List<User>();
            }

            
        }

     
    }
}
