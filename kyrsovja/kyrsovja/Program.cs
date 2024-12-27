using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace kyrsovja
{
    // Базовый класс
    class Game
    {
        public string Name { get; set; }
        public string Developer { get; set; }
        public DateTime ReleaseDate { get; set; }

        public Game(string name, string developer, DateTime releaseDate)
        {
            Name = name;
            Developer = developer;
            ReleaseDate = releaseDate;
        }

        public virtual string GetInfo(List<string> additions = null)
        {
            string info = $"Игра: {Name}";
            if (additions != null)
            {
                if (additions.Contains("Developer"))
                    info += $", Разработчик: {Developer}";
                if (additions.Contains("ReleaseDate"))
                    info += $", Дата релиза: {ReleaseDate.ToShortDateString()}";
            }
            return info;
        }
    }

    // Производный класс
    class DetailedGame : Game
    {
        public List<string> Genres { get; set; }
        public List<string> Platforms { get; set; }

        public DetailedGame(string name, string developer, DateTime releaseDate, List<string> genres, List<string> platforms)
            : base(name, developer, releaseDate)
        {
            Genres = genres;
            Platforms = platforms;
        }

        public override string GetInfo(List<string> additions = null)
        {
            string info = base.GetInfo(additions);
            if (additions != null)
            {
                if (additions.Contains("Genres"))
                    info += $", Жанры: {string.Join(", ", Genres)}";
                if (additions.Contains("Platforms"))
                    info += $", Платформы: {string.Join(", ", Platforms)}";
            }
            return info;
        }
    }

    class Program
    {
        static bool allowAdd = true; // Флаг для отслеживания доступности функции 'a'

        static void Main(string[] args)
        {
            string filePath = @"C:\\Users\\sedov\\OneDrive\\Документы\\бд_игры.txt";

            // Загрузка игр из файла
            List<DetailedGame> games = LoadGamesFromFile(filePath);

            while (true)
            {
                allowAdd = true; // Сбрасываем флаг при каждом возвращении в меню

                Console.Clear();
                Console.WriteLine("Добро пожаловать в информационную систему о видеоиграх!");
                Console.WriteLine("Вы можете выбрать один из следующих вариантов для просмотра информации:\n");
                Console.WriteLine("1 - Список всех игр");
                Console.WriteLine("2 - Список разработчиков");
                Console.WriteLine("3 - Игры по году релиза");
                Console.WriteLine("4 - Игры по жанру");
                Console.WriteLine("5 - Игры по платформе");
                Console.WriteLine("6 - Полный список информации о всех играх");
                Console.WriteLine("7 - Ввести новую игру в базу данных\n");
                Console.Write("Выбранное действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        DisplayGames(games, new List<string>());
                        WaitForBackOrAdd(games);
                        break;
                    case "2":
                        Console.Clear();
                        var developers = games.Select(g => g.Developer).Distinct().ToList();
                        Console.WriteLine("Список разработчиков:");
                        for (int i = 0; i < developers.Count; i++)
                        {
                            Console.WriteLine($"{i + 1} - {developers[i]}");
                        }
                        Console.Write("Введите номер разработчика: ");
                        if (int.TryParse(Console.ReadLine(), out int devIndex) && devIndex > 0 && devIndex <= developers.Count)
                        {
                            Console.Clear();
                            var selectedGames = games.Where(g => g.Developer == developers[devIndex - 1]).ToList();
                            DisplayGames(selectedGames, new List<string>());
                            WaitForBackOrAdd(selectedGames);
                        }
                        break;
                    case "3":
                        Console.Clear();
                        var years = games.Select(g => g.ReleaseDate.Year).Distinct().OrderBy(y => y).ToList();
                        Console.WriteLine("Список годов релиза:");
                        for (int i = 0; i < years.Count; i++)
                        {
                            Console.WriteLine($"{i + 1} - {years[i]}");
                        }
                        Console.Write("Введите номер года: ");
                        if (int.TryParse(Console.ReadLine(), out int yearIndex) && yearIndex > 0 && yearIndex <= years.Count)
                        {
                            int year = years[yearIndex - 1];
                            Console.Clear();
                            var filteredGames = games.Where(g => g.ReleaseDate.Year == year).ToList();
                            DisplayGames(filteredGames, new List<string>());
                            WaitForBackOrAdd(filteredGames);
                        }
                        break;
                    case "4":
                        Console.Clear();
                        var genres = games.SelectMany(g => g.Genres).Distinct().ToList();
                        Console.WriteLine("Список жанров:");
                        for (int i = 0; i < genres.Count; i++)
                        {
                            Console.WriteLine($"{i + 1} - {genres[i]}");
                        }
                        Console.Write("Введите номер жанра: ");
                        if (int.TryParse(Console.ReadLine(), out int genreIndex) && genreIndex > 0 && genreIndex <= genres.Count)
                        {
                            string genre = genres[genreIndex - 1];
                            Console.Clear();
                            var filteredGames = games.Where(g => g.Genres.Contains(genre)).ToList();
                            DisplayGames(filteredGames, new List<string>());
                            WaitForBackOrAdd(filteredGames);
                        }
                        break;
                    case "5":
                        Console.Clear();
                        var platforms = games.SelectMany(g => g.Platforms).Distinct().ToList();
                        Console.WriteLine("Список платформ:");
                        for (int i = 0; i < platforms.Count; i++)
                        {
                            Console.WriteLine($"{i + 1} - {platforms[i]}");
                        }
                        Console.Write("Введите номер платформы: ");
                        if (int.TryParse(Console.ReadLine(), out int platformIndex) && platformIndex > 0 && platformIndex <= platforms.Count)
                        {
                            string platform = platforms[platformIndex - 1];
                            Console.Clear();
                            var filteredGames = games.Where(g => g.Platforms.Contains(platform)).ToList();
                            DisplayGames(filteredGames, new List<string>());
                            WaitForBackOrAdd(filteredGames);
                        }
                        break;
                    case "6":
                        Console.Clear();
                        var allAdditions = new List<string> { "Developer", "ReleaseDate", "Genres", "Platforms" };
                        DisplayGames(games, allAdditions);
                        WaitForBack();
                        break;
                    case "7":
                        Console.Clear();
                        Console.WriteLine("Введите информацию о новой игре:");
                        Console.Write("Название: ");
                        string name = Console.ReadLine();

                        Console.Write("Разработчик: ");
                        string developer = Console.ReadLine();

                        Console.Write("Дата релиза (в формате ГГГГ-ММ-ДД): ");
                        DateTime releaseDate;
                        while (!DateTime.TryParse(Console.ReadLine(), out releaseDate))
                        {
                            Console.Write("Неверный формат. Попробуйте снова: ");
                        }

                        Console.Write("Жанры (перечислите через запятую): ");
                        List<string> newGenres = Console.ReadLine().Split(", ").ToList();

                        Console.Write("Платформы (перечислите через запятую): ");
                        List<string> newPlatforms = Console.ReadLine().Split(", ").ToList();

                        var newGame = new DetailedGame(name, developer, releaseDate, newGenres, newPlatforms);
                        games.Add(newGame);

                        // Сохранение в файл
                        File.AppendAllText(filePath, $"{name} - Developer: {developer}, Release Date: {releaseDate:yyyy-MM-dd}, Genres: {string.Join(", ", newGenres)}, Platforms: {string.Join(", ", newPlatforms)}{Environment.NewLine}");

                        Console.WriteLine("Новая игра успешно добавлена в базу данных!");
                        WaitForBack();
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор. Пожалуйста, попробуйте снова.");
                        break;
                }
            }
        }

        static List<DetailedGame> LoadGamesFromFile(string filePath)
        {
            var games = new List<DetailedGame>();
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var parts = line.Split(" - ");
                if (parts.Length == 2)
                {
                    var name = parts[0];
                    var details = parts[1].Split(", ");

                    // Извлекаем данные
                    var developer = details.FirstOrDefault(d => d.StartsWith("Developer: "))?.Replace("Developer: ", "");
                    var releaseDate = DateTime.Parse(details.FirstOrDefault(d => d.StartsWith("Release Date: "))?.Replace("Release Date: ", ""));

                    // Обрабатываем списки жанров и платформ
                    var genresString = details.FirstOrDefault(d => d.StartsWith("Genres: "))?.Replace("Genres: ", "");
                    var genres = genresString != null ? genresString.Split(new[] { ", " }, StringSplitOptions.None).ToList() : new List<string>();

                    var platformsString = details.FirstOrDefault(d => d.StartsWith("Platforms: "))?.Replace("Platforms: ", "");
                    var platforms = platformsString != null ? platformsString.Split(new[] { ", " }, StringSplitOptions.None).ToList() : new List<string>();

                    // Добавляем игру в список
                    games.Add(new DetailedGame(name, developer, releaseDate, genres, platforms));
                }
            }

            return games;
        }

        static void DisplayGames(List<DetailedGame> games, List<string> additions = null)
        {
            for (int i = 0; i < games.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {games[i].GetInfo(additions)}");
            }
        }

        static void WaitForBackOrAdd(List<DetailedGame> games)
        {
            Console.WriteLine("\nНажмите 'b' для возврата к списку действий" + (allowAdd ? " или 'a' для вывода полной информации по всем играм..." : "..."));
            string input = Console.ReadLine()?.ToLower();

            if (input == "a" && allowAdd)
            {
                Console.Clear();
                var allAdditions = new List<string> { "Developer", "ReleaseDate", "Genres", "Platforms" };
                DisplayGames(games, allAdditions);
                allowAdd = false; // Отключаем возможность вызова функции 'a' до возвращения в меню
                WaitForBack();
            }
        }

        static void WaitForBack()
        {
            Console.WriteLine("\nНажмите 'b' для возврата к списку действий...");
            while (Console.ReadLine()?.ToLower() != "b")
            {
                Console.WriteLine("Некорректный ввод. Попробуйте снова.");
            }
        }
    }
}
