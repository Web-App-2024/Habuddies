var block = document.getElementById("block");
var down = false;

function toggleNotification() {
  if (down) {
    block.style.height = "0px";
    block.style.opacity = 0;
    down = false;
  } else {
    block.style.height = "510px";
    block.style.opacity = 1;
    down = true;
  }
}

var btn = document.getElementById("btn");
var owner = document.getElementById("notification-owner");
var user = document.getElementById("notification-user");

function leftClick() {
  btn.style.left = "0";
  user.style.display = "Block";
  owner.style.display = "None";
}

function rightClick() {
  btn.style.left = "125px";
  user.style.display = "None";
  owner.style.display = "Block";
}
