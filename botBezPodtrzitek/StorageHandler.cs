using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace botBezPodtrzitek
{
    internal class StorageHandler
    {
        // Saves application into readable text files
        public static async Task WriteAll(Application a)
        {
            try
            {
                string basePath = AppContext.BaseDirectory;
                string userDir = Path.Combine(basePath, a.User.Username);

                Directory.CreateDirectory(userDir);

                string filePath = Path.Combine(userDir, "application.txt");
                string fileUserPath = Path.Combine(userDir, "user.txt");

                // User info
                var usb = new StringBuilder();
                usb.AppendLine(a.User.Id.ToString());
                usb.AppendLine(a.User.FirstName);
                usb.AppendLine(a.User.LastName);
                usb.AppendLine("@" + a.User.Username);
                usb.AppendLine(DateTime.Now.ToString());

                // Answers
                var sb = new StringBuilder();
                sb.AppendLine(a.User.Username);
                foreach (Question q in a.Questions)
                {
                    sb.AppendLine(" " + q.TextOfQuestion);

                    foreach (Answer ans in q.Answers)
                    {
                        sb.AppendLine("     " + ans.ContainsOfAnswer.Message.Text);
                    }
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                File.WriteAllText(fileUserPath, usb.ToString(), Encoding.UTF8);
            }
            catch
            {
                // Silent fail (not ideal, but prevents crash)
            }
        }

        // Loads existing application or creates a new one
        public static Application LoadUser(ITelegramBotClient bot, Update update, List<Application> KnownApplicants)
        {
            Application appToReturn = new Application();
            bool isItThere = false;

            string route = Path.Combine(AppContext.BaseDirectory, "ids.txt");
            if (!File.Exists(route))
            {
                File.Create(route);

            }
            string[] ids = File.ReadAllLines(route);

            if (ids.Contains(update.Message.From.Id.ToString()))
            {
                return null;
            }
            else
            {
                // Try loading from memory
                foreach (Application appl in KnownApplicants)
                {
                    if (appl.User.Username == update.Message.From.Username)
                    {
                        isItThere = true;
                        appToReturn = appl;
                    }
                }
            }

            // If not found → create new application
            if (!isItThere)
            {
                appToReturn.User = update.Message.From;
                Application.AddDefaultQuestions(appToReturn);
                KnownApplicants.Add(appToReturn);
            }

            return appToReturn;
        }

        public static void SaveUserAndWrite(Application appl, Update update)
        {
            // Save application to JSON file if not already saved
            string route = Path.Combine(AppContext.BaseDirectory, "ids.txt");
            if (!File.Exists(route))
            {
                File.Create(route);

            }

                File.AppendAllText(route, appl.User.Id.ToString() + "\n");


            // Also save as readable text
            StorageHandler.WriteAll(appl);

            return;
        }
    }
}
