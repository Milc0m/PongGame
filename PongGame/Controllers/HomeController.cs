using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
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

        private static List<Player> playersList = new List<Player>();
        private int ballRadius = 10;
        private int padWidth = 10;
        private int padHeight = 100;
        int gameWidth = 800;
        int gameHeight = 600;
       
            



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

        Player player = new Player();

        [Authorize]
        public IActionResult GameBoard()
        {
            ViewBag.ballRadius = ballRadius;
            ViewBag.padWidth = padWidth;
            ViewBag.padHeight = padHeight;
            
            ViewBag.width = gameWidth;
            ViewBag.height = gameHeight;


            var userId = _userManager.GetUserId(HttpContext.User);
            string userIdstring = userId;
            ViewBag.userId = userId;
            User user = _userManager.FindByIdAsync(userId).Result;
           
            player.Id = userId;
            player.Name = user.Name;
            player.SearchingForOpponent = true;
            ViewBag.searching = player.SearchingForOpponent;
            ViewBag.name = player.Name;
            

            player.pad.TopLeft.X = 0;
            player.pad.TopLeft.Y = gameHeight/2 - 50;
            player.pad.BottomRight.X = 10;
            player.pad.BottomRight.Y = gameHeight / 2 + 50;

           
            


            if (!Player.players.Any(x => x.Id == userId))
            {
                Player.players.Add(player);
            }

            
            Player opponent;
            //opponent.pad.TopLeft.X = gameWidth - 10;
            //TODO find why dont work when player equals 1
            
           if (player.SearchingForOpponent && Player.players.Count > 1)
            {
                
                ViewBag.List = Player.players;
                opponent = Player.players.Where(x => x.Id != player.Id && x.SearchingForOpponent /*&& !x.IsPlaying*/).FirstOrDefault();
                opponent.OpponentId = player.Id;
                player.OpponentId = opponent.Id;
                player.IAmDaddy = true;
                player.ball.Center.X = gameWidth / 2;
                player.ball.Center.Y = gameHeight / 2;
                player.ball.Radius = ballRadius;
                player.ball.Velocity.X = 5;
                player.ball.Velocity.Y = 5;


                opponent.IAmDaddy = false;
                
                playersList.Add(opponent);
                playersList.Add(player);
                Player playerList = playersList.Where(x => x.Id == player.Id).FirstOrDefault();
                

                //delete from Player.players
                Player.players.Remove(opponent);
                Player.players.Remove(player);
            }


            ViewBag.daddy = player.IAmDaddy;
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


        

        [HttpPost]
        public IActionResult OnPost([FromBody]dynamic parametrs)
        {

            Parammetrs paramJson = new Parammetrs();
            Player playerJson = playersList.Where(x => x.Id == parametrs["Id"].ToString()).FirstOrDefault();
            Player oponentrJson = playersList.Where(x => x.Id == playerJson.OpponentId).FirstOrDefault();

            if (oponentrJson == null)
            {
                paramJson.Y = 0;
                paramJson.Id = "none";
            }
            else
            {
                paramJson = JsonConvert.DeserializeObject<Parammetrs>(parametrs.ToString());
                playerJson.pad.TopLeft.Y = paramJson.Y;
                playerJson.pad.BottomRight.Y = paramJson.Y + 100;
                playerJson.ball.Speed = 7;

                if (playerJson.IAmDaddy)
                {
                    playerJson.ball.Center.X += playerJson.ball.Velocity.X;
                    playerJson.ball.Center.Y += playerJson.ball.Velocity.Y;
                    

                    if (playerJson.ball.Center.Y + playerJson.ball.Radius > gameHeight ||
                        playerJson.ball.Center.Y - playerJson.ball.Radius < 0)
                    {
                        playerJson.ball.Velocity.Y = -playerJson.ball.Velocity.Y;
                    }

                    void resetBall()
                    {
                        playerJson.ball.Center.X = gameWidth / 2;
                        playerJson.ball.Center.Y = gameHeight/ 2;
                        if (playerJson.ball.Velocity.X > 0)
                        {
                            playerJson.ball.Velocity.X = -5;
                        }
                        else
                        {
                            playerJson.ball.Velocity.X = 5;
                        }
                        playerJson.ball.Velocity.Y = 5;
                    }

                    if (playerJson.ball.Center.X - playerJson.ball.Radius < 0)
                    {
                        resetBall();
                    }
                    else if (playerJson.ball.Center.X + playerJson.ball.Radius > gameWidth)
                    {
                        resetBall();
                    }

                    bool collisionPlayer(Pad rect, Ball circle)
                    {
                        float y = rect.TopLeft.Y + padHeight / 2;
                        float x = rect.TopLeft.X + padWidth / 2; 

                        float circleDistanceX = Math.Abs(circle.Center.X - x);
                        float circleDistanceY = Math.Abs(circle.Center.Y - y);

                        if (circleDistanceX > (padWidth / 2 + circle.Radius)) { return false; }
                        if (circleDistanceY > (padHeight / 2 + circle.Radius)) { return false; }

                        if (circleDistanceX <= (padWidth / 2)) { return true; }
                        if (circleDistanceY <= (padHeight / 2)) { return true; }

                        double cornerDistanceSq = Math.Pow((circleDistanceX - padWidth / 2), 2) +
                                                 Math.Pow((circleDistanceY - padHeight / 2), 2);

                        return (cornerDistanceSq <= Math.Pow(circle.Radius, 2));
                    }

                    if (collisionPlayer(playerJson.pad, playerJson.ball))
                    {

                        var colliedPoint = (playerJson.pad.TopLeft.Y + (padHeight / 2)) - playerJson.ball.Center.Y;
                        colliedPoint = colliedPoint / (padHeight / 2);

                        var angleRadius = colliedPoint * (5 * Math.PI /12);

                        playerJson.ball.Velocity.X = playerJson.ball.Speed * (float)Math.Cos(angleRadius);
                        playerJson.ball.Velocity.Y = playerJson.ball.Speed *  -(float)Math.Sin(angleRadius);

                        //playerJson.ball.Speed += 1;
                    }

                    // opponent collision
                    if (collisionPlayer(oponentrJson.pad, playerJson.ball))
                    {
                        var colliedPoint = (oponentrJson.pad.TopLeft.Y + (padHeight / 2)) - playerJson.ball.Center.Y;
                        colliedPoint = colliedPoint / (padHeight / 2);

                        var angleRadius = colliedPoint * (5 * Math.PI / 12);

                        playerJson.ball.Velocity.X = playerJson.ball.Speed * (float)Math.Cos(angleRadius);
                        playerJson.ball.Velocity.Y = playerJson.ball.Speed * -(float)Math.Sin(angleRadius);

                        //playerJson.ball.Speed += 1;
                    }


                    paramJson.BallX = playerJson.ball.Center.X;
                    paramJson.BallY = playerJson.ball.Center.Y;
                }
                else
                {
                    
                    //paramJson.VelocityX = -oponentrJson.ball.Velocity.X;
                    paramJson.BallX = gameWidth - oponentrJson.ball.Center.X;
                    paramJson.BallY = oponentrJson.ball.Center.Y;
                }

                playerJson.parameters = paramJson;
                paramJson.Y = oponentrJson.pad.TopLeft.Y;

            }

            return new JsonResult(JsonConvert.SerializeObject(paramJson));

        }

    }
}


