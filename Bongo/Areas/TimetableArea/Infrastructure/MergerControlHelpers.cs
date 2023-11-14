using Bongo.Areas.TimetableArea.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.RegularExpressions;

namespace Bongo.Areas.TimetableArea.Infrastructure
{
    public static class MergerControlHelpers
    {
        static Regex timepattern = new Regex(@"[0-9]{2}:[0-9]{2} [0-9]{2}:[0-9]{2}");
        static Regex daypattern = new Regex(@"Monday|Tuesday|Wednesday|Thursday|Friday");

        public static void Merge(Session[,] Sessions, Session[,] newSessions)
        {
            newSessions = newSessions.SplitSessions();

            foreach(var session in newSessions)
            {
                if (session == null) continue;

                int i = session.Period[0] - 1, j = session.Period[1] - 1;

                if (Sessions[i, j] != null)
                    Sessions[i, j].userCount++;
                else
                    Sessions[i, j] = session;
            }
        }

        public static void UnMerge(Session[,] Sessions, Session[,] removedSessions)
        {
            removedSessions = removedSessions.SplitSessions();

            foreach (var session in removedSessions)
            {
                if (session == null) continue;

                int i = session.Period[0] - 1, j = session.Period[1] - 1;

                if (Sessions[i, j] != null && Sessions[i, j].userCount>1)
                    Sessions[i,j].userCount--;
                else
                    Sessions[i, j] = null;
            }
        }
        public static Session[,] SplitSessions(this Session[,] Sessions)
        {
            Session[,] _Sessions = Sessions.DeepEmptyCopy();
            foreach (var session in Sessions)
            {
                if (session == null) continue;

                int[] hourRange = getHourRange(session.sessionInPDFValue);
                if (hourRange[1] - hourRange[0] != 1)
                {
                    SplitRangeHourly(_Sessions, hourRange[0], hourRange[1],
                        daypattern.Match(session.sessionInPDFValue).Value);
                }
            }

            return _Sessions;
        }
        
        private static int[] getHourRange(string sessionInPDFValue)
        {
            Match timeMatch = timepattern.Match(sessionInPDFValue);

            int minHour = int.Parse(timeMatch.Value.Substring(0, 2));
            int maxHour = int.Parse(timeMatch.Value.Substring(6, 2));

            return new int[] { minHour, maxHour };
        }

        private static void SplitRangeHourly(Session[,] Sessions, int minHour, int maxHour, string day)
        {
            for (int i = minHour; i < maxHour; i++)
            {
                string hour = i < 10 ? $"0{i}" : $"{i}";

                int[] period = Periods.GetPeriod(hour, day);
                Sessions[period[0] - 1, period[1] - 1] = new Session() { Period = period };
            }
        }

        private static Session[,] DeepEmptyCopy(this Session[,] Sessions)
        {
            int iLength = Sessions.GetLength(0), jLength = Sessions.GetLength(1);
            Session[,] _Sessions = new Session[iLength, jLength];
            for (int i = 0; i < iLength; i++)
            {
                for (int j = 0; j < jLength; j++)
                    if (Sessions[i, j] != null)
                        _Sessions[i, j] = new Session()
                        {
                            Period = new int[] { i + 1, j + 1 }
                        };
            }

            return _Sessions;
        }
    }
}
