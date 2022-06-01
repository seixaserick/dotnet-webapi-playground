using Models;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class Base64Controller : ControllerBase
{

    private readonly ILogger<Base64Controller> _logger;

    public Base64Controller(ILogger<Base64Controller> logger)
    {
        _logger = logger;
    }


    /// <summary>
    /// Returns the input string in base64 format. Input string must be a UTF-8 encoding text.
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns></returns>
    [HttpGet("/base64_encode/{inputString}", Name = "Returns the input string in base64 format")]
    public ActionResult GetBase64Encode(string inputString)
    {

        // Just a simple invalid request return test
        if (inputString.Length < 2)
        {
            return BadRequest(new { error = "Input string must be at least 2 chars" });
        }

        var base64EncodedOutputString = Base64Service.Base64Encode(inputString);

        //returning as Object the inputString and base64Encoded
        //Ok() is a HTTP 200 success StatusCode
        return Ok(new
        {
            input = inputString,
            base64Encoded = base64EncodedOutputString
        });

    }



    /// <summary>
    /// Returns the base64 encoded input in plain string format (UTF-8 encoding)
    /// </summary>
    /// <param name="inputBase64String"></param>
    /// <returns></returns>

    [HttpGet("/base64_decode/{inputBase64String}")]
    public ActionResult GetBase64Decode(string inputBase64String)
    {

        // Just a simple invalid request return test
        if (inputBase64String.Length < 2)
        {
            return BadRequest(new { error = "Input string must be at least 2 chars" });
        }


        try
        {
            var base64EncodedOutputString = Base64Service.Base64Decode(inputBase64String);

            //returning as Object the inputString and base64Encoded
            //Ok() is a HTTP 200 success StatusCode
            return Ok(new
            {
                input = inputBase64String,
                base64Decoded = base64EncodedOutputString
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }


    }
}
