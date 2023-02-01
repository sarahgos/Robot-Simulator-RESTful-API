using Microsoft.AspNetCore.Mvc;
using System.Windows.Input;
using robot_controller_api.Persistence;
using robot_controller_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserDataAccess _userRepo;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public UsersController(IUserDataAccess userRepo)
    {
        _userRepo = userRepo;
    }

    /// <summary>
    /// Gets all users from the database.
    /// </summary>
    /// <returns>List of users</returns>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "CEOOnly")]
    [HttpGet()]
    public IEnumerable<User> GetAllUsers()
    {
        return _userRepo.GetUsers();
    }

    /// <summary>
    /// Gets all admin users from the database.
    /// </summary>
    /// <returns>List of admin users</returns>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "CEOOnly")]
    [HttpGet("admin")]
    public IEnumerable<User> GetAdminUsersOnly()
    {
        return _userRepo.GetAdminUsersOnly();
    }

    /// <summary>
    /// Gets user by given Id number.
    /// </summary>
    /// <param name="id">The Id of the user to return.</param>
    /// <returns>HTTP ok response with user object details.</returns>
    /// <response code="200">Returns the user with matching id.</response>    
    /// <response code="404">If user with id isn't found.</response>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "CEOOnly")]
    [HttpGet("{id}", Name = "GetUser")]
    public IActionResult GetUserById(int id)
    {
        var user = _userRepo.GetUserById(id);

        if (user == null)
            return NotFound();
        else
            return Ok(user);
    }

    /// <summary>
    /// Add a new user to database.
    /// </summary>
    /// <param name="newUser">New user object from HTTP request.</param>
    /// <returns>CreatedAtRoute(user object)</returns>
    /// <remarks>
    /// Sample request:
    ///     POST /api/users
    ///     {
    ///         "firstname": "John",
    ///         "lastname": "Smith",
    ///         "email": "email@domain.com",
    ///         "passwordhash": "password",
    ///         "description": "user is blah blah",
    ///         "role": "admin"
    ///     }
    /// </remarks>
    /// <response code="400">If the user request is null.</response>
    /// <response code="409">If a user already exists with the same name.</response>
    /// <response code="201">Returns the newly created user.</response>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "HeadAdminOnly")]
    [HttpPost()]
    public IActionResult AddUser(User newUser)
    {
        var password = newUser.PasswordHash;
        var user = newUser;
        var hasher = new PasswordHasher<User>();
        var pwHash = hasher.HashPassword(user, password);
        var pwVerificationResult = hasher.VerifyHashedPassword(user, pwHash, password);
        user.PasswordHash = pwHash;
        user.CreatedDate = DateTime.Now;
        user.ModifiedDate = DateTime.Now;

        if (newUser == null)
            return BadRequest();

        var result = _userRepo.InsertUser(newUser);

        if (result == null)
            return Conflict();
        else
            return CreatedAtRoute("GetUser", new { id = newUser.Id }, newUser);
    }

    /// <summary>
    /// Modify an existing user in the database.
    /// </summary>
    /// <param name="updatedUser">user object with updated properties.</param>
    /// <returns>No content</returns>
    /// <remarks>
    /// Sample request:
    ///     PUT /api/users
    ///     {
    ///         "id": 6,
    ///         "firstname": "John",
    ///         "lastname": "Smith",
    ///         "description": "user is blah blah",
    ///         "role": "admin"
    ///     }    
    /// </remarks>
    /// <response code="400">If the user request is null.</response>
    /// <response code="404">If a user with the given id isn't found.</response>
    /// <response code="204">If successful, return no content.</response>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "HeadAdminOnly")]
    [HttpPut("{id}")]
    public IActionResult UpdateUser(User updatedUser)
    {
        if (updatedUser == null)
            return BadRequest();

        var result = _userRepo.UpdateUser(updatedUser);

        if (result == null)
            return NotFound();

        else
            return NoContent();
    }

    /// <summary>
    /// Delete a user from the database.
    /// </summary>
    /// <param name="id">Id of user to delete.</param>
    /// <returns>no content or not found</returns>
    /// <response code="404">If a user with given id isn't found.</response>
    /// <response code="204">If deletion is successful.</response>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "HeadAdminOnly")]
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var result = _userRepo.DeleteUser(id);

        if (result == null)
            return NotFound();
        else
            return NoContent();
    }

    /// <summary>
    /// Updates email and password.
    /// </summary>
    /// <param name="id">Id of user to update.</param>
    /// <returns>Login object</returns>
    /// <response code="404">If id of user not found or hash password failed.</response>
    /// <response code="201">Returns the newly created Login.</response>
    /// <response code="403">User does not have access.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "HeadAdminOnly")]
    [HttpPatch("{id}")]
     public IActionResult UpdateEmailAndPassword(int id, Login login)
     {
        User updatedUser = _userRepo.UpdateEmailAndPassword(id, login);

        if (updatedUser == null)
            return BadRequest();

        var password = updatedUser.PasswordHash;
        var user = updatedUser;
        var hasher = new PasswordHasher<User>();
        var pwHash = hasher.HashPassword(user, password);
        var pwVerificationResult = hasher.VerifyHashedPassword(user, pwHash, password);

        if (pwVerificationResult == PasswordVerificationResult.Success)
        {
            user.PasswordHash = pwHash;
        }
        else
            return BadRequest();

        user.CreatedDate = DateTime.Now;
        user.ModifiedDate = DateTime.Now;

        var result = _userRepo.UpdateUser(updatedUser);

        if (result == null)
            return Conflict();
        else
            return Ok(login);
    }
}
