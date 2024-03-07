using System.Reflection;
using HaBuddies.DTOs;
using HaBuddies.Models;
using HaBuddies.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace HaBuddies.Controllers
{
    public class EventController : Controller
    {
        private readonly EventService _eventService;

        public EventController(EventService eventService) =>
            _eventService = eventService;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadEvent(string category = null!, int page = 1, int perPage = 10)
        {
            try
            {
                UserNoPassword user = HttpContext.Session.Get<UserNoPassword>("user")!;
                string userId = "";
                if (user != null) userId = user.Id;
                var paginationResponse = await _eventService.GetAllAsync(page, perPage, category, userId);

                if (paginationResponse.Data.Count <= 0 && paginationResponse.PrevPage != null) {
                    return StatusCode(204);
                }
                
                return PartialView("_EventBannerPartial", paginationResponse);
            }
            catch (Exception)
            {
                return StatusCode(500);
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
        public async Task<IActionResult> Create(CreateEventDTO newEventDTO)
        {
            try {
                UserNoPassword user = HttpContext.Session.Get<UserNoPassword>("user")!;

                if (user == null) {
                    throw new UnauthorizedAccessException();
                }

                Event newEvent = await _eventService.CreateAsync(newEventDTO, user.Id!);

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
                UserNoPassword user = HttpContext.Session.Get<UserNoPassword>("user")!;

                if (evt == null)
                {
                    return View("NotFound");
                }
                else if (user.Id != evt.OwnerId) {
                    throw new Exception("Forbidden");
                }

                return View(evt);
            }
            catch (Exception) {
                return View("Error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Edit(string id, EditEventDTO editedEventDTO)
        {
            try {
                UserNoPassword user = HttpContext.Session.Get<UserNoPassword>("user")!;
                var editedEvent = await _eventService.EditAsync(id, editedEventDTO, user.Id);

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
                UserNoPassword user = HttpContext.Session.Get<UserNoPassword>("user")!;

                await _eventService.RemoveAsync(id, user.Id);

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
                UserNoPassword user = HttpContext.Session.Get<UserNoPassword>("user")!;
                if (user == null) {
                    throw new UnauthorizedAccessException();
                }
                await _eventService.SubscribeEvent(id, user.Id!);
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
