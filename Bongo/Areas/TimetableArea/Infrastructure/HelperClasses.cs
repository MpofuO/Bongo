using Bongo.Areas.TimetableArea.Models;

namespace Bongo.Areas.TimetableArea.Infrastructure
{
    public class ModuleData
    {
        public string ModuleCode { get; set; }
        public List<string> moduleData { get; set; } = new List<string>();
    }
    public class Lecture
    {
        public string ModuleCode { get; set; }
        public string LectureDesc { get; set; }
        public bool isFirstSemester => int.Parse(ModuleCode.Substring(6, 1)) == 0 || ModuleCode[6] % 2 == 1;
        public bool isSecondSemester => ModuleCode[6] % 2 == 0;
        public List<Session> sessions { get; set; } = new List<Session>();
    }
    public static class Periods
    {
        public static int[] GetPeriod(string time, string day)
        {
            int[] period = new int[2];

            switch (day.ToUpper())
            {
                case "MONDAY": period[0] = 1; break;
                case "TUESDAY": period[0] = 2; break;
                case "WEDNESDAY": period[0] = 3; break;
                case "THURSDAY": period[0] = 4; break;
                case "FRIDAY": period[0] = 5; break;
            }
            period[1] = int.Parse(time.Substring(0, 2)) - 6;

            return period;
        }
    }
}
