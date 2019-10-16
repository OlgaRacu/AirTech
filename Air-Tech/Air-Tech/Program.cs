using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Air_Tech
{
    class Program
    {
        static string FileName = @"coding-assigment-orders.json";
        const int MaxBox = 20;
        const string DepartureCity = "YUL";

        enum Destination
        {
            None = 0,
            YYZ = 1,
            YYC = 2,
            YVR = 3
        }

        static void Main(string[] args)
        {
            int input = GetInput();

            var scheduleList = new List<FlightSchedule>(CreateFlightScheduleList(FileName));

            GetOutput(input, scheduleList);

            Console.ReadKey();
        }

        class FlightSchedule
        {
            public string OrderName { get; set; }
            public int OrderNumber { get; set; }
            public int FlightNumber { get; set; }
            public string DepartureCity { get; set; }
            public Destination ArrivalCity { get; set; }
            public int DayNumber { get; set; }
        }

        private static int GetInput()
        {
            int inputNumber;

            Console.WriteLine("Choose USER STORY #1 or USER STORY #2 entering number 1 or 2 and click ENTER: ");

            while (true)
            {
                string inputString = Console.ReadLine();
                bool isInteger = Int32.TryParse(inputString, out inputNumber);

                if (!isInteger || (inputNumber != 1 && inputNumber != 2))
                    Console.WriteLine("Wrong input. Enter number 1 or 2: ");
                else
                    break;
            }

            return inputNumber;
        }

        private static List<FlightSchedule> CreateFlightScheduleList(string fileName)
        {
            int count = 0, countBoxYYZ = 0, countBoxYYC = 0, countBoxYVR = 0;
            var scheduleList = new List<FlightSchedule>();

            var values = GetJsonValues(fileName);

            foreach (KeyValuePair<string, object> pair in values)
            {
                var dayNumber = 1;
                var flightNumber = 0;
                var destination = GetDestination(pair.Value.ToString());

                count++;

                switch (destination)
                {
                    case Destination.YYZ:
                        {
                            flightNumber = (int)destination;
                            countBoxYYZ++;
                            
                            if ((countBoxYYZ - 1) / MaxBox >= 1)
                            {
                                dayNumber = (countBoxYYZ - 1) / MaxBox + 1;

                                flightNumber += (dayNumber - 1) * 3;
                            }

                            break;
                        }

                    case Destination.YYC:
                        {
                            flightNumber = (int)destination;
                            countBoxYYC++;

                            if ((countBoxYYC - 1) / MaxBox >= 1)
                            {
                                dayNumber = (countBoxYYZ - 1) / MaxBox + 1;

                                flightNumber += (dayNumber - 1) * 3;
                            }

                            break;
                        }
                    case Destination.YVR:
                        {
                            flightNumber = (int)destination;
                            countBoxYVR++;

                            if ((countBoxYVR - 1) / MaxBox >= 1)
                            {
                                dayNumber = (countBoxYYZ - 1) / MaxBox + 1;

                                flightNumber += (dayNumber - 1) * 3;
                            }

                            break;
                        }
                    default:
                        {
                            destination = Destination.None;

                            break;
                        }
                }

                scheduleList.Add(new FlightSchedule
                {
                    OrderName = pair.Key,
                    OrderNumber = count,
                    FlightNumber = flightNumber,
                    DepartureCity = DepartureCity,
                    ArrivalCity = destination,
                    DayNumber = dayNumber
                });
            }

            return scheduleList;
        }

        private static Destination GetDestination(string destination)
        {
            try
            {
                var splitDestination = destination.Split(':')[1];
                var arrivalCity = Regex.Matches(splitDestination, @"([A-Z]+)").Cast<Match>().Select(m => m.Value).FirstOrDefault();

                return (Destination)Enum.Parse(typeof(Destination), arrivalCity, true);
            }
            catch (Exception)
            {
                return Destination.None;
            }
        }


        private static Dictionary<string, object> GetJsonValues(string path)
        {
            try
            {
                var json = File.ReadAllText(path);
                var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                return values;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }


        private static void GetOutput(int input, List<FlightSchedule> scheduleList)
        {
            string output;
            
            if (input == 1)
            {
                var scheduleListOrder = scheduleList.OrderBy(x => x.FlightNumber);

                foreach (var schedule in scheduleListOrder)
                {
                    if (schedule.FlightNumber != 0)
                    {
                        output = String.Format("Flight: {0}, departure: YUL, arrival: {1}, day: {2}", schedule.FlightNumber, schedule.ArrivalCity, schedule.DayNumber);

                        Console.WriteLine(output);
                    }    
                }
            }
            else
            {
                var scheduleListOrder = scheduleList.OrderBy(x => x.OrderNumber);

                foreach (var schedule in scheduleList)
                {
                    if (schedule.FlightNumber == 0)
                        output = String.Format("Order: {0}, flightNumber: not scheduled", schedule.OrderName);
                    else
                        output = String.Format("Order: {0}, flightNumber: {1}, departure: YUL, arrival: {2}, day: {3}", schedule.OrderName, schedule.FlightNumber, schedule.ArrivalCity, schedule.DayNumber);

                    Console.WriteLine(output);
                }
            }
        }
    }
}
