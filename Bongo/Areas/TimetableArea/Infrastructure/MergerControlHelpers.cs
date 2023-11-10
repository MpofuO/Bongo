using Bongo.Areas.TimetableArea.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.RegularExpressions;

namespace Bongo.Areas.TimetableArea.Infrastructure
{
    public static class MergerControlHelpers
    {
        static Regex timepattern = new Regex(@"[0-9]{2}:[0-9]{2} [0-9]{2}:[0-9]{2}");
        static Regex daypattern = new Regex(@"Monday|Tuesday|Wednesday|Thursday|Friday");
        public static void SplitSessions(this Session[,] Sessions)
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

            Sessions = _Sessions;
        }
        public static void HandleClashes(Session[,] Sessions, List<List<Session>> clashes)
        {
            foreach (var clash in clashes)
            {
                string day = daypattern.Match(clash[0].sessionInPDFValue).Value;
                int minHour = 0, maxHour = 0;
                foreach (var session in clash)
                {
                    int[] hourRange = getHourRange(session.sessionInPDFValue);

                    if (hourRange[0] < minHour)
                        minHour = hourRange[0];
                    if (hourRange[1] > maxHour)
                        maxHour = hourRange[1];
                }

                SplitRangeHourly(Sessions, minHour, maxHour, day);

            }
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
            for (int i = minHour; i <= maxHour; i++)
            {
                string hour = i < 10 ? $"0{i}" : $"{i}";

                int[] period = Periods.GetPeriod(hour, day);
                Sessions[period[0], period[1]] = new Session() { Period = period };
            }
        }
        public static void HandleGroups(Session[,] Sessions, List<Lecture> groups)
        {

        }

        private static Session[,] DeepEmptyCopy(this Session[,] Sessions)
        {
            int iMax = Sessions.GetLength(0), jMax = Sessions.GetLength(1);
            Session[,] _Sessions = new Session[iMax, jMax];
            for (int i = 0; i < iMax; i++)
            {
                for (int j = 0; j < jMax; j++)
                    if (Sessions[i, j] != null)
                        _Sessions[i, j] = new Session()
                        {
                            Period = new int[] { i, j }
                        };
            }

            return _Sessions;
        }
    }
}
