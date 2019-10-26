using System;
using System.Collections.Generic;
using System.IO;

namespace OpenFin.Notifications.Demo
{
    public enum BodyContentType
    {
        PlainText,
        Markdown
    }

    internal static class NotificationBodyService
    {
        public static string GetNotificationBodyContent(BodyContentType contentType)
        {
            switch (contentType)
            {
                case BodyContentType.PlainText:
                    return getBodyContentByExtension("txt");

                case BodyContentType.Markdown:
                    return getBodyContentByExtension("md");

                default:
                    throw new ArgumentException("unknown content type");
            }
        }

        public static ButtonOptions[] GenerateButtons(int buttonCount)
        {
            var options = new List<ButtonOptions>();

            for (int i = 0; i < buttonCount; i++)
            {
                options.Add(new ButtonOptions
                {
                    Title = $"Button{i + 1}",
                    IconUrl = "https://openfin.co/favicon-32x32.png",
                    OnNotificationButtonClick = new Dictionary<string, object>
                        {
                            { "btn", $"button{i + 1}" }
                        }
                });
            }

            return options.ToArray();
        }

        private static string getBodyContentByExtension(string ext)
        {
            return File.ReadAllText($"body.{ext}");
        }
    }
}