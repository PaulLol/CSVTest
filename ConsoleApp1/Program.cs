using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        public class User
        {
            [Name("Employee Name")]
            public string EmployeeName { get; set; }
            [Name("Date")]
            public string Date { get; set; }
            [Name("Work Hours")]
            public string WorkHours { get; set; }
        }

        public static void CSVFile()
        {
            // ReadCSVFile

            var lines = File.ReadAllLines("acme_worksheet.csv");
            var list = new List<User>();
            foreach (var line in lines)
            {
                var values = line.Split(',');
                var contact = new User()
                {
                    EmployeeName = values[0],
                    Date = values[1],
                    WorkHours = values[2]
                };
                list.Add(contact);
            }

            // Dividing an object into lists

            List<string> namestring = new List<string>();
            foreach (User ns in list)
            {
                namestring.Add(ns.EmployeeName);
            }
            namestring.RemoveAt(0);


            List<string> datestring = new List<string>();
            foreach (User dt in list)
            {
                datestring.Add(dt.Date);
            }
            datestring.RemoveAt(0);


            List<string> workhoursstring = new List<string>();
            foreach (User wh in list)
            {
                workhoursstring.Add(wh.WorkHours);
            }
            workhoursstring.RemoveAt(0);

            // Change format time

            string format = "MMM dd yyyy";
            IEnumerable<DateTime> mapped = datestring.Select(s => DateTime.ParseExact(s, format, CultureInfo.InvariantCulture));
            List<DateTime> datetimelist = mapped.ToList();

            var mainList = new List<User>();

            for (int i = 0; i < namestring.Count; i++)
            {
                var contact = new User()
                {
                    EmployeeName = namestring[i],
                    Date = datetimelist[i].ToString("yyyy-MM-dd"),
                    WorkHours = workhoursstring[i]
                };
                mainList.Add(contact);
            }

            // Unique datetime 

            var datelist = new List<string>();
            for (int i = 0; i < namestring.Count; i++)
            {
                datelist.Add(datetimelist[i].ToString("yyyy-MM-dd"));
            }

            datelist = datelist.Distinct().ToList();

            // Unique names

            namestring = namestring.Distinct().ToList();
            namestring.Sort();

            // Create final main list

            var newlist = new List<User>();

            for (int i=0;i<namestring.Count;i++)
            {
                var found = new User();
                var newlist2 = new List<User>();
                while (found != null) 
                {
                    found = mainList.FirstOrDefault(item => item.EmployeeName == namestring[i]);
                    if (found != null) 
                    {
                        mainList.Remove(found);
                        newlist2.Add(found);
                    }
                }

                for (int j = 0; j < datelist.Count; j++)
                {
                    found = newlist2.FirstOrDefault(item => item.Date == datelist[j]);
                    if (found == null)
                    {
                        newlist2.Add(new User()
                        {
                            EmployeeName = namestring[i],
                            Date = datelist[j],
                            WorkHours = "0"
                        });
                    }
                }

                foreach (User us in newlist2)
                {
                    newlist.Add(us);
                }
            }

            //int k = 1;
            //foreach (User us in newlist)
            //{
            //    Console.WriteLine(k+":{0} {1} {2}", us.EmployeeName, us.Date, us.WorkHours);
            //    k++;
            //}

            // WriteCSVFile

            var templist = newlist;
            try
            {
                StringBuilder csvcontent = new StringBuilder();
                string dateliststring = datelist[0];
                for (int i = 1; i < datelist.Count; i++)
                {
                    dateliststring = dateliststring + "," + datelist[i];
                }

                csvcontent.AppendLine("Name/Date," + dateliststring);

                for (int i = 0; i < namestring.Count; i++)
                {
                    var found = templist.FirstOrDefault(item => item.EmployeeName == namestring[i]);
                    string line = found.WorkHours;
                    var v = datelist.Count - 1;
                    templist.Remove(found);
                    while (v != 0)
                    {
                        found = templist.FirstOrDefault(item => item.EmployeeName == namestring[i]);
                        line = line + "," + found.WorkHours;
                        templist.Remove(found);
                        v--;
                    }

                    csvcontent.AppendLine(namestring[i] + "," + line);
                }

                File.AppendAllText("test.csv", csvcontent.ToString());
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error:", ex);
            }

        }

        static void Main(string[] args)
        {
            CSVFile();
            //Console.ReadLine();
        }
    }
}
