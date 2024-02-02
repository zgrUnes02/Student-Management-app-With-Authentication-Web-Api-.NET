using BackendApiV2.Contexts;
using BackendApiV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendApiV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraineeController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public TraineeController(AppDbContext appDbContext) 
        {
            this.appDbContext = appDbContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllTrainees()
        {
            return Ok(await appDbContext.Trainees.ToListAsync());
        }

        [Authorize]
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetSingleTrainee([FromRoute] Guid id)
        {
            // Search for the trainee
            var trainee = await appDbContext.Trainees.FindAsync(id);

            // If trainee exists
            if ( trainee != null)
            {
                return Ok(trainee);    
            }

            // If not found
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateNewTrainee(CreateTraineeRequest createTraineeRequest)
        {
            var newTrainee = new Trainee()
            {
                Id = Guid.NewGuid(),
                FirstName = createTraineeRequest.FirstName,
                LastName = createTraineeRequest.LastName,
                Age = createTraineeRequest.Age,
                Phone = createTraineeRequest.Phone,
                Address = createTraineeRequest.Address
            };

            // Save in database
            await appDbContext.Trainees.AddAsync(newTrainee);
            await appDbContext.SaveChangesAsync();

            // Return message response
            return Ok(new { message = "The trainee has been create with success" });
        }

        [Authorize]
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateTrainee([FromRoute] Guid id , updateTraineeRequest updateTraineeRequest)
        {
            // Search for trainee by id
            var traineeWantToUpdate = await appDbContext.Trainees.FindAsync(id);

            // If trainee exists
            if ( traineeWantToUpdate != null )
            {
                traineeWantToUpdate.FirstName = updateTraineeRequest.FirstName;
                traineeWantToUpdate.LastName = updateTraineeRequest.LastName;
                traineeWantToUpdate.Age = updateTraineeRequest.Age;
                traineeWantToUpdate.Phone = updateTraineeRequest.Phone;
                traineeWantToUpdate.Address = updateTraineeRequest.Address;

                // Save the changes
                await appDbContext.SaveChangesAsync();

                // Return a message
                return Ok(new { message = "Trainee has been updated with success" });
            }

            // If trainee not found
            return NotFound(new { message = "Trainee not found" });
        }

        [Authorize]
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteTrainee([FromRoute] Guid id)
        {
            // Search for the trainee by id
            var traineeWantToDelete = await appDbContext.Trainees.FindAsync(id);

            // If trainee exists
            if ( traineeWantToDelete != null )
            {
                // Delete trainee and save the changes
                appDbContext.Remove(traineeWantToDelete);
                await appDbContext.SaveChangesAsync();

                // Return a message
                return Ok(new { message = "Trainee has been deleted with success" });
            }

            // If Trainee not found
            return NotFound(new { message = "Trainee not found" });
        }
    }
}
