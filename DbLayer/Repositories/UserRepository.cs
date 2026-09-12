using Core.Model;
using DbLayer.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DbLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Gets all users from the Users table
        /// </summary>
        /// <returns>
        /// Success: List of User objects
        /// Failure: An empty list
        /// </returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = $"""
                    SELECT {nameof(User.Id)},
                           {nameof(User.Name)},
                           {nameof(User.Email)},
                           {nameof(User.Gender)},
                           {nameof(User.Status)}
                    FROM Users
                    """;

                await using var cmd = new SqlCommand(query, connection);
                await using var reader = await cmd.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    var user = MakeUser(reader);
                    
                    users.Add(user);
                }

                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving users: {ex.Message}");
                return new List<User>();
            }
        }

        /// <summary>
        /// Gets a user by their ID from the Users table
        /// </summary>
        /// <param name="id">primary key of the user</param>
        /// <returns>
        /// Success: User object
        /// Failure: An empty User object
        /// </returns>
        public async Task<User> GetUserByIdAsync(long id)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = $"""
                    SELECT {nameof(User.Id)},
                           {nameof(User.Name)},
                           {nameof(User.Email)},
                           {nameof(User.Gender)},
                           {nameof(User.Status)}
                    FROM Users
                    WHERE {nameof(User.Id)} = @Id
                    """;

                await using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                
                await using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())            
                    return new User();                

                return MakeUser(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving user: {ex.Message}");
                return new User();
            }
        }

        /// <summary>
        /// Adds a new user to the Users table
        /// </summary>
        /// <param name="user">User to add</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> AddUserToDbAsync(User user)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = $"""
                    INSERT INTO Users (
                    {nameof(User.Id)},
                    {nameof(User.Name)},
                    {nameof(User.Email)},
                    {nameof(User.Gender)}, 
                    {nameof(User.Status)}
                    )
                    VALUES (@Id, @Name, @Email, @Gender, @Status)
                    """;

                await using var cmd = new SqlCommand(query, connection);

               
                cmd.Parameters.AddWithValue("@Id", user.Id); 
                cmd.Parameters.AddWithValue("@Name", user.Name); 
                cmd.Parameters.AddWithValue("@Email", user.Email); 
                cmd.Parameters.AddWithValue("@Gender", user.Gender); 
                cmd.Parameters.AddWithValue("@Status", user.Status); 

                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding user: {ex.Message}");
                return false;
            }
        }

        // Converts a sqlDataReader row into a User object
        private User MakeUser(SqlDataReader reader)
        {
            return new User
            {
                Id = Convert.ToInt64(reader[nameof(User.Id)]),
                Name = reader[nameof(User.Name)].ToString() ?? string.Empty,
                Email = reader[nameof(User.Email)].ToString() ?? string.Empty,
                Gender = reader[nameof(User.Gender)].ToString() ?? string.Empty,
                Status = reader[nameof(User.Status)].ToString() ?? string.Empty
            };
        }
    }
}
