//// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
//// for details on configuring this project to bundle and minify static web assets.


const cvs = document.getElementById("pong");
const ctx = cvs.getContext("2d");

const user = {
    x: 0,
    y: cvs.height/2 - 50,
    width: 10,
    height: 100,
    color: "WHITE",
    score: 0
}

const user2 = {
    x: cvs.width - 10,
    y: cvs.height/2 - 50,
    width: 10,
    height: 100,
    color: "WHITE",
    score: 0
}

const ball = {
    x: cvs.width / 2,
    y: cvs.height / 2,
    radius: 10,
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
    drawCircle(ball.x, ball.y, ball.radius, ball.color);
}

cvs.addEventListener("mousemove", movePaddle);

function movePaddle(evt) {
    let rect = cvs.getBoundingClientRect();
    user.y = evt.clientY - rect.top - user.height/2;
}

function resetBall() {
    ball.x = cvs.width / 2;
    ball.y = cvs.height / 2;

    ball.speed = 5;
    ball.velocityX = -ball.velocityX;
}

function collision(b, p) {
    p.top = p.y;
    p.bottom = p.y + p.height;
    p.left = p.x;
    p.right = p.x + p.width;

    b.top = b.y - b.radius;
    b.bottom = b.y + b.radius;
    b.left = b.x - b.radius;
    b.right = b.x + b.radius;

    return p.left < b.right && p.top < b.bottom && p.right > b.left && p.bottom > b.top;
}

function update() {
    ball.x += ball.velocityX;
    ball.y += ball.velocityY;

    //AI for test
    let computerLevel = 0.2;
    user2.y += ((ball.y - (user2.y + user2.height / 2))) * computerLevel;

    if (ball.y + ball.radius > cvs.height || ball.y - ball.radius < 0) {
        ball.velocityY = - ball.velocityY;
    }

    let player = (ball.x + ball.radius < cvs.width / 2) ? user : user2;

    if (collision(ball, player)) {
        let colliedPoint = ball.y - (player.y + player.height / 2);
        colliedPoint = colliedPoint / (player.height/2);

        let angleRadius = colliedPoint * (Math.PI/4);

        let direction = (ball.x + ball.radius < cvs.width / 2) ? 1 : -1;


        ball.velocityX = direction * ball.speed * Math.cos(angleRadius);
        ball.velocityY = ball.speed * Math.sin(angleRadius);

        ball.speed += 0.5;
    }

    if (ball.x - ball.radius < 0) {
        user2.score++;
        resetBall();
    } else if (ball.x + ball.radius > cvs.width) {
        user.score++;
        resetBall();
    }
}

//Init
function game() {
    update();
    render();
}

////loop
const framePerSecond = 70;
gameloop = setInterval(game, 1000 / framePerSecond);
gamePaused = false;

function pauseGame() {
    if (!gamePaused) {
        gameloop = clearInterval(gameloop);
        gamePaused = true;
    } else if (gamePaused) {
        gameloop = setInterval(game, 1000 / framePerSecond);
        gamePaused = false;
    }
};