// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function ClickJoinOrLeave(element) {
    var bannerId = element.parentElement.id;
    if (element.classList.contains("join")) {
        alert("Joined " + bannerId);
    }
    else {
        alert("Left " + bannerId);
    }
}