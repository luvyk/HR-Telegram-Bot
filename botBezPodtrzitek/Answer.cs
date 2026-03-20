using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace botBezPodtrzitek
{
    // Represents a single answer from user
    public class Answer
    {
        public Update ContainsOfAnswer { get; set; }
        public DateTime DateTimeOfAnswer { get; set; }

        public Answer(Update containsOfAnswer)
        {
            ContainsOfAnswer = containsOfAnswer;
            DateTimeOfAnswer = DateTime.Now;
        }

        public Answer()
        {
            ContainsOfAnswer = new Update();
            DateTimeOfAnswer = new DateTime();
        }
    }
}
