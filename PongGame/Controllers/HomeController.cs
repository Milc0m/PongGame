using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PongGame.Models;

namespace PongGame.Controllers
{
    public class Parammetrs
    {
        //public string X { get; set; }
        //public string Y { get; set; }
        public int Y { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        
    }


    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        public HomeController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        //Ball ball = new Ball();
        //private string jsonInput;

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            if (userId == null)
            {
                return View();
            }
            else
            {
                User user = _userManager.FindByIdAsync(userId).Result;
                // Getting information about current user
                ViewBag.userName = user.Name;
                ViewBag.userRegistrationDate = user.RegistrationDate;
                ViewBag.userLastVisit = user.LastVisit;
                // Update time of last visit
                user.LastVisit = DateTime.Now; 
                _userManager.UpdateAsync(user);
                return View();
            }
        }

        public IActionResult Game()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [Authorize]
        public IActionResult GameBoard()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            string userIdstring = userId;
            ViewBag.userId = userId;
            User user = _userManager.FindByIdAsync(userId).Result;
            Player player = new Player();
            player.Id = userId;
            player.Name = user.Name;
            player.SearchingForOpponent = true;
            ViewBag.searching = player.SearchingForOpponent;
            ViewBag.name = player.Name;
            
            Player.players.Add(player);
            Player opponent;
            if (player.SearchingForOpponent && Player.players.Count > 1)
            {
                
                opponent = Player.players.Where(x => x.Id != player.Id && x.SearchingForOpponent /*&& !x.IsPlaying*/).FirstOrDefault();
                ViewBag.opp = opponent.Name;
            }


            ViewData["Message"] = "Game.";
            return View();
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

        //static volatile public int ballx = 0;
        //static volatile public int bally = 0;

        [HttpPost]
        public IActionResult OnPost([FromBody]string strmodel)
        {
            //if (model.Type == "search")
            //{
            //    return new JsonResult(model.Id);
            //}
            //else
            //{
            //    return new JsonResult("none");
            //}
            return new JsonResult(strmodel);
            //return new JsonResult(model.Y);

        }



    }
}


