using System.ComponentModel.DataAnnotations;

namespace Models;

public class HashMD5Compare
{

    [Required]
    public string inputString { get; set; }


    [Required]
    [StringLength(32, MinimumLength = 32)]
    public string hashMD5Value { get; set; }

    public bool? isValid { get; set; }
}
