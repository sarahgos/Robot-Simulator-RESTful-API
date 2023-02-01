using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;
using robot_controller_api.Models;
using Microsoft.AspNetCore.Authorization;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    private readonly IRobotCommandDataAccess _robotCommandsRepo;

    /// <summary>
    /// Constructor
    /// </summary>
    public RobotCommandsController(IRobotCommandDataAccess robotCommandsRepo)
    {
        _robotCommandsRepo = robotCommandsRepo;
    }

    /// <summary>
    /// Gets all robot commands.
    /// </summary>
    /// <returns>List of robot commands.</returns>
    [Authorize(Policy = "UserOnly")]
    [HttpGet("")]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        return _robotCommandsRepo.GetRobotCommands();
    }

    /// <summary>
    /// Gets only move robot commands.
    /// </summary>
    /// <returns>List of move commands.</returns>
    [Authorize(Policy = "UserOnly")]
    [HttpGet("move")]
    public IEnumerable<RobotCommand> GetMoveCommandsOnly()
    {
        return _robotCommandsRepo.GetMoveCommandsOnly();
    }

    /// <summary>
    /// Gets robot command by given Id number.
    /// </summary>
    /// <param name="id">The Id of the robot command to return.</param>
    /// <returns>HTTP ok response with robot command object details.</returns>
    /// <response code="200">Returns the robot command with matching id.</response>    
    /// <response code="404">If robot command with id isn't found.</response>
    [Authorize(Policy = "UserOnly")]
    [HttpGet("{id}", Name = "GetRobotCommand")]
    public IActionResult GetRobotCommandById(int id)
    {
        var robotCommand = _robotCommandsRepo.GetRobotCommandById(id);

        if (robotCommand == null)
            return NotFound();
        else
            return Ok(robotCommand);
    }

    /// <summary>
    /// Adds a robot command to the database.
    /// </summary>
    /// <param name="newCommand">RobotCommand object.</param>
    /// <returns>CreatedAtRouteObject</returns>
    /// <remarks>
    /// Sample request:
    ///     POST /api/robot-commands
    ///     {
    ///         "name": "DANCE",
    ///         "description": "Make robot dance",
    ///         "isMoveCommand": true
    ///     }
    /// </remarks>
    /// <response code="400">If the robot command request is null.</response>
    /// <response code="409">If a robot command already exists with the same name.</response>
    /// <response code="201">Returns the newly created robot command.</response>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "AdminOnly")]
    [HttpPost()]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null)
            return BadRequest();
        
        var result = _robotCommandsRepo.InsertRobotCommand(newCommand);

        if (result == null)
            return Conflict();
        else
            return CreatedAtRoute("GetRobotCommand", new { id = newCommand.Id }, newCommand);
    }

    /// <summary>
    /// Updates an existing command in the database.
    /// </summary>
    /// <param name="id">Id of command to update.</param>
    /// <param name="updatedCommand">RobotCommand object with updated parameters.</param>
    /// <returns>No content</returns>
    /// <remarks>
    /// Sample request:
    ///     PUT /api/robot-commands
    ///     {
    ///         "Id": 20,
    ///         "Name": "JUMP",
    ///         "Description": "Makes robot jump",
    ///         "isMoveCommand": true
    ///     }    
    /// </remarks>
    /// <response code="400">If the robot command request is null.</response>
    /// <response code="404">If a robot command with the given id isn't found.</response>
    /// <response code="409">If there is already a robot command with that name.</response>
    /// <response code="204">If successful, return no content.</response>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public IActionResult UpdateRobotCommand(RobotCommand updatedCommand)
    {
        if (updatedCommand == null)
            return BadRequest();

        var result = _robotCommandsRepo.UpdateRobotCommand(updatedCommand);

        if (result == null)
            return NotFound();
        else if (result.Name == null)
            return Conflict();
        else
            return NoContent();
    }

    /// <summary>
    /// Deletes a robot command from the database.
    /// </summary>
    /// <param name="id">Id of command to delete.</param>
    /// <returns>NoContent()</returns>
    /// <response code="404">No robot command matches given id.</response>
    /// <response code="204">Success.</response>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public IActionResult DeleteRobotCommand(int id)
    {
        var result = _robotCommandsRepo.DeleteRobotCommand(id);

        if (result == null)
            return NotFound();
        else
            return NoContent();
    }
}
