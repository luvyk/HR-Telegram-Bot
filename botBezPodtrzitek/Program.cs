namespace botBezPodtrzitek
{
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Text.Json;
    using Telegram.Bot;
    using Telegram.Bot.Polling;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    class Program
    {
        // In-memory list of currently active applicants (not yet saved/finished)
        public static List<Application> KnownApplicants = new List<Application>();

        static async Task Main()
        {
            // Initialize bot client with token
            var bot = new TelegramBotClient("place for token");

            using var cts = new CancellationTokenSource();

            // Configure receiver to accept all update types
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive everything
            };

            // Start listening for incoming updates
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token
            );

            // Set bot description (shown in Telegram UI)
            await bot.SetMyDescription(
                description: "Jsem testovací bot pro nábor. Použij /start aby jsi začal nábor a následně /back a /forward pro cyklování mezi otázkami a pro doplnění více informací."
            );

            // Short description
            await bot.SetMyShortDescription(
                shortDescription: "Testovací bot pro nábor. Použij /start a pak /back /forward pro doplnění více informací"
            );

            // Get bot info and print to console
            User me = await bot.GetMe();
            Console.WriteLine($"Bot {me.Username} runs...");

            // Keep app running until user presses Enter
            Console.ReadLine();
            cts.Cancel();
        }

        // Main update handler - processes every incoming message/update
        static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            // Ignore messages from me (the bot)
            if (update.Message.From.Id == 1087968824)
            {
                return;
            }

            // Ignore if update has no message
            if (update.Message is not { } message)
                return;

            // Ignore if message has no text
            if (message.Text is not { } text)
                return;

            Console.WriteLine($"Incoming message: {text}");

            // Load or create application for this user
            Application appl = StorageHandler.LoadUser(bot, update, KnownApplicants);

            if(appl is null)
            {
                await bot.SendMessage(
                       chatId: message.Chat.Id,
                       text: $"You've answered all the questions, please wait until we contact you further"
                   );
                return;
            }

            // Start command - begins application process
            if ((!appl.Started) && text == "/start")
            {
                appl.Started = true;

                // Welcome message
                await bot.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"(bude přeloženo) vítej, pokud chceš být součástí skupiny _____ _____, musíš zodpovědět pár otázek"
                );
            }
            else if (!appl.Started)
            {
                // User hasn't started yet → instruct them
                await bot.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"If you want to apply as a _____ _____ (PF) helper type \"/start\""
                );
            }

            if (appl.Started)
            {
                // Navigation between questions
                if (text == "/back" && appl.NextQuestion > 0)
                {
                    appl.NextQuestion--;
                }
                else if (text == "/forward" && appl.NextQuestion < appl.Questions.Count - 1)
                {
                    appl.NextQuestion++;
                }

                // If message is not a command, treat it as an answer
                if (text[0] != '/')
                {
                    appl.Questions[appl.NextQuestion].Answer(update);
                    appl.NextQuestion++;
                }

                // If all questions are answered
                if (appl.NextQuestion >= appl.Questions.Count)
                {
                    await bot.SendMessage(
                        chatId: message.Chat.Id,
                        text: $"You've answered all the questions, please wait until we contact you further"
                    );

                    StorageHandler.SaveUserAndWrite(appl, update);

                    // Remove from active list
                    KnownApplicants.Remove(appl);
                    return;
                }

                // Ask next question
                await appl.AskQuestion(appl.NextQuestion, message.Chat.Id, bot);

                long collectorChatId = -1003869252711; // ID of admin/collector chat

                var msg = update.Message;
                if (msg == null)
                    return;

                // Send info about which question this is
                await bot.SendMessage(
                    chatId: collectorChatId,
                    text: $"{appl.NextQuestion}. otázka"
                );

                // Forward user's message to collector chat
                await bot.ForwardMessage(
                    chatId: collectorChatId,
                    fromChatId: msg.Chat.Id,
                    messageId: msg.MessageId
                );
            }
        }

        // Error handler
        static Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return Task.CompletedTask;
        }
    }
}