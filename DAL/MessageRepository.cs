using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MessageProject.Models;
using Npgsql;

namespace MessageProject.DAL
{
    public class MessageRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(IConfiguration configuration, ILogger<MessageRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
        }

        public async Task<bool> InsertMessageAsync(Message message)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "INSERT INTO messages (sequence_number, text, timestamp) VALUES (@seq, @text, @timestamp)";
                await using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("seq", message.SequenceNumber);
                cmd.Parameters.AddWithValue("text", message.Text);
                cmd.Parameters.AddWithValue("timestamp", message.Timestamp);

                var rows = await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("Inserted message with sequence {SequenceNumber}. Rows affected: {Rows}", 
                                        message.SequenceNumber, rows);
                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting message with sequence {SequenceNumber}", message.SequenceNumber);
                return false;
            }
        }

        public async Task<List<Message>> GetMessagesAsync(DateTime from, DateTime to, int limit = 100)
        {
            var messages = new List<Message>();
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "SELECT id, sequence_number, text, timestamp FROM messages " +
                          "WHERE timestamp BETWEEN @from AND @to ORDER BY timestamp ASC LIMIT @limit";
                await using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("from", from);
                cmd.Parameters.AddWithValue("to", to);
                cmd.Parameters.AddWithValue("limit", limit);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    messages.Add(new Message
                    {
                        Id = reader.GetInt32(0),
                        SequenceNumber = reader.GetInt32(1),
                        Text = reader.GetString(2),
                        Timestamp = reader.GetDateTime(3)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages from {From} to {To}", from, to);
            }

            return messages;
        }
    }
}
