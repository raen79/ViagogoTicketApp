using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GogoKit;
using GogoKit.Models.Response;
using GogoKit.Models.Request;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections;
using ViagogoTicketApp.Controllers;

namespace ViaGogoTicketInterview.Controllers
{
    public class ArtistController : Controller
    {
        // Access to Viagogo API
        private readonly IViagogoClient _viagogoClient = new ViagogoClient(new ProductHeaderValue("ViaGogoTicketInterview"),
                                        "TaRJnBcw1ZvYOXENCtj5",
                                        "ixGDUqRA5coOHf3FQysjd704BPptwbk6zZreELW2aCYSmIT8XJ9ngvN1MuKV");

        // Display Artist's Events
        public async Task<ActionResult> Index(DateTime? minDate = null, DateTime? maxDate = null, int page = 1)
        {
            // Set min and max dates to default values
            if (minDate == null)
            {
                ViewBag.MinDate = DateTime.Today;
                minDate = DateTime.Today;
            }
            else
            {
                ViewBag.minDate = minDate.Value;
            }

            
            if (maxDate == null)
            {
                maxDate = Convert.ToDateTime("9999-12-31 23:59:59");
                ViewBag.MaxDate = "";
            }
            else
            {
                ViewBag.MaxDate = maxDate.Value;
            }

            var searchRequest = new SearchResultRequest();
            // Set type of search to category (to find artist)
            searchRequest.Parameters.Add("type", "category");

            // Get the artist's category
            var searchResults = await _viagogoClient.Search.GetAsync("The Weeknd", searchRequest);

            // Retrieve category info
            var category = await _viagogoClient.Hypermedia.GetAsync<Category>(searchResults.Items.First().CategoryLink);

            var eventsRequest = new EventRequest();
            // Pagination for events request
            eventsRequest.Parameters.Add("page_size", "100000");
            // Retrieve all events
            var allEvents = await _viagogoClient.Events.GetAllByCategoryAsync(category.Id.Value, eventsRequest);

            // Generate list of relevant information for each event
            EventController eventController = new EventController();
            List<Hashtable> eventsInfo = eventController.RelevantEventsInfo(allEvents, minDate.Value, maxDate.Value);

            ViewBag.Events = eventsInfo.Skip((page - 1) * 15).Take(15);
            // Rounding up division to calculate amount of pages
            ViewBag.Pages = (eventsInfo.Count + 15 - 1) / 15;
            ViewBag.Page = page;
            return View();
        }
    }
}