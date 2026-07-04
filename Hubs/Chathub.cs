using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        private static readonly string SystemPrompt = @"
You are a helpful AI reservation assistant for a hotel booking system called 'BookingBot'.

Your role is to help users with:
1. Checking room/property availability
2. Making reservations
3. Answering questions about properties
4. Providing information about amenities, pricing, and policies

Guidelines:
- Be friendly, professional, and concise
- If a user asks about availability, ask for: check-in date, check-out date, number of guests
- If a user wants to book, guide them through the process
- If you don't know something, say so and suggest they check the website or contact support
- Keep responses under 200 words

Today's date is: {0}
";

        public ChatHub(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task SendMessage(string message)
        {
            try
            {
                var apiKey = _configuration["OpenAI:ApiKey"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", GetMockResponse(message));
                    return;
                }

                var response = await GetAIResponse(message, apiKey);
                await Clients.Caller.SendAsync("ReceiveMessage", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ChatHub: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveMessage",
                    "I'm having trouble connecting. Please try again later.");
            }
        }

        private async Task<string> GetAIResponse(string userMessage, string apiKey)
        {
            try
            {
                var currentDate = DateTime.Now.ToString("MMMM dd, yyyy");
                var systemPrompt = string.Format(SystemPrompt, currentDate);

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new object[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userMessage }
                    },
                    max_tokens = 500,
                    temperature = 0.7
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var response = await _httpClient.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    content);

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<OpenAIResponse>(responseJson);

                return result?.Choices?[0]?.Message?.Content ?? "I'm sorry, I couldn't process that. Please try again.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OpenAI Error: {ex.Message}");
                return GetMockResponse(userMessage);
            }
        }

        private string GetMockResponse(string message)
        {
            message = message.ToLower();

            if (message.Contains("book") || message.Contains("reservation"))
            {
                return "Great! I can help you book a reservation. Please tell me:\n\n" +
                       "📅 What date would you like to book?\n" +
                       "👥 How many guests?\n" +
                       "⏰ What time would you prefer?\n\n" +
                       "You can also check availability on our website.";
            }
            else if (message.Contains("available") || message.Contains("availability"))
            {
                return "Let me check availability for you! 🏨\n\n" +
                       "We have several properties available:\n" +
                       "• Grand Plaza Hotel - $189/night\n" +
                       "• Oceanview Apartments - $145/night\n" +
                       "• Mountain Lodge & Spa - $275/night\n\n" +
                       "To check specific dates, please tell me your check-in and check-out dates.";
            }
            else if (message.Contains("hour") || message.Contains("time") || message.Contains("open"))
            {
                return "🕐 Our operating hours:\n\n" +
                       "• Reservations: 24/7 via our website or chat\n" +
                       "• Check-in: 3:00 PM - 11:00 PM\n" +
                       "• Check-out: 11:00 AM\n\n" +
                       "Our front desk is available 24/7 for assistance.";
            }
            else if (message.Contains("hello") || message.Contains("hi") || message.Contains("hey"))
            {
                return "👋 Hello! Welcome to BookingBot!\n\n" +
                       "I'm here to help you with:\n" +
                       "• Finding available properties\n" +
                       "• Making reservations\n" +
                       "• Answering your questions\n\n" +
                       "How can I assist you today?";
            }
            else
            {
                return "🤔 I can help you with:\n" +
                       "• Booking a reservation\n" +
                       "• Checking availability\n" +
                       "• Property information\n" +
                       "• Operating hours\n\n" +
                       "Could you please rephrase your question?";
            }
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveMessage",
                "👋 Hello! I'm connected and ready to help you with your booking needs!");
            await base.OnConnectedAsync();
        }

        #region Response Classes

        public class OpenAIResponse
        {
            public Choice[] Choices { get; set; }
        }

        public class Choice
        {
            public Message Message { get; set; }
        }

        public class Message
        {
            public string Content { get; set; }
        }

        #endregion
    }
}