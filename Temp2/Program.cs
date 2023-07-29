using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Temp2
{
    class Field
    {
        
        private int[,] field; 
        private int fieldWidth, fieldHeight;
        private int stepNumber;
        public Field(int width, int height)
        {
            //сделать тут проверку входных данных
            fieldHeight = height;
            fieldWidth = width;
            field = new int[fieldHeight, fieldWidth];
        }
        public void Play()
        {
            stepNumber = 0;
            ConsoleKeyInfo move, gameEnding;
            FillStartCells(); 
            while (!EndOfGame())
            {
                PrintField();
                move = Console.ReadKey(true);
                stepNumber++;
                if (KeyCorrect(move.Key))
                {
                    if (CanSwipe(move.Key)) SlideAndFoldCells(move.Key);
                    else 
                    {
                        Console.Clear();
                        continue; 
                    }
                }
                else
                {
                    if (move.Key == ConsoleKey.Escape)
                    {
                        Console.Clear();
                        ClearField();
                        return;
                    }
                    Console.WriteLine("Нажмите на правильную клавишу!");
                    Thread.Sleep(1500);
                    Console.Clear();
                    continue;
                }
                Console.Clear();
                FillRandomCells();
            }
            Console.WriteLine("Вы проиграли! Было сделано {stepNumber} ходов. Хотите начать заново? Y - Да");
            stepNumber = 0;
            gameEnding = Console.ReadKey(true);
            if (gameEnding.Key == ConsoleKey.Y)
            {
                ClearField();
                Console.Clear();
                Play();
                return;
            }
        }
        private bool CanSwipe(in ConsoleKey direction)
        {
            switch (direction)
            {
                case ConsoleKey.W:
                    {
                        //если хотя бы перед одной ячейкой сверху есть
                        //свободное пространство, то return true;
                        for (int i = 1; i < fieldHeight; i++)
                            for (int j = 0; j < fieldWidth; j++)
                                //если клетка занята и сверху от неё есть пустое место:
                                //return true;
                                if ((CellFilled(field[i, j]) && field[i - 1, j] == 0) || (field[i,j] == field[i-1,j] && field[i,j] != 0)) return true;
                        return false;
                    }
                case ConsoleKey.A:
                    {
                        //если хотя бы перед одной ячейкой слева есть
                        //свободное пространство, то return true;
                        for (int i = 0; i < fieldHeight; i++)
                            for (int j = 1; j < fieldWidth; j++)
                                //если клетка занята и сверху от неё есть пустое место:
                                //return true;
                                if ((CellFilled(field[i, j]) && field[i , j - 1] == 0) || (field[i, j] == field[i , j-1] && field[i, j] != 0)) return true;
                        return false;
                    }
                case ConsoleKey.S:
                    {
                        //если хотя бы перед одной ячейкой есть
                        //свободное пространство внизу, то return true;
                        for (int i = 0; i < fieldHeight - 1; i++)
                            for (int j = 0; j < fieldWidth; j++)
                                //если клетка занята и сверху от неё есть пустое место:
                                //return true;
                                if ((CellFilled(field[i, j]) && field[i + 1, j] == 0) || (field[i, j] == field[i + 1, j] && field[i, j] != 0))return true;
                        return false;
                    }
                case ConsoleKey.D:
                    {
                        //если хотя бы перед одной ячейкой есть
                        //свободное пространство справо, то return true;
                        for (int i = 0; i < fieldHeight; i++)
                            for (int j = 0; j < fieldWidth - 1; j++)
                                //если клетка занята и сверху от неё есть пустое место:
                                //return true;
                                if ((CellFilled(field[i, j]) && field[i, j + 1] == 0) || (field[i, j] == field[i, j + 1] && field[i, j] != 0)) return true;
                        return false;
                    }
            }
            return false;
        }
        private bool KeyCorrect(in ConsoleKey key)
        {
            switch(key)
            {
                case ConsoleKey.W:
                case ConsoleKey.A:
                case ConsoleKey.S:
                case ConsoleKey.D:
                    return true;
                default:
                    return false;
            }
        }
        public void PrintField()
        {
            for (int i = 0; i < fieldHeight; i++)
            { 
                for (int j = 0; j < fieldWidth; j++)
                {
                    ChangeConsoleColor(in field[i, j]);
                    Console.Write(field[i, j] + "\t");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White ;
                }
                Console.WriteLine();
            }
        }
        private bool EndOfGame()
        {
            for (int i = 0; i < fieldHeight; i++)
            {
                for (int j = 0; j < fieldWidth; j++)
                {
                    if (field[i, j] == 0) return false;
                }
            }
            return true;
        }
        private void FillRandomCells() 
        {
            //добавляется всё время по одной ячейке - либо 2, либо 4
            //у 4 шанс 10%
            Random rand = new Random();
            int x = rand.Next(fieldHeight), y = rand.Next(fieldWidth);
            while(CellFilled(field[x,y])) { x = rand.Next(fieldHeight); y = rand.Next(fieldWidth);}
            if (rand.Next(10) == 9) field[x, y] = 4; else field[x, y] = 2;
        }
        private void FillStartCells()
        {
            Random random= new Random();
            int x1 = random.Next(fieldHeight), y1 = random.Next(fieldWidth);
            int x2 = random.Next(fieldHeight), y2 = random.Next(fieldWidth);
            while (x1 == x2 && y1 == y2) x1 = random.Next(fieldHeight);
            field[x1, y1] = 2;
            field[x2, y2] = 2;
        }
        private void ClearField()
        {
            for (int i = 0; i < fieldWidth; i++)
                for (int j = 0; j < fieldHeight; j++)
                    field[i, j] = 0;
        }
        private bool CellFilled(in int cell)
        {
            return cell == 0 ? false : true; 
        }

        
        private void SlideAndFoldCells(in ConsoleKey direction)
        {

            switch (direction)
            {
                case ConsoleKey.W:
                    {
                        int[] toInput = new int[fieldHeight];
                        for (int j = 0; j < fieldWidth; j++)
                        {
                            GetRowFromField(ConsoleKey.W, j, ref toInput);
                            ShiftElements(ref toInput);
                            PullElementsInField(ConsoleKey.W, j, toInput);
                        }
                        
                       
                        break;
                    }
                case ConsoleKey.A:
                    {
                        int[] toInput = new int[fieldWidth];
                        for (int i = 0; i < fieldHeight; i++)
                        {
                            GetRowFromField(ConsoleKey.A, i, ref toInput);
                            ShiftElements(ref toInput);
                            PullElementsInField(ConsoleKey.A, i, toInput);
                        }
                        break;
                    }
                case ConsoleKey.S:
                    {
                        int[] toInput = new int[fieldHeight];
                        for (int j = 0; j < fieldWidth; j++)
                        {
                            GetRowFromField(ConsoleKey.S, j, ref toInput);
                            ShiftElements(ref toInput);
                            PullElementsInField(ConsoleKey.S, j, toInput);
                        }
                        break;
                    }
                case ConsoleKey.D:
                    {
                        int[] toInput = new int[fieldWidth];
                        for (int i = 0; i < fieldHeight; i++)
                        {
                            GetRowFromField(ConsoleKey.D, i, ref toInput);
                            ShiftElements(ref toInput);
                            PullElementsInField(ConsoleKey.D, i, toInput);
                        }
                        break;
                    }
            }
        
        }
        private bool AdjancentCellsAreEqual(in int cell1,in int cell2)
        {
            return cell1 == cell2 ? true : false;
        }
            
        private void ShiftElements(ref int[] line)
        {
            int zeroIndex = 0;//до этого индекса все переменные отсортированы.
            int val1 = 0, val2 = 0, findedIndex = 0;
            bool zeroBetweenElements = false;
            while(zeroIndex + 1 < line.Length)
            {
                while (zeroIndex + 1 < line.Length && line[zeroIndex] == 0)
                {
                    if(CanFindValueAfterZeroIndex(line, zeroIndex))
                    {
                        SwipeAllElementsOnce(ref line, zeroIndex);
                    }
                    else
                    {
                        return;
                    }
                }
                if (CanFind2ValuesInLine(ref val1,ref val2,ref findedIndex, in line, in zeroIndex,ref zeroBetweenElements))
                {
                    if(val1 == val2)
                    {
                        line[findedIndex] = 0;
                        line[zeroIndex]*=2;
                        zeroIndex++;
                        continue;
                    }
                    else
                    {
                        if (zeroBetweenElements)
                        {
                            line[zeroIndex + 1] = val2;
                            line[findedIndex] = 0;
                            zeroIndex++;
                            continue;
                        }
                        else
                        {
                            zeroIndex++;
                            continue;
                        }
                    }
                }
                else
                {
                    return;
                }


            }
        }
        /// <summary>
        /// Возвращаем массив так, что направление движения указывает на элемент с нулевым индексом
        /// </summary>
        /// <returns></returns>
        private void GetRowFromField(in ConsoleKey direction,in int rowOrColumnIndex, ref int[] input)        
        {
            switch(direction)
            {
                case ConsoleKey.W:
                    {
                        for (int i = 0; i < fieldHeight; i++)
                        {
                            input[i] = field[i, rowOrColumnIndex];
                        }
                        return;
                    }
                case ConsoleKey.A:
                    {
                        for (int j = 0; j < fieldWidth; j++)
                        {
                            input[j] = field[rowOrColumnIndex, j];
                        }
                        return;
                    }
                case ConsoleKey.S:
                    {
                        for (int i = 0; i < fieldHeight; i++)
                        {
                            input[i] = field[fieldHeight-i-1, rowOrColumnIndex];
                        }
                        return;
                    }
                case ConsoleKey.D:
                    {
                        for (int j = 0; j < fieldWidth; j++)
                        {
                            input[j] = field[rowOrColumnIndex, fieldWidth - j - 1];
                        }
                        break;
                    }
                default:
                    throw new Exception();
            }
        }
        private bool CanFind2ValuesInLine(ref int val1, ref int val2,ref int findedIndex, in int[] line,in int zeroIndex,ref bool zeroBetweenElements)
        {
            zeroBetweenElements = false;
            findedIndex = zeroIndex + 1;
            while (findedIndex < line.Length && line[findedIndex] == 0) { findedIndex++; zeroBetweenElements = true; }
            if (findedIndex >= line.Length) return false;
            val1 = line[zeroIndex];
            val2 = line[findedIndex];
            if (val2 != 0) return true;
            return false;
        }
        private void PullElementsInField(in ConsoleKey direction, in int rowOrColumnIndex, in int[] line)
        {
            switch (direction)
            {
                case ConsoleKey.W:
                    {
                        for (int i = 0; i < fieldHeight; i++)
                        {
                            field[i, rowOrColumnIndex] = line[i];
                        }
                        return;
                    }
                case ConsoleKey.A:
                    {
                        for (int j = 0; j < fieldWidth; j++)
                        {
                            field[rowOrColumnIndex, j] = line[j];
                        }
                        return;
                    }
                case ConsoleKey.S:
                    {
                        for (int i = 0; i < fieldHeight; i++)
                        {
                            field[i, rowOrColumnIndex] = line[fieldHeight - i - 1];
                        }
                        return;
                    }
                case ConsoleKey.D:
                    {
                        for (int j = 0; j < fieldWidth; j++)
                        {
                            field[rowOrColumnIndex, j] = line[fieldWidth - j - 1];
                        }
                        return;
                    }
                default:
                    throw new Exception("PullElementsInField");
            }
        }
        
        private void SwipeAllElementsOnce(ref int[] line, in int zeroIndex)
        {
            for (int i = zeroIndex + 1; i < line.Length; i++)
            {
                line[i - 1] = line[i];
                line[i] = 0;
            }
        }
        private bool CanFindValueAfterZeroIndex(in int[] line, in int zeroIndex)
        {
            for (int i = zeroIndex + 1; i < line.Length; i++)
            {
                if (line[i] != 0) return true;
            }
            return false;
        }
        private void ChangeConsoleColor(in int cell)
        {
            switch (cell)
            {
                case 0:
                    {
                        break;
                    }
                case 2:
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    }
                case 4:
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    }
                case 8:
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    }
                case 16:
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        break;
                    }
                case 32:
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        break;
                    }
                case 64:
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        break;
                    }
                case 128:
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        break;
                    }
                case 256:
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        break;
                    }
                case 512:
                    {
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        break;
                    }
                case 1024:
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        break;
                    }
                case 2048:
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        break;
                    }
                case 4096:
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        break;
                    }
                case 8192:
                    {
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        break;
                    }
                case 16384:
                    {
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        break;
                    }
            }
        }
    }
    
    class Program
    {
        static private void GetRowsAndColumns(ref int rows, ref int columns)
        {
            Console.WriteLine("Введите количество строк в игровом поле: ");
            while (!int.TryParse(Console.ReadLine(), out rows)) 
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Вы неправильно ввели количество строк, попробуйте ещё раз!");
                Console.BackgroundColor = default;
            }
            Console.WriteLine("Введите количество столбцов в игровом поле:");
            while (!int.TryParse(Console.ReadLine(), out columns))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Вы неправильно ввели количество столбцов, попробуйте ещё раз!");
                Console.BackgroundColor = default;
            }
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("Данные получены!");
            Console.BackgroundColor = ConsoleColor.Black;
            Thread.Sleep(1500);
            Console.Clear();
        }
        static void Main(string[] args)
        {
            int rows = default, columns = default;
            GetRowsAndColumns(ref rows, ref columns);
            Field game = new Field(rows, columns);
            game.Play();
        }
    }
}
/*Ikimono-gakari
 *Текст песни Blue Bird / Aoi aoi ano sora
 * 
 * 
 * Habataitara modora nai to itte
Mezashita no ha aoi aoi ano sora

Kanashimi ha mada oboerezu
Setsunasa ha ima tsukami hajimeta
Anata he to idaku kono kanjou mo
Ima "kotoba" ni kawatteiku

Michinaru sekai no yume kara mezamete
Kono hane wo hiroge tobitatsu

Habataitara modora nai to itte
Mezashita no ha shiroi shiroi ano kumo
Tsukinuketara mitsukaru to shitte

Furikiru hodo aoi aoi ano sora
Aoi aoi ano sora
Aoi aoi ano sora

Aisozukita you na oto de sabireta furui mado ha kowareta

Miakita kago ha hora suteteiku furikaeru koto ha mou nai
Takanaru kodou ni kokyuu wo azukete
Kono mado wo kette tobitatsu

Kakedashitara te ni dekiru to itte
Izanau no ha tooi tooi ano koe
Mabushii sugita anata no te mo nigitte
Motomeru hodo aoi aoi ano sora

Ochiteiku to wakatteita sore de mo hikari wo oi tsuzuketeiku yo

Habataitara modora nai to itte
Sagashita no ha shiroi shiroi ano kumo
Tsukinuketara mitsukaru to shitte
Furikiru hodo aoi aoi ano sora
aoi aoi ano sora
aoi aoi ano sora*/