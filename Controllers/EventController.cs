using System.Threading.Tasks;
using HaBuddies.DTOs;
using HaBuddies.Models;
using HaBuddies.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaBuddies.Controllers
{
    public class EventController : Controller
    {
        private readonly EventService _eventService;

        public EventController(EventService eventService) =>
            _eventService = eventService;

        public async Task<IActionResult> Index(string category = null!, int page = 1, int perPage = 10)
        {
            try {
                var paginationResponse = await _eventService.GetAllAsync(page, perPage, category);
                return View(paginationResponse);
            }
            catch (Exception) {
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            try { 
                var evt = await _eventService.GetOneAsync(id);

                if (evt == null)
                {
                    return View("NotFound");
                }

                return View(evt);
            }
            catch (Exception) {
                return View("Error");
            }
        }

        public IActionResult Create()
        {
            try {
                return View();
            }
            catch (Exception) {
                return View("Error");
            }
        }
            
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventDTO newEventDTO)
        {
            try {
                string userId = HttpContext.Session.GetString("userId")!;

                if (userId == null) {
                    throw new UnauthorizedAccessException();
                }

                Event newEvent = await _eventService.CreateAsync(newEventDTO, userId);

                return RedirectToAction("Details", new { Id = newEvent.Id });
            } 
            catch (UnauthorizedAccessException) {
                return RedirectToAction("LoginAndRegister", "User");
            }
            catch (Exception) {
                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try {
                var evt = await _eventService.GetOneAsync(id);

                if (evt == null)
                {
                    return View("NotFound");
                }

                return View(evt);
            }
            catch (Exception) {
                return View("Error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Edit(string id, [FromBody] EditEventDTO editedEventDTO)
        {
            try {
                var editedEvent = await _eventService.EditAsync(id, editedEventDTO);

                return RedirectToAction("Details", new { Id = editedEvent.Id });
            }
            catch (Exception) {
                return View("Error");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            try {
                var evt = await _eventService.GetOneAsync(id);

                if (evt == null)
                {
                    return View("NotFound");
                }

                await _eventService.RemoveAsync(id);

                return RedirectToAction("Index");
            }
            catch (Exception) {
                return View("Error");
            }
        }

        [HttpPatch]
        public async Task<IActionResult> Subscribe(string id)
        {
            try {
                string userId = HttpContext.Session.GetString("userId")!;
                if (userId == null) {
                    throw new UnauthorizedAccessException();
                }
                await _eventService.SubscribeEvent(id, userId);
                return RedirectToAction("Details", new { Id = id });
            } 
            catch(UnauthorizedAccessException){
                return RedirectToAction("LoginAndRegister", "User");
            }
            catch(Exception){
                return View("Error");
            }
        }
    }
}
