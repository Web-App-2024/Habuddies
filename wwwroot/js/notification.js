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
