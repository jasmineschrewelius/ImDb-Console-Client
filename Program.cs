using System.Net.Http.Json;
using static System.Console;
using ImDb.Domain;
using ImDb.DTO;

namespace ImDb;

class Program
{

    static readonly HttpClient httpClient = new()  // this will send a HTTP to Web API
    {
        BaseAddress = new Uri("http://localhost:9000/")
    };

    public static void Main()
    {
        Title = "ImDb";

        while (true)
        {
            CursorVisible = false;

            WriteLine("1. Lista Filmer");
            WriteLine("2. Lägg Till Film");
            WriteLine("3. Radera Film");

            var keyPressed = ReadKey(true);

            Clear();

            switch (keyPressed.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                
                ListMoviesView();
                
                break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:

                AddMovieView();

                break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:

                DeleteMovieView();

                break;
            }

            Clear();
        }
    }

    private static void DeleteMovieView() // show delete movie view to user
    {
        Write("Ange Titel:");        // ask user for titel

        var title = ReadLine();      // read the user input

        Clear();

        var movies = GetMovies(title); // call GetMovies to get movies that includes the title

        foreach (var movie in movies)   // if we find the movies that included the title, show Id and Title
        {
            WriteLine($"{movie.Id} - {movie.Title}");
        }

        WriteLine();

        Write("Ange ID:");   // ask user for id

        var movieId = ReadLine();  // read the user input

        Clear();

        DeleteMovie(movieId);   // call DeleteMovie, includes the movieId

        WriteLine("Film Raderad");

        Thread.Sleep(2000);
    }

    private static void DeleteMovie(string? movieId) // DeleteMovie, that has movieId included
    {
        httpClient.DeleteAsync($"movie/{movieId}");
    }

    private static void AddMovieView()   // show add movie view, and save the user input
    {
        var title = GetUserInput("Titel");
        var plot = GetUserInput("Handling");
        var genre = GetUserInput("Genre");
        var director = GetUserInput("Regissör");
        var releaseYear = int.Parse(GetUserInput("År"));

        var movie = new Movie{
            Title = title,
            Plot = plot,
            Genre = genre,
            Director = director,
            ReleaseYear = releaseYear
        };

        Clear();

        try   // if the movie can be saved, writeline
        {
            SaveMovie(movie);   // call SaveMovie 

            WriteLine("Film Sparad");
        }
        catch   // if the movie can not be saved, writeline
        {
            WriteLine("Ogiltig information");
        }

        Thread.Sleep(2000);
    }

    private static void SaveMovie(Movie movie)  // will save movie
    {
        var createMovieRequest = new CreateMovieRequest
        {
            Title = movie.Title,
            Plot = movie.Plot,
            Genre = movie.Genre,
            Director = movie.Director,
            ReleaseYear = movie.ReleaseYear
        };

        // serialise Movie to JSON, then send information to we API
        // HTTP POST https://localhost:9000/movie
        var response = httpClient.PostAsJsonAsync("movie", createMovieRequest).Result;

        // throw an exception is status code is not within 2xx (200,201 ...)
        response.EnsureSuccessStatusCode();
    }

    private static string GetUserInput(string label)
    {
        CursorVisible = true;

        Write($"{label}:");

        return ReadLine() ?? "";
    }

    private static void ListMoviesView() // will list all the movies view
    {
        var movies = GetMovies();   // call function that recieves all the movies

        Write($"{"Id", -16}");
        Write($"{"Titel", -16}");
        Write($"{"Genre", -26}");
        WriteLine("Director");

        foreach (var movie in movies)
        {
            Write($"{movie.Id, -16}");
            Write($"{movie.Title, -16}");
            Write($"{movie.Genre, -26}");
            WriteLine(movie.Director);
        }

        WaitUntilKeyPressed(ConsoleKey.Escape);
    }

    private static void WaitUntilKeyPressed(ConsoleKey key)
    {
        while (ReadKey(true).Key != key);
    }

    private static IEnumerable<Movie> GetMovies(string title = null)  // gets all the movies  (string title = null)- set the state of title to null as standard
    {
        // send HTTP GET to web API and deserialise  JSON to a IEnumerable<MovieDto>
        var moviesDto = title is not null    // if the title is not null 
        // we will send a HTTP GET https://localhost:9000/movie?title={title} request
        ? httpClient.GetFromJsonAsync<IEnumerable<MovieDto>>($"movie?title={title}").Result
        // if the title is null, we will send a HTTP GET https://localhost:9000/movie request
        : httpClient.GetFromJsonAsync<IEnumerable<MovieDto>>($"movie").Result
        ?? Enumerable.Empty<MovieDto>();

        // will take a collection of IEnumerable<MovieDto> and will create a new IEnumerable<Movie>
        var movies = moviesDto.Select(x => new Movie
        {
            Id = x.Id,
            Title = x.Title,
            Plot = x.Plot,
            Genre = x.Genre,
            Director = x.Director,
            ReleaseYear = x.ReleaseYear

        });

        // return IEnumerable<Movie>
        return movies;
    }
}