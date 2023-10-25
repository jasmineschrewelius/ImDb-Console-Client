namespace ImDb.DTO;

public class MovieDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Plot { get; set; }

    public string Genre { get; set; }
   
    public string Director { get; set; }

    public int ReleaseYear { get; set; }
}