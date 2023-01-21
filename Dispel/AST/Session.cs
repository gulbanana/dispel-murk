using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Dispel.AST
{
    public class Session
    {
        public string Ident;
        public readonly Line[] Body;
        public readonly string StartTime;
        public readonly string EndTime;
        public readonly string[] Participants;

        public Session(string ident, string startTime, string endTime, IEnumerable<Line> body, IEnumerable<string> participants)
        {
            Ident = ident;
            Body = body.ToArray();

            if (DateTime.TryParseExact(startTime, "ddd MMM dd HH:mm:ss yyyy", null, DateTimeStyles.None, out var startDate))
            {
                StartTime = startDate.ToShortDateString();
            }
            else
            {
                StartTime = startTime;
            }

            if (DateTime.TryParseExact(endTime, "ddd MMM dd HH:mm:ss yyyy", null, DateTimeStyles.None, out var endDate))
            {
                EndTime = endDate.ToShortDateString();
            }
            else
            {
                EndTime = endTime;
            }

            Participants = participants.ToArray();
        }
    }
}
