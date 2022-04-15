using ElevenNoteWebApp.Server.Data;
using ElevenNoteWebApp.Server.Models;
using ElevenNoteWebApp.Shared.Models.Note;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevenNoteWebApp.Server.Services.NoteService
{
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _context;
        private string _userId;

        public NoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateNoteAsync(NoteCreate model)
        {
            var noteEntity = new Note
            {
                Title = model.Title,
                Content = model.Content,
                OwnerId = _userId,
                CreatedUtc = DateTimeOffset.Now,
                CategoryId = model.CategoryId
            };

            _context.Notes.Add(noteEntity);
            var numberOfChanges = await _context.SaveChangesAsync();

            return numberOfChanges == 1;
        }

        public async Task<bool> DeleteNoteAsync(int noteId)
        {
            var entity = await _context.Notes.FindAsync(noteId);
            if (entity?.OwnerId != _userId)
            {
                return false;
            }

            _context.Notes.Remove(entity);
            return await _context.SaveChangesAsync() == 1;
        }

        public Task<bool> DeleteNoteAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<NoteListItem>> GetAllNotesAsync()
        {
            var noteQuery = _context.Notes.Where(x => x.OwnerId == _userId)
                .Select(x => new NoteListItem
                {
                    Id = x.Id,
                    Title = x.Title,
                    CategoryName = x.Category.Name,
                    CreatedUtc = x.CreatedUtc,
                });

            return await noteQuery.ToListAsync();
        }

        public async Task<NoteDetail> GetNoteByIdAsync(int noteId)
        {
            var noteEntity = await _context.Notes.Include(nameof(Category))
                .FirstOrDefaultAsync(x => x.Id == noteId && x.OwnerId == _userId);

            if (noteEntity == null)
                return null;

            var detail = new NoteDetail
            {
                Id = noteEntity.Id,
                Title = noteEntity.Title,
                Content = noteEntity.Content,
                CreatedUtc = noteEntity.CreatedUtc,
                ModifiedUtc = noteEntity.ModifiedUtc,
                CategoryName = noteEntity.Category.Name,
                CategoryId = noteEntity.Category.Id,
            };
            return detail;
        }

        public async Task<bool> UpdateNoteAsync(NoteEdit model)
        {
            if (model == null) return false;

            var entity = await _context.Notes.FindAsync(model.Id);

            if (entity?.OwnerId != _userId) return false;

            entity.Title = model.Title;
            entity.Content = model.Content;
            entity.CategoryId = model.CategoryId;
            entity.ModifiedUtc = DateTimeOffset.Now;

            return await _context.SaveChangesAsync() == 1;
        }
        public void SetUserId(string userId) => _userId = userId;
    }
}
