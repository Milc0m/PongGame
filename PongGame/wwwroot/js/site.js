//// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
//// for details on configuring this project to bundle and minify static web assets.


const cvs = document.getElementById("pong");
const ctx = cvs.getContext("2d");

const user = {
    id: "none",
    x: 0,
    y: cvs.height/2 - 50,
    width: document.getElementById('padWidth').value/*10*/,
    height: document.getElementById('padHeight').value/*100*/,
    color: "WHITE",
    score: 0
}

const user2 = {
    id: "none",
    x: cvs.width - 10,
    y: cvs.height/2 - 50,
    width: document.getElementById('padWidth').value/*10*/,
    height: document.getElementById('padHeight').value/*100*/,
    color: "WHITE",
    score: 0
}

const ball = {
    x: cvs.width / 2,
    y: cvs.height / 2,
    radius: document.getElementById('ballRadius').value,
    speed: 5,
    velocityX: 5,
    velocityY: 5,
    color: "WHITE"
}

const net = {
    x: cvs.width/2 - 1,
    y: 0,
    width: 2,
    height: 10,
    color: "WHITE"
}

function drawNet() {
    for (let i = 0; i <= cvs.height; i += 15) {
        drawRect(net.x, net.y + i, net.width, net.height, net.color);
    }
}

function drawRect(x, y, w, h, color) {
    ctx.fillStyle = color;
    ctx.fillRect(x, y, w, h);
}



function drawCircle(x, y, r, color) {
    ctx.fillStyle = color;
    ctx.beginPath();
    ctx.arc(x, y, r, 0, Math.PI * 2, false);
    ctx.closePath();
    ctx.fill();
}

function drawText(text, x, y, color) {
    ctx.fillStyle = color;
    ctx.font = "45px fantasy";
    ctx.fillText(text, x, y);
}

function render() {
    drawRect(0, 0, cvs.width, cvs.height, "BLACK");

    drawNet();
    //score
    drawText(user.score, cvs.width / 4, cvs.height / 5, "WHITE");
    drawText(user2.score, 3 * cvs.width / 4, cvs.height / 5, "WHITE");
    //paddle
    drawRect(user.x, user.y, user.width, user.height, user.color);
    drawRect(user2.x, user2.y, user2.width, user2.height, user2.color);
    //ball
    console.log(ball.radius);
    drawCircle(ball.x, ball.y, ball.radius, ball.color);
}

cvs.addEventListener("mousemove", movePaddle);

function movePaddle(evt) {
    let rect = cvs.getBoundingClientRect();
    let mouseY = evt.clientY;
    if (mouseY - 50 < rect.top) {
        user.y = 0;
    } else if (mouseY + 50 > rect.bottom) {
        user.y = 500;
    } else {
        user.y = evt.clientY - rect.top - user.height / 2;
    }
}

var send = 0;

//Init
function game() {
    if (send == 0) {
        sendRequest(user.y);
        send = 1;
    } else {
        send = 0;
    }
    
    render();
}

function startGame() {
    const framePerSecond = 50;
    gameloop = setInterval(game, 1000 / framePerSecond);
    gamePaused = false;
}

////loop

String.prototype.replaceAll = function (str1, str2, ignore) {
    return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignore ? "gi" : "g")), (typeof (str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
} 

startGame();

function pauseGame() {
    if (!gamePaused) {
        gameloop = clearInterval(gameloop);
        gamePaused = true;
    } else if (gamePaused) {
        gameloop = setInterval(game, 1000 / framePerSecond);
        gamePaused = false;
    }
};

function sendRequest(y) {
    var http = new XMLHttpRequest();
    var url = 'OnPost';
    var params = JSON.stringify({ "Y": y, "Id": document.getElementById('userId').textContent, "BallX": ball.x, "BallY": ball.y, "VelocityX": ball.velocityX, "VelocityY": ball.velocityY }); 
    http.open('POST', url, true);

   

    //Send the proper header information along with the request
    http.setRequestHeader('Content-type', 'application/json'/*'application/x-www-form-urlencoded'*/);

    http.onreadystatechange = function () {//Call a function when the state changes.
        if (http.readyState == 4 && http.status == 200) {
           
            var response = http.responseText;
            var obj = JSON.parse(response);
            var obj2 = JSON.parse(obj)

            var div = document.getElementById('searchingOpponent');

            if (obj2.Id == "none") {
                div.innerHTML = 'Searching for opponent';
                return;
            }
            div.innerHTML = obj2.Id;
            ball.x = obj2.BallX;
            ball.y = obj2.BallY;
            user2.y = parseFloat(obj2.Y);
            user2.score = obj2.ScoreOpponent;
            user.score = obj2.ScorePlayer;
            //console.log(response);
        }
    }
    http.send(params);
}

