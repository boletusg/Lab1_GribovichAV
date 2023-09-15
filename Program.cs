using System;
using System.IO;
using System.Text.RegularExpressions;

namespace lab1_registration
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            readData();
        }
        public static void readData()
        {
            Console.WriteLine("Придумайте логин (номер телефона/email/никнейм)");
            string login = Console.ReadLine();
            Console.WriteLine("Придумайте пароль");
            string password = Console.ReadLine();
            Console.WriteLine("Повторите пароль");
            string password2 = Console.ReadLine();
            Console.WriteLine(" ");

            WriteFile(login, password, password2);
        }
        public static void WriteFile(string login, string password, string password2)
        {
            bool number = ValidNumber(login);
            bool email = ValidEmail(login);
            bool nickname = ValidNickname(login);

            bool log = ValidationLogin(login, email, number, nickname);
            bool passwd = validationPasswd(password);
            bool repeat = repeatPasswd(password, password2);

            if(passwd == false)
            {
                Console.WriteLine("Пароль не соответствует формату");
            }

            if (log == true & passwd == true & repeat == true)
            {
                //сохранение нового логина в файл
                StreamWriter file = new StreamWriter("C:/Users/Настя/source/repos/lab1_registration/LoginList.txt", true);
                file.WriteLine(login);
                file.Close();
                Console.WriteLine(" ");
                Console.WriteLine("Новый пользователь зарегистрирован");

                //запись данных в логи

            }           
            else
            {
                Console.WriteLine(" ");
                Console.WriteLine("Попробуйте еще раз");
                readData();
            }
        }

        public static bool ReadLoginList(string login)
        {
            String line;
            bool log = false;
            try
            {
                StreamReader sr = new StreamReader("C:/Users/Настя/source/repos/lab1_registration/LoginList.txt");
                line = sr.ReadLine();
                while (line != null)
                {
                    if (login == line)
                    {
                        log = true;
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return log;
        }
        public static bool ValidationLogin(string login, bool email, bool number, bool nickname)
        {
            //проверка на совпадение с существующими логинами
            bool saveLog = ReadLoginList(login);
            if (saveLog == false)
            {
                if (number == true || email == true || nickname == true)
                {
                    //передача логина для записи в файл
                    return true;

                }
                else
                {
                    Console.WriteLine("Логин не соответствует формату данных");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Пользователь с таким логином уже существует");
                return false;
            }
        }
        public static bool ValidNumber(string login)
        {
            // Создаем регулярное выражение для проверки номера телефона
            Regex regex = new Regex(@"^\+\d-\d{3}-\d{3}-\d{4}$");

            // Проверяем соответствие строки формату номера телефона
            bool number = regex.IsMatch(login);

            if (number == false)
            {
                //сообщение об ошибке формата телефона в консоль
                Console.WriteLine("Is it not number");
            }

            return number;
        }
        public static bool ValidEmail(string login)
        {
            // Паттерн для проверки соответствия формату email
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            // Создание экземпляра регулярного выражения с использованием паттерна
            Regex regex = new Regex(pattern);

            // Проверка соответствия строки паттерну
            bool email = regex.IsMatch(login);

            if (email == false)
            {
                //сообщение об ошибке формата email в консоль
                Console.WriteLine("Is it not email");
            }

            return email;
        }
        public static bool ValidNickname(string login)
        {
            string log = login;
            bool nickname;
            if (Regex.IsMatch(log, "^[A-Za-z0-9_]+$"))
            {
                nickname = true;
            }
            else
            {
                nickname = false;
            }
            if (!Regex.IsMatch(log, "[A-Za-z]"))
            {
                nickname = false;
            }
            if (!Regex.IsMatch(log, "[0-9]"))
            {
                nickname = false;
            }
            if (log.Length <= 5)
            {
                nickname = false;
            }
            

            if (nickname == false)
            {
                //сообщение об ошибке формата ника в консоль
                Console.WriteLine("Is it not nickname");
            }

            return nickname;
        }
        public static bool validationPasswd(string password)
        {
                // Проверка на минимальную длину
                if (password.Length < 7)
                {
                    return false;
                }

                // Проверка на наличие хотя бы одной буквы в верхнем регистре
                if (!Regex.IsMatch(password, "[А-Я]"))
                {
                    return false;
                }

                // Проверка на наличие хотя бы одной буквы в нижнем регистре
                if (!Regex.IsMatch(password, "[а-я]"))
                {
                    return false;
                }

                // Проверка на наличие хотя бы одной цифры
                if (!Regex.IsMatch(password, @"\d"))
                {
                    return false;
                }

                // Проверка на наличие хотя бы одного спецсимвола (допустимы только символы Юникода)
                if (!Regex.IsMatch(password, @"[\p{P}\p{S}]"))
                {
                    return false;
                }

                // Проверка на наличие только кириллицы, цифр и спецсимволов
                if (!Regex.IsMatch(password, @"^[\p{Ll}\p{Lu}\p{M}\p{N}\p{P}\p{S}]+$"))
                {
                    return false;
                }

                return true;
        }
        public static bool repeatPasswd(string password, string password2)
        {
            if(password == password2)
            {
                return true;
                //вывод успеха в файл
            }
            else
            {
                Console.WriteLine("Пароли не совпадают");
                return false;
                //вывод ошибки в файл
            }
        }

    }  
}
