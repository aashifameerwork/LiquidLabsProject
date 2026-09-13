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

        /// <summary>
        /// Gets all users from the database. If no users are found, it gets them from the API and saves them in the db.
        /// </summary>
        /// <returns>
        /// Success: List of Users
        /// Failure: An empty list
        /// </returns>
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

        /// <summary>
        /// Gets a user by ID from the database. 
        /// If the user is not found, it gets all users from the API, saves them in the db, and then returns the requested user.
        /// </summary>
        /// <param name="id">ID of the user to retrieve</param>
        /// <returns>
        /// Success: User object
        /// Failure: An empty User object
        /// </returns>
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

                var requestedUser = usersListFromApi.FirstOrDefault(u => u.Id == id);
                if(requestedUser == null)
                    return new User();

                foreach (var userObj in usersListFromApi)
                {
                    await _userRepository.AddUserToDbAsync(userObj);
                }

                return requestedUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving user: {ex.Message}");
                return new User();
            }
        }

        /// <summary>
        /// Gets users from the public API.
        /// Public API URL : https://gorest.in/public/v2/users
        /// </summary>
        /// <returns>
        /// Success: List of Users
        /// Failure: An empty list
        /// </returns>
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
