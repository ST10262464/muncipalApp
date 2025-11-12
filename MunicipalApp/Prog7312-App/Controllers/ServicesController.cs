using Microsoft.AspNetCore.Mvc;
using Prog7312_App.Models;
using Prog7312_App.Models.DataStructures; 
using System.Collections.Generic;
using System.Linq;

namespace Prog7312_App.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ILogger<ServicesController> _logger;
        
        // 1. CustomBinarySearchTree (BST) for efficient retrieval by ReferenceNumber (Tree requirement)
        private static CustomBinarySearchTree<string, ServiceRequest> _serviceRequestTree = new();
        
        // 2. CustomPriorityQueue (Min-Heap) for efficient prioritization (Heap requirement)
        private static CustomPriorityQueue<ServiceRequest> _priorityQueue = new();
        
        // 3. CustomGraph for demonstration of routing/MST (Graph/Traversal/MST requirement)
        private static CustomGraph<string> _serviceRouteGraph = new();
        
        private static int _nextId = 1;

        // Public access to AdjacencyList for the ViewRouteOptimization view
        public Dictionary<string, List<(string Neighbor, int Weight)>> AdjacencyList => _serviceRouteGraph.AdjacencyList;


        public ServicesController(ILogger<ServicesController> logger)
        {
            _logger = logger;
            if (_serviceRequestTree.Count == 0)
            {
                SeedRequests(); 
            }
        }
        
        private void SeedRequests()
        {
            // Utility: 1 (Highest), Roads: 2, Sanitation: 3, Safety: 4, Parks: 5 (Lowest)
            AddRequestToStructures(new ServiceRequest { Id = _nextId++, Location = "Main Road", Category = "Roads", Description = "Large pothole in fast lane.", Priority = 2, Status = ServiceRequestStatus.InProgress });
            AddRequestToStructures(new ServiceRequest { Id = _nextId++, Location = "Park Street 12", Category = "Sanitation", Description = "Weekly refuse collection missed.", Priority = 3, Status = ServiceRequestStatus.Submitted });
            AddRequestToStructures(new ServiceRequest { Id = _nextId++, Location = "Suburbia Power Substation", Category = "Utilities", Description = "Power outage for 48 hours.", Priority = 1, Status = ServiceRequestStatus.Submitted });
            AddRequestToStructures(new ServiceRequest { Id = _nextId++, Location = "Public Library", Category = "Public Safety", Description = "Graffiti on exterior walls.", Priority = 4, Status = ServiceRequestStatus.Resolved });
            
            // Seed the Graph for Route Demonstration
            // Nodes are Issue Locations (for simplicity)
            _serviceRouteGraph.AddNode("DEPOT"); 
            _serviceRouteGraph.AddNode("Main Road");
            _serviceRouteGraph.AddNode("Park Street 12");
            _serviceRouteGraph.AddNode("Suburbia Power Substation");
            _serviceRouteGraph.AddNode("Public Library");
            
            // Edges represent travel time/distance (Weight)
            _serviceRouteGraph.AddEdge("DEPOT", "Main Road", 10);
            _serviceRouteGraph.AddEdge("DEPOT", "Park Street 12", 25);
            _serviceRouteGraph.AddEdge("DEPOT", "Public Library", 15);
            _serviceRouteGraph.AddEdge("Main Road", "Park Street 12", 5);
            _serviceRouteGraph.AddEdge("Park Street 12", "Suburbia Power Substation", 12);
            _serviceRouteGraph.AddEdge("Public Library", "Suburbia Power Substation", 8);
        }
        
        private void AddRequestToStructures(ServiceRequest request)
        {
            // Set required fields for new requests (ReferenceNumber, CreatedAt)
            if (string.IsNullOrEmpty(request.ReferenceNumber))
            {
                request.ReferenceNumber = $"SR{DateTime.Now:yyyyMMdd}{request.Id:D4}";
                request.CreatedAt = DateTime.Now;
            }
            
            _serviceRequestTree.Insert(request.ReferenceNumber, request);
            _priorityQueue.Enqueue(request, request.Priority);
            
            // Add new reported location to the graph for potential future routing
            _serviceRouteGraph.AddNode(request.Location);
        }

        public IActionResult ReportIssue()
        {
            // Pass a default model with a priority value
            return View(new ServiceRequest { Priority = 5 }); 
        }

        [HttpPost]
        public async Task<IActionResult> ReportIssue(ServiceRequest request, List<IFormFile> attachments)
        {
            // Manually set priority based on category for demonstration
            request.Priority = request.Category.ToLower() switch
            {
                "utilities" => 1,
                "roads" => 2,
                "sanitation" => 3,
                _ => 5
            };

            if (ModelState.IsValid)
            {
                request.Id = _nextId++;
                request.SubmittedByEmail = HttpContext.Session.GetString("UserEmail");

                // ... Handle file uploads logic (kept from original file) ...

                // **Insertion into advanced data structures**
                AddRequestToStructures(request);

                TempData["SuccessMessage"] = $"Your service request has been submitted successfully. Reference number: {request.ReferenceNumber}";
                return RedirectToAction("ReportSuccess", new { referenceNumber = request.ReferenceNumber });
            }

            return View(request);
        }

        // Updated signature/logic to use ReferenceNumber for efficient BST lookup
        public IActionResult ReportSuccess(string referenceNumber)
        {
            // Use the BST for efficient lookup by unique identifier (O(log n) average)
            var request = _serviceRequestTree.Find(referenceNumber);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }
        
        // Action for looking up request by reference number (tracking requirement)
        public IActionResult GetRequestByReference(string referenceNumber)
        {
            var request = _serviceRequestTree.Find(referenceNumber);
            if (request != null)
            {
                return View("ReportSuccess", request);
            }
            return NotFound(); 
        }

        public IActionResult ViewRequests()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            
            // Get all requests using In-Order Traversal from BST for a sorted list
            var allRequests = _serviceRequestTree.InOrderTraversal();
            
            if (!string.IsNullOrEmpty(userEmail))
            {
                // Filter requests for the logged-in user
                var userRequests = allRequests.Where(r => r.SubmittedByEmail == userEmail).ToList();
                ViewData["Title"] = "My Service Requests";
                return View(userRequests);
            }
            
            ViewData["Title"] = "All Submitted Requests (Sorted by Reference)"; 
            return View(allRequests); 
        }
        
        // Action to demonstrate Heap/Priority Queue usage
        public IActionResult ViewPriorityRequests()
        {
            // Get all requests for a full snapshot
            var allRequests = _serviceRequestTree.InOrderTraversal();
            
            // 1. Create a local queue and populate it to demonstrate priority sorting 
            var localQueue = new CustomPriorityQueue<ServiceRequest>();
            foreach (var request in allRequests)
            {
                localQueue.Enqueue(request, request.Priority);
            }
            
            // 2. Dequeue all to get them in priority order
            var sortedList = new List<ServiceRequest>();
            while (localQueue.TryDequeue(out ServiceRequest? request, out int priority))
            {
                // Null check for safety
                if(request != null)
                {
                    sortedList.Add(request);
                }
            }
            
            ViewData["Title"] = "Priority Requests (Heap Demonstration)";
            // Reuse the existing view for display
            return View("ViewRequests", sortedList); 
        }

        // **NEW: Action to demonstrate Graph Traversal and MST**
        public IActionResult ViewRouteOptimization()
        {
            ViewData["Title"] = "Route Optimization Demo (Graph Structures)";
            
            // 1. Graph Traversal Demo (BFS)
            // Shows the order a worker might check sites near the DEPOT
            var bfsResult = _serviceRouteGraph.BreadthFirstSearch("DEPOT");
            ViewData["BFS"] = bfsResult;

            // 2. Minimum Spanning Tree (MST) Demo
            // Shows the cheapest way to connect all service points/locations
            var mstResult = _serviceRouteGraph.GetMinimumSpanningTree("DEPOT");
            ViewData["MST"] = mstResult;
            
            return View("RouteOptimization"); // New view needed
        }

        // Retained or modified JSON actions (can be removed if not needed)
        public IActionResult GetRecentRequests(int count = 5)
        {
            var allRequests = _serviceRequestTree.InOrderTraversal();
            var recentRequests = allRequests
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToList();
            return Json(recentRequests);
        }

        public IActionResult GetAllRequests()
        {
            return Json(_serviceRequestTree.InOrderTraversal());
        }
    }
}