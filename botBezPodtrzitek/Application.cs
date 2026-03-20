using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace botBezPodtrzitek
{
    // Represents entire user application
    public class Application
    {
        public User User { get; set; }
        public bool Started { get; set; }
        public List<Question> Questions { get; set; }
        public int NextQuestion { get; set; }

        public Application(User user, List<Question> questions)
        {
            User = user;
            Questions = questions;
            NextQuestion = 0;
            Started = false;
        }

        public Application()
        {
            User = new User();
            Questions = new List<Question>();
            NextQuestion = 0;
            Started = false;
        }

        // Sends a question to the user
        public async Task AskQuestion(int questionId, long chatID, ITelegramBotClient bot)
        {
            await bot.SendMessage(
                chatId: chatID,
                text: Questions[questionId].TextOfQuestion
            );
        }

        // Adds default set of questions to application
        public static void AddDefaultQuestions(Application appl)
        {
            List<Question> questions = new List<Question>();

            questions.Add(new Question("Řekni mi, proč chceš být součástí právě skupiny PičínFurr.", UpdateType.Message));
            questions.Add(new Question("Co tě na této roli nejvíc láká a co naopak nejméně?", UpdateType.Message));
            questions.Add(new Question("Jaké vlastnosti podle tebe dělají dobrého helpera?", UpdateType.Message));
            questions.Add(new Question("Jak by jsi reagoval na konflikt během outingu, např. když do sebe dva lidé začnou strkat.", UpdateType.Message));
            questions.Add(new Question("Máš skoro hotovo, projdi si své odpovědi, pokud se chceš ještě k něčemu vrátit, použij /back jinak napiš cokoliv bez lomítka, aby jsi své odpovědi odeslal.", UpdateType.Message));

            appl.Questions = questions;
        }
    }
}
