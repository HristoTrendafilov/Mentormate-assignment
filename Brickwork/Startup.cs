using Common;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Brickwork
{
    class Startup
    {
        static void Main(string[] args)
        {
            // the result for the brickwork of Layer 2
            var result = new StringBuilder();

            // numbers of which we build the multidimensional array
            var dimensions = Console.ReadLine()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            // number of rows
            var n = dimensions[0];
            // number of cols
            var m = dimensions[1];

            try
            {
                ValidateAreaSize(n, m);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }

            // build the multidimensional array with the given numbers for rows and cols
            var firstLayer = BuildFirstLayer(n, m);

            // add the brickwork nubers from layer 1, so we can use them in layer 2 
            // if brickwork is incorrect, the method throws ArgumentException and we exit the app
            try
            {
                 AddBrickworkNumbers(firstLayer);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }

            // the "try-catch" did'nt throw exception and here we have the consecutive numbers from Layer 1
            var numbers = AddBrickworkNumbers(firstLayer);

            for (int row = 0; row < firstLayer.GetLength(0); row++)
            {
                // this stringbuilder keeps the numbers for the second row of the brickwork 
                // so we can add them later to the final stringbuilder
                var secondRowSb = new StringBuilder();

                for (int col = 0; col < firstLayer.GetLength(1); col++)
                {
                    // below we keep the current row-col number and the next row-col number 
                    // so we can use them if its the last index of the area

                    // the number at current row-col
                    var currentRowCol1 = firstLayer[row, col];
                    // the number at next row-col
                    var nextRowCol1 = firstLayer[row + 1, col];

                    // here we are at the final index of the area and we add the last 2 numbers
                    if (col == firstLayer.GetLength(1) - 1)
                    {
                        result.Append($"|{currentRowCol1}|");
                        secondRowSb.Append($"|{nextRowCol1}|");
                        break;
                    }

                    // the number at current row with col + 1
                    var currentRowCol2 = firstLayer[row, col + 1];
                    // the number at the next row with col + 1
                    var nextRowCol2 = firstLayer[row + 1, col + 1];

                    // below we keep the first two consecutive numbers so we can use them if there is a brick on the Layer 1

                    // first number from the numbers stack
                    var firstNumberToInsert = numbers.Pop();
                    // second number from the numbers stack
                    var secondNumberToInsert = numbers.Pop();

                    // check if the layer 1 has a brick
                    var isBrick = CheckForBrick(currentRowCol1, currentRowCol2, nextRowCol1, nextRowCol2);

                    if (isBrick)
                    {
                        result.Append($"|{firstNumberToInsert}|*");
                        secondRowSb.Append($"|{secondNumberToInsert}|*");
                    }
                    else
                    {
                        // third number from the numbers stack
                        var thirdNumberToInsert = numbers.Pop();
                        // fourth number from the numbers stack
                        var fourthNumberToInsert = numbers.Pop();

                        result.Append("|" + $"{firstNumberToInsert} {secondNumberToInsert}" + "|*");
                        secondRowSb.Append("|" + $"{thirdNumberToInsert} {fourthNumberToInsert}" + "|*");

                        // increase the col index because we added a brick
                        col += 1;
                        continue;
                    }
                }
                // increase the row index 
                row++;
                result.Append(Environment.NewLine);
                result.AppendLine(secondRowSb.ToString());
            }

            // print on the console the final result of the brickwork
            Console.WriteLine(result.ToString());
        }

        private static Stack AddBrickworkNumbers(int[,] area)
        {
            // storing the consecutive numbers from Layer 1
            var numbers = new Stack();

            for (int row = 0; row < area.GetLength(0); row++)
            {
                for (int col = 0; col < area.GetLength(1); col++)
                {
                    // the number at current row-col
                    var currentRowCol1 = area[row, col];
                    // the number at next row-col
                    var nextRowCol1 = area[row + 1, col];

                    if (col == area.GetLength(1) - 1)
                    {
                        if (currentRowCol1 != nextRowCol1)
                        {
                            throw new ArgumentException(GlobalExceptions.InvalidBrickwork);
                        }

                        numbers.Push(currentRowCol1);
                        numbers.Push(nextRowCol1);
                        break;
                    }

                    // the number at current row with col + 1
                    var currentRowCol2 = area[row, col + 1];
                    // the number at the next row with col + 1
                    var nextRowCol2 = area[row + 1, col + 1];

                    var isBrick = CheckForBrick(currentRowCol1, currentRowCol2, nextRowCol1, nextRowCol2);

                    if (isBrick)
                    {
                        numbers.Push(currentRowCol1);
                        numbers.Push(currentRowCol2);
                        numbers.Push(nextRowCol1);
                        numbers.Push(nextRowCol2);

                        col++;
                        continue;
                    }
                    else if (currentRowCol1 == nextRowCol1)
                    {
                        numbers.Push(currentRowCol1);
                        numbers.Push(nextRowCol1);
                    }
                    else
                    {
                        throw new ArgumentException(GlobalExceptions.InvalidBrickwork);
                    }
                }
                row++;
            }

            return numbers;
        }

        private static int[,] BuildFirstLayer(int n, int m)
        {
            var area = new int[n, m];

            for (int row = 0; row < area.GetLength(0); row++)
            {
                // the input numbers we add for each row of the area
                var numbersToAdd = Console.ReadLine()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();

                if(numbersToAdd.Length > m)
                {
                    Console.WriteLine(GlobalExceptions.NumbersOnRowExceeded);
                    Environment.Exit(-1);
                }

                var maxNumber = (n + m) / 2;
                var minNumber = 0;

                if(numbersToAdd.Any(x => x > maxNumber) || numbersToAdd.Any(x => x < minNumber))
                {
                    Console.WriteLine(GlobalExceptions.InvalidNumbersCount, row);
                    Environment.Exit(-1);
                }

                // this array adds the numbers to the correct positions in the area
                for (int col = 0; col < area.GetLength(1); col++)
                {
                    area[row, col] = numbersToAdd[col];
                }
            }

            return area;
        }

        private static bool CheckForBrick(int currentRowCol1, int currentRowCol2, int nextRowCol1, int nextRowCol2)
        {
            if (currentRowCol1 == currentRowCol2 && nextRowCol1 == nextRowCol2)
            {
                return true;
            }

            return false;
        }

        private static void ValidateAreaSize(int n, int m)
        {
            if (m > 100 || n > 100)
            {
                throw new ArgumentException(GlobalExceptions.ExceededRowsOrColsNumber);
            }

            if (n % 2 != 0 || m % 2 != 0)
            {
                throw new ArgumentException(GlobalExceptions.InvalidRowsOrColsNumber);
            }
        }
    }
}
