using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Newtonsoft.Json.Linq;

namespace telegrambot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new TelegramBotClient("7921317001:AAGfssNnc1OhestDTxWBTp3zwhZ1SEkW7Y4");
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        private static async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine(exception.Message);
        }

        async static Task Update(ITelegramBotClient Botclient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message.Text != null)
            {
                Console.WriteLine($"{message.Chat.Username ?? "anon"}   |    {message.Text}");

                if (message.Text.ToLower().Contains("phrase"))
                {
                    string quote = await GetBreakingBadQuote();
                    await Botclient.SendTextMessageAsync(message.Chat.Id, quote);
                }
                else
                {
                    await Botclient.SendTextMessageAsync(message.Chat.Id, "Hmm, you clearly don't know who you're talking to. So let me enlighten you. I'm not some stupid bot, Skyler. I'm the best bot. \nWrite \"Phrase\" and you will see everything for yourself.");
                }
            }
        }
        private static async Task<string> GetBreakingBadQuote()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.breakingbadquotes.xyz/v1/quotes";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Логируем статус ответа
                    Console.WriteLine($"Response status code: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response JSON: {json}"); 

                        JArray jsonArray = JArray.Parse(json);
                        string quote = jsonArray[0]["quote"].ToString();
                        string author = jsonArray[0]["author"].ToString();
                        return $"{quote} \n- {author}";
                    }
                    else
                    {
                        return $"API request failed with status code: {response.StatusCode}";
                    }
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine($"Exception caught: {ex.Message}");
                    return "I am the one who knocks, but something went wrong!";
                }
            }
        }
    }
}