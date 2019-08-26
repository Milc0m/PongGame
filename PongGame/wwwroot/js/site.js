// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function moveCircle(idStr, valueX, valueY) {
    var circle = document.getElementById(idStr);
    //var coordX = parseInt(circle.getAttribute("cx"));
    //valueX = coordX + deltaX;
    //var coordY = parseInt(circle.getAttribute("cy"));
    //valueY = coordY + deltaY;
    circle.setAttributeNS(null, "cx", valueX);
    circle.setAttributeNS(null, "cy", valueY);
}



function startGame(idStr) {
    console.log("Begin startgame")
    var intervalID = setInterval(function () { getCoords(idStr) }, 100);
    console.log("end startgame")
}



 
function getCoords(idStr) {

    $.ajax({
        type: "POST",
        url: 'OnPost',
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        dataType: "json"
    }).done(function (data) {
        var obj = JSON.parse(data);
        moveCircle(idStr, obj.x, obj.y);
    })
}



function mouseHandler(event) {
    x = event.clientX;
    y = event.clientY;
    console.log("Coordinates: (" + x + "," + y + ")");
}