using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotelyApp.Models;
using NotelyApp.Repositories;

namespace NotelyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly INoteRepository _noteRepository;

        public HomeController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public IActionResult Index()
        {
            //Get all the notes whih have not been deleted and pass to view
            var notes = _noteRepository.GetAllNotes().Where(n => n.IsDeleted == false);
            return View(notes);
        }

        public IActionResult NoteDetail(Guid id)
        {
            //Finds note by note id and returns it
            var note = _noteRepository.FindNoteById(id);
            return View(note);
        }

        // Displays the view (Note form)
        [HttpGet]
        public IActionResult NoteEditor(Guid id = default)
        {
            // Checks if note already exists, if it doesnt we create a new note
            if (id != Guid.Empty)
            {
                var note = _noteRepository.FindNoteById(id);
                return View(note);
            }

            return View();
        }

        // Method used to accept the form
        [HttpPost]
        public IActionResult NoteEditor(NoteModel noteModel)
        {
            var date = DateTime.Now;

            //If the note is valid and the note id is empty (new note)
            if (noteModel != null && noteModel.Id == Guid.Empty)
            {
                //Save note to the repository
                noteModel.Id = Guid.NewGuid();
                noteModel.CreatedDate = date;
                noteModel.LastModified = date;
                _noteRepository.SaveNote(noteModel);
            }
            else
            {
                // this is an existing note, find the note by id and update it
                var note = _noteRepository.FindNoteById(noteModel.Id);
                note.LastModified = date;
                note.Subject = noteModel.Subject;
                note.Detail = noteModel.Detail;
            }

            // Once action is complete, redirect to home page
            return RedirectToAction("Index");
        }

        // Method to delete a note
        public IActionResult DeleteNote(Guid id)
        {
            // Find note by id and set isDeleted to true
            var note = _noteRepository.FindNoteById(id);
            note.IsDeleted = true;
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
