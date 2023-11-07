using System.Text.RegularExpressions;
using Bongo.Areas.TimetableArea.Models;

namespace Bongo.Areas.TimetableArea.Infrastructure
{

    public class TimetableExtractor
    {
        #region Private Lists
        public static List<Session> lstSessions;
        public static List<string> timetableLines;
        public static List<ModuleData> modulesData;
        public static List<Lecture> Lectures;
        #endregion

        public TimetableExtractor()
        {
            lstSessions = new List<Session>();
            timetableLines = new List<string>();
            modulesData = new List<ModuleData>();
            Lectures = new List<Lecture>();

        }

        public List<Session> ExtractSessions(string text, out List<Lecture> lectures, out List<string> moduleCodes)
        {
            ReadTextToList(text);
            ExtractModules();
            ExtractModuleData();
            ExtractLectures();
            ExtractSessions();

            lectures = Lectures;
            moduleCodes = new List<string>(modulesData.Select(md => md.ModuleCode));
            return lstSessions;
        }
        private static void ReadTextToList(string text)
        {
            //Take all the lines in the text and put them into a list
            timetableLines = text.Split('\n').ToList();

        }
        private static void ExtractModules()
        {
            //GetSessionsArray the list of modulesData
            Regex modulepattern = new Regex(@"[A-Z]{4}[\d]{4}|CLASH!![\d]");
            for (int i = 0; i < timetableLines.Count; i++)
            {
                Match match = modulepattern.Match(timetableLines[i]);
                if (match.Success)
                    modulesData.Add(new ModuleData { ModuleCode = match.Value });
            }
        }
        private static void ExtractModuleData()
        {
            //Add the sessions details
            for (int i = 0; i < modulesData.Count; i++)
            {
                int startIndex = timetableLines.IndexOf(modulesData[i].ModuleCode) + 1;
                int endIndex = i == modulesData.Count - 1 ? timetableLines.Count - 1 : timetableLines.IndexOf(modulesData[i + 1].ModuleCode);
                for (int j = startIndex; j < endIndex; j++)
                {
                    modulesData[i].moduleData.Add(timetableLines[j]);
                }
                modulesData[i].moduleData = modulesData[i].moduleData.Distinct().ToList();
            }
        }
        private static void ExtractLectures()
        {
            //GetSessionsArray the list of lectures
            Regex lecturepattern = new Regex(@"Lecture [0-9]?|Tutorial [0-9]?|Practical [0-9]?");
            foreach (ModuleData module in modulesData)
                for (int i = 0; i < module.moduleData.Count; i++)
                {
                    Match match = lecturepattern.Match(module.moduleData[i]);
                    if (match.Success)
                        Lectures.Add(new Lecture { ModuleCode = module.ModuleCode, LectureDesc = match.Value });
                }
        }
        private static void ExtractSessions()
        {
            Regex timepattern = new Regex(@"[0-9]{2}:[0-9]{2} [0-9]{2}:[0-9]{2}");
            Regex daypattern = new Regex(@"Monday|Tuesday|Wednesday|Thursday|Friday");
            Regex lecturepattern = new Regex(@"Lecture [0-9]?|Tutorial [0-9]?|Practical [0-9]?");

            foreach (Lecture lect in Lectures)
            {
                ModuleData module = modulesData.FirstOrDefault(m => m.ModuleCode == lect.ModuleCode);
                int startIndex = module.moduleData.IndexOf(lect.LectureDesc);
                for (int i = startIndex + 1; i < module.moduleData.Count; i++)
                {
                    Match match = lecturepattern.Match(module.moduleData[i]);
                    if (!match.Success)
                    {
                        Match timeMatch = timepattern.Match(module.moduleData[i]);
                        Match dayMatch = daypattern.Match(module.moduleData[i]);

                        if (timeMatch.Success && dayMatch.Success)
                        {
                            Session session = new Session
                            {
                                ModuleCode = lect.ModuleCode,
                                sessionType = lect.LectureDesc,
                                sessionInPDFValue = module.moduleData[i],
                                Venue = module.moduleData[i].Substring(0, timeMatch.Index - 1),
                                Period = Periods.GetPeriod(timeMatch.Value, dayMatch.Value)
                            };

                            lect.sessions.Add(session);
                        }
                    }
                    else
                        break;
                }
            }
        }
    }
}
