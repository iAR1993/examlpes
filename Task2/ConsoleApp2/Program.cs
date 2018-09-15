using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Task2
{
    class Activity
    {
        public int id;
        public int volume;
        public DateTime startTime;
        public DateTime endTime;
    }

    class Pairs
    {
        public List<Activity> activities;
        public int sumVolume;
    }

    class Program
    {
        static List<Activity> tryParseText(string text)
        {
            List<Activity> activityList = new List<Activity>();
            Regex regex = new Regex(@"\d*;\d{2}.\d{2}.\d{2} \d{2}:\d{2}:\d{2};\d{2}.\d{2}.\d{2} \d{2}:\d{2}:\d{2};\d*");
            MatchCollection matches = regex.Matches(text);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    string[] separator = { ";" };
                    string[] values = match.Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    Activity activityInstance = new Activity();
                    activityInstance.id = Convert.ToInt32(values[0]);
                    activityInstance.startTime = GetTime(values[1]);
                    activityInstance.endTime = GetTime(values[2]);
                    activityInstance.volume = Convert.ToInt32(values[3]);
                    activityList.Add(activityInstance);
                }
            }
            return activityList;
        }

        static public DateTime GetTime(string TimeString)
        {
            string[] separators = { " ", ".", ":" };
            string[] values = TimeString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            DateTime dateTime = new DateTime(Convert.ToInt32("20" + values[2]), Convert.ToInt32(values[1]), Convert.ToInt32(values[0]),
                Convert.ToInt32(values[3]), Convert.ToInt32(values[4]), Convert.ToInt32(values[5]));
            return dateTime;
        }

        static public List<Pairs> getPairs(List<Activity> activityList)
        {
            List<Pairs> pairsList = new List<Pairs>();
            for (int i = 0; i < activityList.Count; i++)
            {
                Pairs pair = new Pairs();
                Activity iActivity = activityList[i];
                pair.activities = new List<Activity>();
                pair.activities.Add(iActivity);
                int sumvolume = iActivity.volume;
                for (int j = activityList.Count - 1; j >= i; j--)
                {
                    Activity jActivity = activityList[j];
                    if (iActivity.id == jActivity.id)
                        break;

                    bool intersect = !(jActivity.endTime < iActivity.startTime || jActivity.startTime > iActivity.endTime);
                    if (intersect)
                    {
                        sumvolume += jActivity.volume;
                        pair.activities.Add(jActivity);
                    }

                }
                pair.sumVolume = sumvolume;
                pairsList.Add(pair);
            }
            return pairsList;
        }

        static public void SearchMaxVolume(List<Pairs> pairsList)
        {
            var listVolumes = pairsList.Select(maxVolume => maxVolume.sumVolume);
            var max = listVolumes.Max();

            foreach (var pair in pairsList)
            {
                if (pair.sumVolume == max)
                {
                    List<DateTime> allTimes = new List<DateTime>();
                    foreach (var activ in pair.activities)
                    {
                        allTimes.Add(activ.startTime);
                        allTimes.Add(activ.endTime);
                    }
                    string ids = " ";
                    foreach (var activ in pair.activities)
                    {
                        ids += activ.id.ToString() + "; ";
                    }
                    var minTime = allTimes.Min();
                    var maxTime = allTimes.Max();

                    Console.WriteLine("Максимальный объем :" + max.ToString());
                    Console.WriteLine($"Был произведен в период c {minTime} до {maxTime}");
                    Console.WriteLine("Пересекаются записи со след. id: " + ids);
                }
            }
        }

        static void Main(string[] args)
        {
            string pathSource = @"D:\file.csv";
            FileStream fs = new FileStream(pathSource, FileMode.Open);
            long size = new FileInfo(pathSource).Length;
            byte[] mybytes = new byte[size];
            for (int i = 0; i < size; i++)
            {
                mybytes[i] = (byte)fs.ReadByte();
            }

            string text = Encoding.Default.GetString(mybytes);
            fs.Dispose();

            List<Activity> activityList = tryParseText(text);
            List<Pairs> pairsList = getPairs(activityList);
            SearchMaxVolume(pairsList);
            Console.ReadKey();
        }
    }
}
