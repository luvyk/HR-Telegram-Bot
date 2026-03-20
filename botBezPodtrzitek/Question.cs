using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace botBezPodtrzitek
{

    // Represents a single question in the application
    public class Question
    {
        public string TextOfQuestion { get; set; }
        public List<Answer> Answers { get; set; }
        public DateTime TimeOfQuestionEntry { get; set; }
        public UpdateType ExpectedTypeOfAnswer { get; set; }

        public Question()
        {
            TextOfQuestion = string.Empty;
            TimeOfQuestionEntry = DateTime.MinValue;
            ExpectedTypeOfAnswer = new UpdateType();
            Answers = new List<Answer>();
        }

        public Question(string textOfQ, UpdateType updt)
        {
            TextOfQuestion = textOfQ;
            TimeOfQuestionEntry = DateTime.MinValue;
            ExpectedTypeOfAnswer = updt;
            Answers = new List<Answer>();
        }

        // Checks if question already has at least one answer
        public bool IsAnswered()
        {
            return Answers.Count > 0;
        }

        // Stores answer if type matches expected type
        public async Task Answer(Update upd)
        {
            if (ExpectedTypeOfAnswer == upd.Type)
            {
                Answers.Add(new Answer(upd));
            }
            else
            {
                Console.WriteLine("Error: wrong answer format");
            }
        }
    }

}
