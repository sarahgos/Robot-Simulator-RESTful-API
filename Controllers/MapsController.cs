using Microsoft.AspNetCore.Mvc;
using System.Windows.Input;
using robot_controller_api.Persistence;
using robot_controller_api.Models;
using Microsoft.AspNetCore.Authorization;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    private readonly IMapDataAccess _mapsRepo;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public MapsController(IMapDataAccess mapsRepo)
    {
        _mapsRepo = mapsRepo;
    }

    /// <summary>
    /// Gets all maps from the database.
    /// </summary>
    /// <returns>List of maps</returns>
    [Authorize(Policy = "UserOnly")]
    [HttpGet()]
    public IEnumerable<Map> GetAllMaps()
    {
        return _mapsRepo.GetMaps();
    }

    /// <summary>
    /// Gets all square maps from the database.
    /// </summary>
    /// <returns>List of square maps</returns>
    [Authorize(Policy = "UserOnly")]
    [HttpGet("square")]
    public IEnumerable<Map> GetSquareMapsOnly()
    {
        return _mapsRepo.GetSquareMapsOnly();
    }

    /// <summary>
    /// Gets map by given Id number.
    /// </summary>
    /// <param name="id">The Id of the map to return.</param>
    /// <returns>HTTP ok response with map object details.</returns>
    /// <response code="200">Returns the map with matching id.</response>    
    /// <response code="404">If map with id isn't found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "UserOnly")]
    [HttpGet("{id}", Name = "GetMap")]
    public IActionResult GetMapById(int id)
    {
        var map = _mapsRepo.GetMapById(id);

        if (map == null)
            return NotFound();
        else
            return Ok(map);
    }

    /// <summary>
    /// Add a new map to database.
    /// </summary>
    /// <param name="newMap">New map object from HTTP request.</param>
    /// <returns>CreatedAtRoute(map object)</returns>
    /// <remarks>
    /// Sample request:
    ///     POST /api/robot-commands
    ///     {
    ///         "name": "Crater1",
    ///         "columns": 10,
    ///         "rows": 12
    ///     }
    /// </remarks>
    /// <response code="400">If the map request is null.</response>
    /// <response code="409">If a map already exists with the same name.</response>
    /// <response code="201">Returns the newly created map.</response>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "AdminOnly")]
    [HttpPost()]
    public IActionResult AddMap(Map newMap)
    {
        if (newMap == null)
            return BadRequest();

        var result = _mapsRepo.InsertMap(newMap);

        if (result == null)
            return Conflict();
        else
            return CreatedAtRoute("GetMap", new { id = newMap.Id }, newMap);
    }

    /// <summary>
    /// Modify an existing map in the database.
    /// </summary>
    /// <param name="updatedMap">Map object with updated properties.</param>
    /// <returns>No content</returns>
    /// <remarks>
    /// Sample request:
    ///     PUT /api/maps
    ///     {
    ///         "Id": 16,
    ///         "Name": "Map6",
    ///         "Description": "80 x 90 map",
    ///         "Columns":80,
    ///         "Rows":90
    ///     }    
    /// </remarks>
    /// <response code="400">If the map request is null.</response>
    /// <response code="404">If a map with the given id isn't found.</response>
    /// <response code="409">If there is already a map with that name.</response>
    /// <response code="204">If successful, return no content.</response>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public IActionResult UpdateMap(Map updatedMap)
    {
        if (updatedMap == null)
            return BadRequest();

        var result = _mapsRepo.UpdateMap(updatedMap);

        if (result == null)
            return NotFound();
        else if (result.Name == null)
            return Conflict();
        else
            return NoContent();
    }

    /// <summary>
    /// Delete a map from the database.
    /// </summary>
    /// <param name="id">Id of map to delete.</param>
    /// <returns>no content or not found</returns>
    /// <response code="404">If a map with given id isn't found.</response>
    /// <response code="204">If deletion is successful.</response>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public IActionResult DeleteMap(int id)
    {
        var result = _mapsRepo.DeleteMap(id);

        if (result == null)
            return NotFound();
        else
            return NoContent();
    }

    /// <summary>
    /// Checks if a given coordinate is on a map.
    /// </summary>
    /// <param name="id">Id of map to check.</param>
    /// <param name="x">Column of map.</param>
    /// <param name="y">Row of map.</param>
    /// <returns>True is on map/false if not.</returns>
    /// <response code="400">If coordinates less than 0.</response>
    /// <response code="404">If id of map not found.</response>
    /// <response code="200">Returns true/false.</response>
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = "UserOnly")]
    [HttpGet("{id}/{x}-{y}")]
     public IActionResult CheckCoordinate(int id, int x, int y)
     {
        bool isOnMap;

        if (x < 0 || y < 0)
            return BadRequest();
 
        var result = _mapsRepo.CheckCoordinates(id, x, y);

        if (result == null)
            return NotFound();
        else if (x > result.Columns || y > result.Rows)
            isOnMap = false;
        else
            isOnMap = true;

        return Ok(isOnMap);
     }
}
