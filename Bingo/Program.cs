using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bingo
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] bingo2d = new int[5, 5];
            int[] randNum = new int[] { };
            int[] bingoMark = new int[] { };
            string input = "";
            bool answer = false;
            bool again = true;
            bool isValid = false;

            while (again)
            {
                GenerateBingoTable(ref randNum, bingo2d);

                PrintBingoTable(bingo2d);

                ValidateInput(ref input, ref bingoMark, ref isValid);

                MarkBingoCell(bingoMark, bingo2d);

                PrintBingoTable(bingo2d);

                CheckAnswer(bingo2d, answer);

                PlayAgain(ref input, ref again, ref isValid);
            }
        }

        delegate int[] runNumber(int from, int to);
        
        static readonly Func<string, int[]> _split = i => i.Split(',').Select(a => Convert.ToInt32(a)).ToArray();
        
        static readonly Func<string, bool> _trysplit = i => i.Split(',').Any(s => int.TryParse(s, out int number));
        
        public static void ValidateInput(ref string input, ref int[] bingoMark, ref bool isValid)
        {
            while (!isValid)
            {
                Console.WriteLine("########## Validate ##########");
                Console.WriteLine("Count of numbers should more than 5 numbers");
                Console.WriteLine("Value of Numbers must between 1-25 and do not repeated");
                Console.WriteLine("Not alphabets or special character except brackets");
                Console.WriteLine("Split each Number by commas (,)");
                Console.WriteLine("Example input: 1,2,3,4,5 or [1, 2, 3, 4, 5]");
                Console.WriteLine("Please enter bingo input numbers");

                input = Console.ReadLine();
                input = input.Replace("]", "").Replace("[", "").Trim();

                try
                {
                    ValidateBingoInput validate = new ValidateBingoInput(input, ref isValid);

                    if (!isValid)
                    {
                        Console.WriteLine("Invalid input");
                        
                        isValid = false;
                        continue;
                    }

                    bingoMark = _split(input);
                }
                catch
                {
                    Console.WriteLine("Invalid input");
                    
                    isValid = false;
                    continue;
                }

                isValid = true;
            }
        }

        public class ValidateBingoInput
        {
            public ValidateBingoInput(string input, ref bool isValid)
            {
                isValid = Validation(input, isValid);
            }

            public bool Validation(string input, bool isValid)
            {
                int[] checkNumber = _split(input);

                if (Regex.IsMatch(input, @"^[1-9\,\ ]+$"))
                {
                    isValid = true;
                }

                if (_trysplit(input))
                {
                    isValid = true;
                    List<int> uniqueNumber = new List<int>();
                    
                    foreach (int item in checkNumber)
                    {
                        if (item > 0 && item <= 25)
                        {
                            isValid = true;
                        }
                        else
                        {
                            return false;
                        }

                        if (!uniqueNumber.Contains(item))
                        {
                            isValid = true;
                        }
                        else
                        {
                            return false;
                        }

                        uniqueNumber.Add(item);
                    }
                }

                if (checkNumber.Length < 5)
                {
                    isValid = false;
                }

                return isValid;
            }
        }

        public static int[] FromTo(int from = 1, int to = 25)
        {
            return Enumerable.Range(from, to).OrderBy(g => Guid.NewGuid()).Take(25).ToArray();
        }

        public static void PlayAgain(ref string input, ref bool again, ref bool isValid)
        {
            Console.WriteLine("Do you want to play again ? (Y/N)");
            
            input = Console.ReadLine();

            if (input.ToUpper() == "Y")
            {
                again = true;
                isValid = false;
            }
            else
            {
                again = false;
            }
        }

        public static void MarkBingoCell(int[] bingoMark, int[,] bingo2d)
        {
            Console.WriteLine("Mark Result with 0 Number");

            foreach (int mark in bingoMark)
            {
                for (int row = bingo2d.GetLowerBound(0); row <= bingo2d.GetUpperBound(0); ++row)
                {
                    for (int col = bingo2d.GetLowerBound(1); col <= bingo2d.GetUpperBound(1); ++col)
                    {
                        if (bingo2d[row, col] == mark)
                        {
                            bingo2d[row, col] = 0;
                        }
                    }
                }
            }
        }

        public static void GenerateBingoTable(ref int[] randNum, int[,] bingo2d)
        {
            runNumber runNumber = new runNumber(FromTo);
            randNum = runNumber(1, 25);

            Console.WriteLine("Generate/Random Bingo Table");

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    bingo2d[i, j] = randNum[i * 5 + j];
                }
            }
        }

        public static void PrintBingoTable(int[,] bingo2d)
        {
            for (int i = 0; i < bingo2d.GetLength(0); i++)
            {
                for (int j = 0; j < bingo2d.GetLength(1); j++)
                {
                    Console.Write("[ " + (string.Format("{0,2}", bingo2d[i, j])) + " ] \t");
                }
                
                Console.WriteLine();
            }
        }

        public static void CheckAnswer(int[,] bingo2d, bool answer)
        {
            Console.WriteLine("########## Result ##########");

            for (int i = 0; i < 5; i++)
            {
                if (GetRow(bingo2d, i).Sum() == 0)
                {
                    answer = true;
                }

                if (GetColumn(bingo2d, i).Sum() == 0)
                {
                    answer = true;
                }
            }

            if (answer)
            {
                Console.WriteLine("Bingo");
            }
            else
            {
                Console.WriteLine("Not Bingo");
            }

            Console.WriteLine("########## Result ##########");
        }

        public static IEnumerable<T> GetRow<T>(T[,] array, int row)
        {
            for (int i = 0; i <= array.GetUpperBound(1); ++i)
            {
                yield return array[row, i];
            }
        }

        public static IEnumerable<T> GetColumn<T>(T[,] array, int column)
        {
            for (int i = 0; i <= array.GetUpperBound(0); ++i)
            {
                yield return array[i, column];
            }
        }
    }
}
