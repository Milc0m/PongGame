﻿using System;
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
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        public HomeController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        Ball ball = new Ball();
        private string jsonInput;

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

            //ball.x = 150;
            //ball.y = 70;
            //ball.speed = 1000;

            ViewBag.x = Ball.x;
            //ViewBag.ballCoordY = ball.y;
            //ViewBag.ballSpeed = ball.speed;
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

        static volatile public int ballx = 0;
        static volatile public int bally = 0;

        //[ValidateAntiForgeryToken]
        public IActionResult OnPost()
        {

            Ball.x = Ball.x + 10;
            //ball.y = ball.y + 10;
            //int tempX = ball.x + 10;
            //ball.x = tempX;
            jsonInput = JsonConvert.SerializeObject(ball);
            ballx = ballx + 1;
            bally = bally + 1;
            return new JsonResult("{ \"x\": \"" + ballx.ToString() + "\", \"y\":  \"" + bally.ToString() + "\" }");
        }



    }
}


