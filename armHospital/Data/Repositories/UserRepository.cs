using armHospital.Models;
using Npgsql;

namespace armHospital.Data.Repositories
{
    public class UserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AddUser(User user)
        {
            using (var connection = _context.CreateConnection())
            {
                var npgsqlConnection = (NpgsqlConnection)connection;
                await npgsqlConnection.OpenAsync();

                var query = @"
                    INSERT INTO users (username, email, password, phone_number, role, full_name)
                    VALUES (@Username, @Email, @Password, @PhoneNumber, @Role, @FullName)";

                using (var command = new NpgsqlCommand(query, (NpgsqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<User> FindUserByUsername(string username)
        {
            using (var connection = _context.CreateConnection())
            {
                var npgsqlConnection = (NpgsqlConnection)connection;
                await npgsqlConnection.OpenAsync();

                var query = "SELECT * FROM users WHERE username = @Username";
                using (var command = new NpgsqlCommand(query, (NpgsqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                PhoneNumber = reader.GetString(4),
                                Role = reader.GetString(5),
                                FullName = reader.GetString(6),
                                CreatedAt = reader.GetDateTime(7)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<List<User>> GetUsersByRole(string role)
        {
            var users = new List<User>();

            using (var connection = _context.CreateConnection())
            {
                var npgsqlConnection = (NpgsqlConnection)connection;
                await npgsqlConnection.OpenAsync();

                var query = "SELECT * FROM users WHERE role = @Role";
                using (var command = new NpgsqlCommand(query, npgsqlConnection))
                {
                    command.Parameters.AddWithValue("@Role", role);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                PhoneNumber = reader.GetString(4),
                                Role = reader.GetString(5),
                                FullName = reader.GetString(6),
                                CreatedAt = reader.GetDateTime(7)
                            });
                        }
                    }
                }
            }

            return users;
        }

        public async Task<User> GetUserById(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var npgsqlConnection = (NpgsqlConnection)connection;
                await npgsqlConnection.OpenAsync();

                var query = "SELECT * FROM users WHERE id = @Id";
                using (var command = new NpgsqlCommand(query, npgsqlConnection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                PhoneNumber = reader.GetString(4),
                                Role = reader.GetString(5),
                                FullName = reader.GetString(6),
                                CreatedAt = reader.GetDateTime(7)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task UpdateUser(User user)
        {
            using (var connection = _context.CreateConnection())
            {
                var npgsqlConnection = (NpgsqlConnection)connection;
                await npgsqlConnection.OpenAsync();

                var query = @"
            UPDATE users
            SET 
                email = @Email,
                phone_number = @PhoneNumber,
                full_name = @FullName
            WHERE id = @Id";

                using (var command = new NpgsqlCommand(query, npgsqlConnection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
