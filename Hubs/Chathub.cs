using Microsoft.AspNetCore.SignalR; // signalR se hum clinet or server ka darmian communication kar sakta
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Hubs
    //Issue: Datbase se connect nahi hai mock result daty hai jab api down ho , tokens comp hon , Internet band ho
{
    public class ChatHub : Hub
    {
        private readonly IConfiguration _configuration; // ya configuration file access karna ka liya hai eg appsettings.json
        private readonly HttpClient _httpClient; // API request bhajna ka liya obj hai.Is obj se openAi call ho gy 

        private static readonly string SystemPrompt = @"
            You are a helpful AI reservation assistant for a hotel booking system called 'StayVerse'.

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

        public ChatHub(IConfiguration configuration) // ctor 
        {
            _configuration = configuration;
            _httpClient = new HttpClient(); // use kar sakta get, post ko 
        }

        public async Task SendMessage(string message) // ya signalR hub ka method hai 
        {
            try
            {
                var apiKey = _configuration["OpenAI:ApiKey"]; //secret key aa jae gy 

                if (string.IsNullOrEmpty(apiKey))
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", GetMockResponse(message));
                    //signalR ka method hai jis na msg bhaja hai usi ko response bhajta hai 
                    // GetMockResponse agr API key nahi to fake response generate kar da ga 
                    //ya function built-in nahi hai is ko bnaya hoa neecha
                    return;
                }

                var response = await GetAIResponse(message, apiKey); // AI ka response la ga 
                await Clients.Caller.SendAsync("ReceiveMessage", response); // ya aga forward kra ga response frontend ko 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ChatHub: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveMessage",
                    "I'm having trouble connecting. Please try again later.");
            }
        }

        private async Task<string> GetAIResponse(string userMessage, string apiKey)
            //ya private hai isi class ma use ho ga
        {
            try
            {
                var currentDate = DateTime.Now.ToString("MMMM dd, yyyy");
                var systemPrompt = string.Format(SystemPrompt, currentDate);// current date ko get kara ga 

                var requestBody = new // new object bana rae abhi json ma nahi hai ya 
                {
                    model = "llama-3.3-70b-versatile",// kon sa AI model hai 
                    messages = new object[] // AI ko msgs ki list bhaj rae
                    {
                        new { role = "system", content = systemPrompt /*AI ki instructions hain ya user ko nahi dikhata */},
                        new { role = "user", content = userMessage /*Ya actual user ka msg hai*/}
                    },
                    max_tokens = 500, // AI max 300-400 words ka reply da sakta
                    temperature = 0.7
                    /* Creativity control karta hai 
                 (Temp)0   Almost same answer every time
                       0.2 Very factual
                       0.7 Balanced
                       1   Creative
                       2   Very random*/
                };

                var json = JsonConvert.SerializeObject(requestBody); // C# ka obj ko json bana da ga
                //API sirf json samaghty hai
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // server ko btata hai k ya json format ma hai 

                _httpClient.DefaultRequestHeaders.Clear(); // purana header ko remove kar raha hai 
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                // API key aa rae or header bana ga 

                //POST request bhaj raha 
                var response = await _httpClient.PostAsync(
                    "https://api.groq.com/openai/v1/chat/completions", //endpoint of groq api
                    content);// JSON body

                //GROQ na jo json bhaja us ko string ma kar raha
                var responseJson = await response.Content.ReadAsStringAsync();

                Console.WriteLine(response.StatusCode);
                Console.WriteLine(responseJson);

                var result = JsonConvert.DeserializeObject<OpenAIResponse>(responseJson); //JSON  ko phir C# ka obj bana diya 

                //AI ka actual ans nikal raha (Result -> Choice-> 0 -> Message -> Content)
                return result?.Choices?[0]?.Message?.Content ?? "I'm sorry, I couldn't process that. Please try again.";
            }
            catch (Exception ex) //Internet band ho, API down ho etc 
            {
                Console.WriteLine(ex); // Server logs
                return "I'm sorry, something went wrong. Please try again later.";
            }
        }

        //Agr API ko call nahi jaty to ya mock msgs return kara ga If internet not working properly then it returns something 
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
                return "👋 Hello! Welcome to StayVerse!\n\n" +
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
            // ya override hai k ya HUB ma bana hoa is ko hum use kar raha
        {
            await Clients.Caller.SendAsync("ReceiveMessage", // client ko msg jata hai
                "👋 Hello! I'm connected and ready to help you with your booking needs!");
            await base.OnConnectedAsync(); //base is ma HUB hai 
        }

        #region Response Classes 
        // sirf code organize karna ka liya hai 

        public class OpenAIResponse // ya class API ka response ko receive kara gy 
        {
            public Choice[] Choices { get; set; } // API se response ata is ma store kra ga
        }

        public class Choice // aik choice ko represent krta
        {
            public Message Message { get; set; }
        }

        public class Message
        {
            public string Content { get; set; } //{message: {content: ""}}
        }

        #endregion
    }
}