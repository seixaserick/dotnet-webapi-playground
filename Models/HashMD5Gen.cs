using System.ComponentModel.DataAnnotations;

namespace Models;

public class HashMD5Gen
{

    [Required]
    [StringLength(int.MaxValue, MinimumLength = 1)]
    public string inputString { get; set; }


    public string? hashMD5Value { get; set; }

}
