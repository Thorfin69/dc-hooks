using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace discordTool
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "dchook";
            while (true)
            {
                Console.Clear();
                Banner();
                Menu();
                ConsoleKeyInfo input = Console.ReadKey();
                char option = input.KeyChar;
                Console.WriteLine();

                switch (option)
                {
                    case '1':
                        await webhookMessage();
                        break;
                    case '2':
                        await webhookEmbed();
                        break;
                    case '3':
                        await massMessage();
                        break;
                    case '4':
                        await deleteWebhook();
                        break;
                    case '5':
                        await checkWebhook();
                        break;
                    case '6':
                        await spamMessage();
                        break;
                    case '7':
                        return;
                }
            }
        }

        static void Banner()
        {
            // Vibrant gradient colors
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("                             ________  ________  ___  ___  ________  ________  ___  **    **______      ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"                            |\   ___ \|\   ____\|\  \|\  \|\   **  \|\   **  \|\  \|\  \ |\   ____\     ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"                            \ \  \_|\ \ \  \___|\ \  \\\  \ \  \|\  \ \  \|\  \ \  \/  /|\ \  \___|_    ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(@"                             \ \  \ \\ \ \  \    \ \   **  \ \  \\\  \ \  \\\  \ \   **_  \ \_____  \   ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(@"                              \ \  \_\\ \ \  \____\ \  \ \  \ \  \\\  \ \  \\\  \ \  \\ \  \|____|\  \  ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@"                               \ \_______\ \_______\ \__\ \__\ \_______\ \_______\ \__\\ \__\____\_\  \ ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(@"                                \|_______|\|_______|\|__|\|__|\|_______|\|_______|\|__| \|__|\_________\");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                                                                                dchooks - @ThorfinxD");
            Console.ResetColor();
        }

        static void Menu()
        {
            Console.WriteLine("1. Send Simple Message");
            Console.WriteLine("2. Send Embed Message");
            Console.WriteLine("3. Mass Message (Multiple Webhooks)");
            Console.WriteLine("4. Delete Webhook");
            Console.WriteLine("5. Check Webhook Info");
            Console.WriteLine("6. Spam Message");
            Console.WriteLine("7. Exit");
        }

        static async Task webhookMessage()
        {
            Console.Clear();
            Console.Write("Webhook URL: ");
            string webhook = Console.ReadLine();

            Console.Write("Message: ");
            string message = Console.ReadLine();

            Console.Write("Number of times to repeat (1 for single message): ");
            int repeatCount = int.TryParse(Console.ReadLine(), out int count) && count > 0 ? count : 1;

            Console.Write("Delay between messages (in milliseconds, 0 for no delay): ");
            int delay = int.TryParse(Console.ReadLine(), out int d) && d >= 0 ? d : 0;

            string json = $"{{\"content\":\"{message}\"}}";
            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < repeatCount; i++)
                {
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(webhook, content);

                    if (response.IsSuccessStatusCode)
                        Console.WriteLine($"Message {i + 1}/{repeatCount} sent successfully!");
                    else
                        Console.WriteLine($"Failed to send message {i + 1}. Status code: {response.StatusCode}");

                    if (delay > 0 && i < repeatCount - 1)
                        await Task.Delay(delay);
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static async Task webhookEmbed()
        {
            Console.Clear();
            Console.Write("Webhook URL: ");
            string webhook = Console.ReadLine();

            Console.Write("Embed Title: ");
            string title = Console.ReadLine();

            Console.Write("Embed Description: ");
            string description = Console.ReadLine();

            Console.Write("Embed Color (hex, e.g., FF0000 for red): ");
            string colorHex = Console.ReadLine();
            int color = int.TryParse(colorHex, System.Globalization.NumberStyles.HexNumber, null, out int col) ? col : 0xFF0000;

            string json = $@"{{
                ""embeds"": [
                    {{
                        ""title"": ""{title}"",
                        ""description"": ""{description}"",
                        ""color"": {color}
                    }}
                ]
            }}";

            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(webhook, content);

                if (response.IsSuccessStatusCode)
                    Console.WriteLine("Embed sent successfully!");
                else
                    Console.WriteLine($"Failed to send embed. Status code: {response.StatusCode}");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static async Task massMessage()
        {
            Console.Clear();
            var webhooks = new List<string>();

            Console.WriteLine("Enter webhook URLs (one per line, enter blank line to finish):");
            string input;
            while (!string.IsNullOrWhiteSpace(input = Console.ReadLine()))
                webhooks.Add(input);

            Console.Write("Message to send to all webhooks: ");
            string message = Console.ReadLine();
            string json = $"{{\"content\":\"{message}\"}}";

            using (HttpClient client = new HttpClient())
            {
                foreach (string webhook in webhooks)
                {
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(webhook, content);

                    if (response.IsSuccessStatusCode)
                        Console.WriteLine($"Message sent successfully to {webhook}!");
                    else
                        Console.WriteLine($"Failed to send to {webhook}. Status code: {response.StatusCode}");
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static async Task deleteWebhook()
        {
            Console.Clear();
            Console.Write("Webhook URL to delete: ");
            string webhook = Console.ReadLine();

            using (HttpClient client = new HttpClient())
            {
                var response = await client.DeleteAsync(webhook);

                if (response.IsSuccessStatusCode)
                    Console.WriteLine("Webhook deleted successfully!");
                else
                    Console.WriteLine($"Failed to delete webhook. Status code: {response.StatusCode}");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static async Task checkWebhook()
        {
            Console.Clear();
            Console.Write("Webhook URL to check: ");
            string webhook = Console.ReadLine();

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(webhook);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("\nWebhook Information:");
                    Console.WriteLine((content));
                }
                else
                {
                    Console.WriteLine($"Failed to get webhook info. Status code: {response.StatusCode}");
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static async Task spamMessage()
        {
            Console.Clear();
            Console.Write("Webhook URL: ");
            string webhook = Console.ReadLine();

            Console.Write("Message: ");
            string message = Console.ReadLine();

            Console.Write("Number of messages (1-100): ");
            int count = int.TryParse(Console.ReadLine(), out int num) && num > 0 && num <= 100 ? num : 10;

            Console.Write("Delay between messages (in milliseconds): ");
            int delay = int.TryParse(Console.ReadLine(), out int d) && d >= 0 ? d : 100;

            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < count; i++)
                {
                    string json = $"{{\"content\":\"{message} [{i + 1}/{count}]\"}}";
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(webhook, content);

                    if (response.IsSuccessStatusCode)
                        Console.WriteLine($"Message {i + 1}/{count} sent!");
                    else
                    {
                        Console.WriteLine($"Failed to send message {i + 1}. Status code: {response.StatusCode}");
                        break;
                    }

                    if (delay > 0) await Task.Delay(delay);
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

       
}
