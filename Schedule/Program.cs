using System;

class Program
{

    // По сути, данная задача - это задача в рюкзаке, только с расписанием
    // Я решил выстроить таблицу со всеми значениями, таблица выглядет примерно так
    /*
     *  1 2 3 (строкой является часы)
     * 1
     * 2
     * 3
     * ( столбец является элементом таблицы из ТЗ, где 1 - Иссакиевский собор, 2 - Эрмитраж, 3 - Кунсткамера и т д
     */

    static void Main()
    {
        Schedule[] schedules = Input();
        int countPlace = 20, maxTime = 32;

        Table[,] table = new Table[countPlace + 1, maxTime + 1];

        CycleFunction(schedules, table, countPlace, maxTime);

        Output(table);
    }

    // В данной функции вводим значения
    static Schedule[] Input()
    {
        return new Schedule[] 
        { 
            new Schedule("Исаакиевский собор", 5, 10), 
            new Schedule("Эрмитаж", 8, 11), 
            new Schedule("Кунсткамера", 3.5, 4), 
            new Schedule("Петропавловская крепость", 10, 7), 
            new Schedule("Ленинградский зоопарк", 9, 15), 
            new Schedule("Медный всадник", 1, 17), 
            new Schedule("Казанский собор", 4, 3), 
            new Schedule("Спас на Крови", 2, 9), 
            new Schedule("Зимний дворец Петра I", 7, 12), 
            new Schedule("Зоологический музей", 5.5, 6), 
            new Schedule("Музей обороны и блокады Ленинграда", 2, 19), 
            new Schedule("Русский музей", 5, 8), 
            new Schedule("Навестить друзей", 12, 20), 
            new Schedule("Музей восковых фигур", 2, 13), 
            new Schedule("Литературно-мемориальный музей Ф.М. Достоевского", 4, 2), 
            new Schedule("Екатерининский дворец", 1.5, 5), 
            new Schedule("Петербургский музей кукол", 1, 14), 
            new Schedule("Музей микроминиатюры «Русский Левша»", 3, 18), 
            new Schedule("Всероссийский музей А.С. Пушкина и филиалы", 6, 1), 
            new Schedule("Музей современного искусства Эрарта", 7, 16) 
        };
    }

    // Функция, для вывода значений, решил добавить команду для вывода всей таблицы и для вывода оптимального решения
    static void Output(Table[,] table, int countPlace = 20, int maxTime = 32)
    {
        bool flag = true;
        while (flag)
        {
            Console.WriteLine("Режимы вывода: \n1 - Вывести таблицу\n2 - Оптимальный вариант\n3 - Выход");
            int mod;
            if (!int.TryParse(Console.ReadLine(), out mod))
            {
                Console.WriteLine("Неправильный ввод!");
                continue;
            }
               

            switch (mod) {
                case 1:
                    // Не получилось сделать более красивый вывод таблицы, пришлось довольствоваться, чтобы каждый элемент таблицы выводился с новой строки
                    for (int i = 0; i < countPlace; i++)
                    {
                        for (int j = 0; j < maxTime; j++)
                        {
                            Console.WriteLine(table[i, j].Output());
                        }
                        Console.WriteLine();
                    }
                    break;
                case 2:
                    Console.WriteLine(table[countPlace - 1, maxTime - 1].Output());
                    break;
                case 3:
                    flag = false;
                    break;
                default:
                    Console.WriteLine("Такой команды не существует!");
                    break;
            }
        }
    }

    // Решил разделить циклы и основной алгоритм
    // Здесь лишь цикл для прохождения и заполнения всех элементов таблицы
    static void CycleFunction(Schedule[] schedules, Table[,] table, int countPlace, int maxTime)
    {
        for (int i = 0; i < countPlace + 1; i++)
        {
            for (int j = 0; j < maxTime + 1; j++)
            {
                table[i, j] = CaseProcessing(schedules, i, j, table);
            }
        }
    }

    // Основной цикл для выстраивания таблицы, сначало хотел не добавлять весь массив schedules и table, но тогда пришлось бы увеличить количество входных данных
    // Этот алгоритм сложно объяснить в комментариях, чем на словах, но я попробую
    static Table CaseProcessing(Schedule[] schedules, int i, int j, Table[,] table)
    {
        // Самую верхнюю строку и самый левый столбец заполняем 0
        if (i == 0 || j == 0)
        {
            return new Table(new Schedule[] { }, 0);
        }
        // Если же i == 1 (вторая верхняя строка), заполняем сначало 0 до определённого момента (пока часы элемента Schedules не станут больше часов из table), не знаю как лучше всего объяснить 
        else if (i == 1)
        {
            return schedules[i - 1].GetTime() <= j ? new Table(new Schedule[] { schedules[i - 1] }, Convert.ToInt32(schedules[i].GetTime())) : new Table(new Schedule[] { }, 0);
        }
        else
        {
            // Проверяем если текущие часы элемента schedules не помещаются в текущие часы table, то записываем прошлый элемент table в текущий
            if (schedules[i - 1].GetTime() > j)
            {
                return table[i - 1, j];
            }
            else
            {
                // Создаем новый point из суммы текущих часов элемента table с текущими часами элемента Schedules 
                double point = schedules[i - 1].GetTime() + table[i - 1, j - Convert.ToInt32(schedules[i - 1].GetTime())].point;
                // Если новый point больше прошлого в таблице, то true, в ином случае false
                bool flag = point > table[i - 1, j].point;

                if (flag)
                    // Создаем в текущим table новый элемент, где добавляем суммируем из предыдущего элемента все имена, часы и приорететы с новым элементом Schedules 
                    return new Table(new Schedule[] { schedules[i - 1] }.Concat(table[i - 1, Convert.ToInt32(j - schedules[i - 1].GetTime())].schedule).ToArray(), point);
                else
                    // В ином случае записываем в текущий прошлый элемент table
                    return table[i - 1, j];
            }
        }
    }
}

// Класс Расписание
public class Schedule
{
    private string _name;

    private double _time;

    private int _priority;

    // Конструктор для быстрой записи элементов
    public Schedule(string name, double time, int priority)
    {
        _name = name;
        _time = time;
        _priority = priority;
    }

    // Функции для получения элементов класса
    public string GetName() => _name;
    public double GetTime() => _time;
    public int GetPriority() => _priority;

}

// Класс Таблица
public class Table
{
    public Schedule[] schedule;
    public double point;

    // Конструктор
    public Table (Schedule[] schedule, double point)
    {
        this.schedule = schedule;
        this.point = point;
    }

    // Функция для удобного вывода названий мест и их общее количество часов
    public string Output()
    {
        return schedule == null ? "" : string.Join(" + ", schedule.Select(i => i.GetName())) + " - " + point;
    }
}

// Думаю, что в данном коде есть возможные костыли, которые могут не правильно вычислять саму таблицу, но надеюсь, что таблица выстроена правильно
// Был бы признателен, если бы указали на возможные ошибки в коде



