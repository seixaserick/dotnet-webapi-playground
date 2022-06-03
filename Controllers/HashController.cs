using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class HashController : ControllerBase
{

    private readonly ILogger<Base64Controller> _logger;

    public HashController(ILogger<Base64Controller> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Return the MD5 hash for the input HashMD5Gen.inputString.
    /// </summary>
    /// <param name="_object">HashMD5Gen object</param>
    /// <returns></returns>
    [HttpPost("/hash_md5_generator")]
    public ActionResult<HashMD5Gen> PostHashMD5Generator(HashMD5Gen _object)
    {

        //Just a sample of Specific HTTP return StatusCode validating input values.
        if (!string.IsNullOrEmpty(_object.hashMD5Value))
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Leave the property hashMD5Value null when requesting. The hashMD5Value will be present in the returned object, but can't be informed in the POST request." });
        }


        //Generates the hash MD5 for the given inputString
        string hashMD5 = HashService.HashMD5(_object.inputString);
        _object.hashMD5Value = hashMD5; //fill the returning property hashMD5Value (string) with the generated hash


        //returning the complete Object 
        return Ok(_object);//Ok() is a HTTP 200 success StatusCode

    }




    /// <summary>
    /// Validates the input object "HashMD5Compare" verifying the received hash. If hash is valid, the response will be the input object with property isValid=true
    /// </summary>
    /// <param name="_object">HashMD5Compare object</param>
    /// <returns>An object HashMD5Compare with HashMD5Compare.isValid = true / false indicating the result of hash validation.</returns>
    [HttpPost("/hash_md5_validator")]
    public ActionResult<HashMD5Compare> PostHashMD5Validator(HashMD5Compare _object)
    {


        string generatedHashMD5 = HashService.HashMD5(_object.inputString);
        _object.isValid = generatedHashMD5 == _object.hashMD5Value; //compares the received hash with generated hash.

        //returning as Object with propoer isValue = true/false depending on comparison
        //Ok() is a HTTP 200 success StatusCode
        return Ok(_object);


    }


}
