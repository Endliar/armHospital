﻿using armHospital.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace armHospital.Data.Repositories
{
    public class AppointmentRepository
    {
        private readonly DatabaseContext _context;

        public AppointmentRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAppointmentsByUserId(int userId)
        {
            var appointments = new List<Appointment>();

            using (var connection = _context.CreateConnection())
            {
                var npgsqlConnection = (NpgsqlConnection)connection;
                await npgsqlConnection.OpenAsync();

                var query = "SELECT * FROM appointments WHERE user_id = @userId";

                using (var command = new NpgsqlCommand(query, npgsqlConnection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            appointments.Add(new Appointment
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                DoctorId = reader.GetInt32(2),
                                Title = reader.GetString(3),
                                Description = reader.GetString(4),
                                AppointmentDate = reader.GetDateTime(5),
                                Status = reader.GetString(6),
                                Comments = reader.GetString(7),
                                CreatedAt = reader.GetDateTime(8),
                                UpdatedAt = reader.GetDateTime(9)
                            });
                        }
                    }
                }
            }

            return appointments;
        }

        public async Task<List<Appointment>> GetAllAppointments()
        {
            var appointments = new List<Appointment>();

            using (var connection = _context.CreateConnection())
            {
                var npgsqlConnection = (NpgsqlConnection)connection;
                await npgsqlConnection.OpenAsync();

                var query = "SELECT * FROM appointments";
                using (var command = new NpgsqlCommand(query, npgsqlConnection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            appointments.Add(new Appointment
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                                DoctorId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                                Title = reader.GetString(3),
                                Description = reader.GetString(4),
                                AppointmentDate = reader.GetDateTime(5),
                                Status = reader.GetString(6),
                                Comments = reader.GetString(7),
                                CreatedAt = reader.GetDateTime(8),
                                UpdatedAt = reader.GetDateTime(9)
                            });
                        }
                    }
                }
            }

            return appointments;
        }

        public async Task AddAppointment(Appointment appointment)
        {
            using (var connection = _context.CreateConnection())
            {
                var npgsqlConnection = (NpgsqlConnection)connection;
                await npgsqlConnection.OpenAsync();

                var query = @"
            INSERT INTO appointments (user_id, doctor_id, title, description, appointment_date, status, comments)
            VALUES (@UserId, @DoctorId, @Title, @Description, @AppointmentDate, @Status, @Comments)";
                using (var command = new NpgsqlCommand(query, npgsqlConnection))
                {
                    command.Parameters.AddWithValue("@UserId", (object)appointment.UserId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DoctorId", (object)appointment.DoctorId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Title", appointment.Title);
                    command.Parameters.AddWithValue("@Description", appointment.Description);
                    command.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);
                    command.Parameters.AddWithValue("@Status", appointment.Status);
                    command.Parameters.AddWithValue("@Comments", appointment.Comments);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
