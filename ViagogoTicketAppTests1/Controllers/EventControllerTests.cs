using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViagogoTicketApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Threading.Tasks;
using GogoKit;
using GogoKit.Models.Response;
using GogoKit.Models.Request;
using System.Net.Http.Headers;
using System.Collections;

namespace ViagogoTicketApp.Controllers.Tests
{
    [TestClass()]
    public class EventControllerTests
    {
        // Access to Viagogo API
        private readonly IViagogoClient _viagogoClient = new ViagogoClient(new ProductHeaderValue("ViaGogoTicketInterview"),
                                            "TaRJnBcw1ZvYOXENCtj5",
                                            "ixGDUqRA5coOHf3FQysjd704BPptwbk6zZreELW2aCYSmIT8XJ9ngvN1MuKV");

        private async Task<IReadOnlyList<Event>> FetchSearchResults()
        {
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

            return allEvents;
        }

        // Check that EventController.RelevantEventsInfosToEvents returns a List<Hashtable>
        [TestMethod()]
        public async Task RelevantEventsInfosReturnTypeTest()
        {
            EventController controller = new EventController();
            // Store search results
            IReadOnlyList<Event> allEvents = await FetchSearchResults();
            // Return value of RelevantEventsInfosToEvents
            DateTime maxDate = Convert.ToDateTime("9999-12-31 23:59:59");
            DateTime minDate = DateTime.Today;
            var result = controller.RelevantEventsInfo(allEvents, minDate, maxDate);
            // Verify that RelevantEventsInfosToEvents returns a Hashtable array
            Assert.IsInstanceOfType(result, typeof(List<Hashtable>));
        }

        // Check that all keys in each hashtable in the array returned by EventController.RelevantEventsInfosToEvents are not null 
        [TestMethod()]
        public async Task RelevantEventsInfosReturnNullTest()
        {
            EventController controller = new EventController();
            // Store search results
            IReadOnlyList<Event> searchResults = await FetchSearchResults();
            // Return value of RelevantEventsInfosToEvents
            DateTime maxDate = Convert.ToDateTime("9999-12-31 23:59:59");
            DateTime minDate = DateTime.Today;
            var result = controller.RelevantEventsInfo(searchResults, minDate, maxDate);
            foreach (var e in result) {
                foreach (var v in e.Values)
                {
                    // Verify that each element in the event is not null
                    Assert.IsNotNull(v);
                }
            }
        }
    }
}