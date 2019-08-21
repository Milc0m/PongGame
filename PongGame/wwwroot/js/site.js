// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function moveSection(idStr, xOffset, yOffset) {
    console.log("called");
    var domElemnt = document.getElementById(idStr);
    if (domElemnt) {
        var transformAttr = ' translate(' + xOffset + ',' + yOffset + ')';
        domElemnt.setAttribute('transform', transformAttr);
    }
}

function startGame(idStr) {
    var intervalID = setInterval(function () { getCoords(idStr) }, 1000);
}



 
function getCoords(idStr) {

    $.ajax({
        type: "POST",
        url: 'OnPost',
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        dataType: "json"
    }).done(function (data) {
        console.log(data);
        moveSection(idStr, data, 1);
    })
}
 