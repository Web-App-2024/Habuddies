using System.Threading.Tasks;
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
            var paginationResponse = await _eventService.GetAllAsync(page, perPage, category);
            return View(paginationResponse);
        }

        public async Task<IActionResult> Details(string id)
        {
            var evt = await _eventService.GetOneAsync(id);

            if (evt == null)
            {
                return NotFound("Event Not Found");
            }

            return View(evt);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Event newEvent)
        {
            await _eventService.CreateAsync(newEvent);

            return RedirectToAction(nameof(Details), new { id = newEvent.id });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var evt = await _eventService.GetOneAsync(id);

            if (evt == null)
            {
                return NotFound();
            }

            return View(evt);
        }

        [HttpPut]
        public async Task<IActionResult> Edit(string id, [FromBody] Event updatedEvent)
        {
            var evt = await _eventService.GetOneAsync(id);

            if (evt == null)
            {
                return NotFound();
            }

            updatedEvent.id = evt.id;

            await _eventService.UpdateAsync(id, updatedEvent);

            return RedirectToAction(nameof(Details), new { id = updatedEvent.id });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var evt = await _eventService.GetOneAsync(id);

            if (evt == null)
            {
                return NotFound();
            }

            await _eventService.RemoveAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
