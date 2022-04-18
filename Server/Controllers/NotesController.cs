using ElevenNoteWebApp.Server.Services.NoteService;
using ElevenNoteWebApp.Shared.Models.Note;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElevenNoteWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }

        private string GetUserId()
        {
            string userIdClaim = User.Claims.First(i => i.Type == ClaimTypes.NameIdentifier).Value;

            if (userIdClaim == null) return null;

            return userIdClaim;
        }

        private bool SetUserIdInService()
        {
            var userId = GetUserId();
            if (userId == null) return false;

            _noteService.SetUserId(userId);
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!SetUserIdInService())
            {
                return Unauthorized();  
            }

            var notes = await _noteService.GetAllNotesAsync();
            return Ok(notes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Note(int id)
        {
            if (!SetUserIdInService()) return Unauthorized();

            var note = await _noteService.GetNoteByIdAsync(id);

            if (note == null) return NotFound();

            return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NoteCreate model)
        {
            if (model == null) return BadRequest();

            if (!SetUserIdInService()) return Unauthorized();

            bool wasSuccessful = await _noteService.CreateNoteAsync(model);
            if (wasSuccessful) return Ok();
            else return UnprocessableEntity();
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Edit(int id, NoteEdit model)
        {
            if (!SetUserIdInService()) return Unauthorized();

            if (model == null || !ModelState.IsValid) return BadRequest();

            if (model.Id != id) return BadRequest();

            bool wasSuccessful = await _noteService.UpdateNoteAsync(model);
            if (wasSuccessful) return Ok();
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!SetUserIdInService()) return Unauthorized();
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null) return NotFound();

            bool wasSuccessful = await _noteService.DeleteNoteAsync(id);
            if (!wasSuccessful) return BadRequest();

            return Ok();
        }
    }
}
