using System.ComponentModel.DataAnnotations;

namespace Models;

public class HashMD5Gen
{

    [Required]
    public string inputString { get; set; }


    public string? hashMD5Value { get; set; }
     
}
