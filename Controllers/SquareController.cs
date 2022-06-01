using Models;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class SquareController : ControllerBase
{

    private readonly ILogger<SquareController> _logger;

    public SquareController(ILogger<SquareController> logger)
    {
        _logger = logger;
    }


    /// <summary>
    /// Returns an integer after each input digit to be squared and concatenated (as string) before returing as int. Example: (int) input 929 will return => 81-4-81 => 81481 (int)
    /// <param name="inputNumber">Integer value. Each digit will be squared separately and the result will be an integer of this "string join". Example: (int) input 929 will return => 81-4-81 => 81481 (int)</param>
    /// <returns></returns>
    [HttpGet("/square_all_digits/{inputNumber}")]
    public ActionResult GetSquareAllDigits(int inputNumber)
    {
        try
        {
            int _return = Services.SquareService.SquareAllDigits(inputNumber); //Calls the SquareAllDigits() function which squares each digit separately and the result will be an integer of this "string join".
            return Ok(_return);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }


    /// <summary>
    /// Simples square calculation
    /// </summary>
    /// <param name="inputNumber">Integer value. The number you wish to square. Example:  input 9 => output 81.</param>
    /// <returns></returns>
    [HttpGet("/square/{inputNumber}")]
    public ActionResult GetSquare(int inputNumber)
    {
        try
        {
            int _return = inputNumber * inputNumber; //calculates the square of inputNumber
            return Ok(_return);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }
}
