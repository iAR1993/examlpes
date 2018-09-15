using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Task1
{
    class Box
    {
        public int startPoint;
        public int endPoint;
    }

    class Program
    {

        static BackgroundWorker bw = new BackgroundWorker();

        static int? PointReader(int[] range, int num)
        {
            int? point = null;
            Console.WriteLine($"Введите значение {num} : ");
            try
            {
                int tempPoint;
                bool result = Int32.TryParse(Console.ReadLine(), out tempPoint);
                if (result)
                {
                        point = tempPoint;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Значение не является целым числом");
            }
                return point;
        }

       static private double getStandardDeviation(List<int> someDoubles)
        {
            double average = someDoubles.Average();
            double sumOfSquaresOfDifferences = someDoubles.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / (someDoubles.Count - 1));
            return sd;
        }

        static private double getAverage(List<int> someDoubles)
        {
            double average = someDoubles.Average();
            return average;
        }

        static void Main(string[] args)
        {
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += (sender, e) => 
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                List<int> randoms = new List<int>();
                Random rnd = new Random();
                Box box = (Box)e.Argument;
                int practicalLimit = 268435456; // for 64bit
                while (true)
                {
                    int value = rnd.Next(box.startPoint, box.endPoint + 1);
                    
                    if (randoms.Count() < practicalLimit)
                    {
                        randoms.Add(value);
                    }
                    else
                    {
                        randoms.RemoveAt(0);
                        randoms.Add(value);

                    }
   
                    if (worker.CancellationPending == true)
                    {
                        e.Result = randoms;
                        break;
                    }
                }
            };

            bw.RunWorkerCompleted += (sender, e) =>
            {
                List<int> intList = e.Result as List<int>;
                Console.WriteLine("Среднее арифметическое: " + Math.Round(getAverage(intList), 2));
                Console.WriteLine("Стандартное отклонение: "+ Math.Round(getStandardDeviation(intList), 2));

            };

            int[] range = new int[2] { int.MinValue, int.MaxValue };

            Box boxInstance = new Box();

            int? tempFirstPoint = PointReader(range, 1);
            int? tempSecondPoint = PointReader(range, 2);


            if ((tempFirstPoint != null) && (tempSecondPoint != null))
            {
                if (tempFirstPoint < tempSecondPoint)
                {
                    boxInstance.startPoint = tempFirstPoint.Value;
                    boxInstance.endPoint = tempSecondPoint.Value;
                }
                else
                {
                    boxInstance.startPoint = tempSecondPoint.Value;
                    boxInstance.endPoint = tempFirstPoint.Value;
                }
            }
            else
                Console.WriteLine("Ошибка");

            bw.RunWorkerAsync(boxInstance);

            while (Console.ReadKey(true).Key != ConsoleKey.Enter)
            {
                Console.WriteLine("Для начала вычеслений нажмите enter");

            }
            Console.WriteLine("Идет вычисление...");
            bw.CancelAsync();

            Console.ReadKey();


        }
    }
}
