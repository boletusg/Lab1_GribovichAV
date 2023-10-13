using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Serilog;

namespace lab1_registration2
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File("log.txt")
                .CreateLogger();

            Log.Information("Запуск программы");

            readData();

            Log.Information("Завершенние программы");
            Log.CloseAndFlush();
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

            CheckRegistration(login, password, password2);
        }
        public static void CheckRegistration(string login, string password, string password2)
        {
            (bool, string) number = ValidNumber(login);


            (bool, string) email = ValidEmail(login);
            (bool, string) nickname = ValidNickname(login);

            (bool, string) log = ValidationLogin(login, email, number.Item1, nickname);
            (bool, string) passwd = validationPasswd(password);
            (bool, string) repeat = repeatPasswd(password, password2);

            if (passwd != (true, ""))
            {
                Log.Information("Пароль не соответствует формату");
            }

            if (log == (true, "") & passwd == (true, "") & repeat == (true, ""))
            {
                try
                {
                    //сохранение нового логина в файл
                    StreamWriter file = new StreamWriter("C:/Users/Настя/source/repos/lab1_registration/LoginList.txt", true);
                    file.WriteLine(login);
                    file.Close();
                    Console.WriteLine(" ");
                    Log.Information("Новый пользователь зарегистрирован");
                }
                catch (Exception e)
                {
                    Log.Information("Ошибка: " + e.Message);
                }
                //запись данных в логи
                Log.Information("Логин: " + login);
                MaskPasswd(password, password2);
            }
            else
            {
                Log.Information("Логин: " + login);
                MaskPasswd(password, password2);
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
                Log.Information("Ошибка: " + e.Message);

            }
            return log;
        }
        public static (bool, string) ValidationLogin(string login, (bool, string) email, bool number, (bool, string) nickname)
        {
            //проверка на совпадение с существующими логинами
            bool saveLog = ReadLoginList(login);
            if (saveLog == false)
            {
                if (number == true || email == (true, "") || nickname == (true, ""))
                {
                    //передача логина для записи в файл
                    Log.Information("Проверка логина на соответствие формату завершилась успехом");
                    return (true, "");

                }
                else
                {
                    Log.Information("Несоответствие логина формату данных");
                    return (false, "Несоответствие логина формату данных");
                }
            }
            else
            {
                Log.Information("Ошибка: данный логин уже зарегистрирован другим пользователем");
                return (false, "Ошибка: данный логин уже зарегистрирован другим пользователем");
            }
        }
        public static (bool, string) ValidNumber(string login)
        {
            // Создаем регулярное выражение для проверки номера телефона
            Regex regex = new Regex(@"^\+\d-\d{3}-\d{3}-\d{4}$");

            // Проверяем соответствие строки формату номера телефона
            bool number = regex.IsMatch(login);

            if (number == false)
            {
                //сообщение об ошибке формата телефона в консоль
                Log.Information("Логин не соответствует формату номера телефона");
                return (false, "Логин не соответствует формату номера телефона");
            }
            else
            {

                return (number, "");
            }
        }
        public static (bool, string) ValidEmail(string login)
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
                Log.Information("Логин не соответствует формату электронной почты");
                return (false, "Логин не соответствует формату электронной почты");
            }
            else
            {
                return (email, "");
            }

            
        }
        public static (bool, string) ValidNickname(string login)
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
                Log.Information("Логин содержит недопустимые символы");
            }
            if (!Regex.IsMatch(log, "[A-Za-z]"))
            {
                nickname = false;
                Log.Information("Логин должен содержать хотя бы одну прописную и заглавную букву");
            }
            if (!Regex.IsMatch(log, "[0-9]"))
            {
                nickname = false;
                Log.Information("Логин должен содержать хотя бы одну цифру");
            }
            if (log.Length <= 5)
            {
                nickname = false;
                Log.Information("Количество символов должно быть не менее пяти символов");
            }


            if (nickname == false)
            {
                //сообщение об ошибке формата ника в консоль
                Log.Information("Логин не соответствует формату никнейма");
                return (false, "Логин не соответствует формату никнейма");
            }

            return (nickname, "");
        }
        public static (bool, string) validationPasswd(string password)
        {
            // Проверка на минимальную длину
            if (password.Length < 7)
            {
                Log.Information("Количество символов в пароле должно быть более семи");
                return (false, "Количество символов в пароле должно быть более семи");
            }

            // Проверка на наличие хотя бы одной буквы в верхнем регистре
            if (!Regex.IsMatch(password, "[А-Я]"))
            {
                Log.Information("Логин должен содержать хотя бы одну букву в верхнем регистре");
                return (false, "Логин должен содержать хотя бы одну букву в верхнем регистре");
            }

            // Проверка на наличие хотя бы одной буквы в нижнем регистре
            if (!Regex.IsMatch(password, "[а-я]"))
            {
                Log.Information("Логин должен содержать хотя бы одну букву в нижнем регистре");
                return (false, "Логин должен содержать хотя бы одну букву в нижнем регистре");
            }

            // Проверка на наличие хотя бы одной цифры
            if (!Regex.IsMatch(password, @"\d"))
            {
                Log.Information("Логин должен содержать хотя бы одну цифру");
                return (false, "Логин должен содержать хотя бы одну цифру");
            }

            // Проверка на наличие хотя бы одного спецсимвола (допустимы только символы Юникода)
            if (!Regex.IsMatch(password, @"[\p{P}\p{S}]"))
            {
                Log.Information("Логин должен содержать хотя бы один спецсимвол");
                return (false, "Логин должен содержать хотя бы один спецсимвол");
            }

            // Проверка на наличие только кириллицы, цифр и спецсимволов
            if (!Regex.IsMatch(password, @"^[\p{Ll}\p{Lu}\p{M}\p{N}\p{P}\p{S}]+$"))
            {
                Log.Information("Логин должен содержать только кириллицу, цифры и спецсимволы");
                return (false, "Логин должен содержать только кириллицу, цифры и спецсимволы");
            }

            return (true, "");
        }
        public static (bool, string) repeatPasswd(string password, string password2)
        {
            if (password == password2)
            {
                return (true, "");
            }
            else
            {
                Log.Information("Пароли не совпадают");
                return (false, "Пароли не совпадают");
            }
        }
        public static void MaskPasswd(string password, string password2)
        {
            StringBuilder mask_passwd = new StringBuilder();
            StringBuilder mask_passwd2 = new StringBuilder();
            foreach (char c in password)
            {
                char nextChar = (char)(c + 1);
                mask_passwd.Append(nextChar);
            }
            foreach (char c in password2)
            {
                char nextChar = (char)(c + 1);
                mask_passwd2.Append(nextChar);
            }
            Log.Debug("Пароль: " + mask_passwd);
            Log.Information("Пароль2: " + mask_passwd2);
        }

    }
}
