using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GogoKit;
using GogoKit.Models.Request;
using GogoKit.Models.Response;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections;

namespace ViagogoTicketApp.Controllers
{

    public class EventController : Controller
    {
        // Access to Viagogo API
        private readonly IViagogoClient _viagogoClient = new ViagogoClient(new ProductHeaderValue("ViaGogoTicketInterview"),
                                        "TaRJnBcw1ZvYOXENCtj5",
                                        "ixGDUqRA5coOHf3FQysjd704BPptwbk6zZreELW2aCYSmIT8XJ9ngvN1MuKV");

        public async Task<ActionResult> Listing(int id, int page = 1, int noOfTickets = 0)
        {
            var request = new ListingRequest(); 
            var Event = await _viagogoClient.Events.GetAsync(id);
            if (noOfTickets != 0)
            {
                request.Parameters.Add("number_of_tickets", noOfTickets.ToString());
            }
            var listings = await _viagogoClient.Listings.GetAllByEventAsync(id, request);

            ViewBag.NoOfTickets = noOfTickets;
            // Rounding up division for page count
            ViewBag.Pages = (listings.Count + 15 - 1) / 15;
            ViewBag.Page = page;
            ViewBag.ID = id;
            ViewBag.EventName = Event.Name + " - " + Event.Venue.Name;
            ViewBag.Listings = listings.ToList<Listing>().Skip((page-1)*15).Take(15);
            return View();
        }

        // Convert search results list to list of events (hashtable datatype)
        public List<Hashtable> RelevantEventsInfo(IReadOnlyList<Event> events, DateTime minDate, DateTime maxDate)
        {
            // Create Hashtable list to store all relevant values for each event
            List<Hashtable> eventsInfo = new List<Hashtable>();
            // Create Dictionary to store event ID with cheapest ticket per country
            Dictionary<string, int> cheapestEventPerCountry = new Dictionary<string, int>();
            // Iterate through the events
            for (int i = 0; i < events.Count; i++)
            {
                // Store current event in variable local to loop
                var Event = events[i];

                // If event doesn't pass filtering, skip this iteration of the loop
                if (minDate.Ticks > Event.StartDate.Value.DateTime.Ticks || maxDate.Ticks < Event.StartDate.Value.DateTime.Ticks)
                {
                    continue;
                }

                // Create Hashtable in eventsInfo list
                eventsInfo.Add(new Hashtable());

                // Add relevant information to the event Hashtable
                Hashtable latestEvent = eventsInfo[eventsInfo.Count - 1];
                latestEvent.Add("ID", Event.Id);
                latestEvent.Add("Name", Event.Name);
                latestEvent.Add("Date", Event.StartDate);
                if (Event.MinTicketPrice != null)
                {
                    latestEvent.Add("MinPrice", Event.MinTicketPrice.Amount);
                    latestEvent.Add("MinPriceDisplay", Event.MinTicketPrice.Display);
                }
                else
                {
                    latestEvent.Add("MinPrice", 0);
                    latestEvent.Add("MinPriceDisplay", "Not available!");
                }
                latestEvent.Add("Country", Event.Venue.Country.Name);
                latestEvent.Add("City", Event.Venue.City);
                latestEvent.Add("Venue", Event.Venue.Name);

                // Add to cheapest event dictionary if appropriate
                if (Event.MinTicketPrice != null)
                {
                    if (!cheapestEventPerCountry.ContainsKey(Event.Venue.Country.Name) ||
                    Event.MinTicketPrice.Amount < (decimal)eventsInfo[cheapestEventPerCountry[Event.Venue.Country.Name]]["MinPrice"])
                    {
                        cheapestEventPerCountry[Event.Venue.Country.Name] = eventsInfo.Count - 1;
                    }
                }
            }

            // Mark events as cheapest in country if relevant
            foreach (var key in cheapestEventPerCountry.Keys)
            {
                int id = cheapestEventPerCountry[key];
                eventsInfo[id].Add("IsCheapestInCountry", true);
            }

            return eventsInfo;
        }
    }
}