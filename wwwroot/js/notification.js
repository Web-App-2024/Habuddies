function NotiManager(NotiDiv, MessageScroll) {
  this.display = false;
  this.showJoinNoti = true;
  this.div = NotiDiv
  this.MessageScroll = MessageScroll;
  var self = this;

  this.loadNotification = function (refresh = false) {
    if (self.display) {
      if (self.showJoinNoti) {
        self.MessageScroll.setRouteJoin();
      }
      else {
        self.MessageScroll.setRouteOwner();
      }
      self.MessageScroll.loadNoti(self.div.notiMessage, refresh);
    }
  }

  this.toggleNotification = function () {
    if (self.display) {
      self.div.block.style.height = "0px";
      self.div.block.style.opacity = 0;
      self.display = false;
    } else {
      self.div.block.style.height = "510px";
      self.div.block.style.opacity = 1;
      self.display = true;
      self.loadNotification(true);
    }
  }

  self.div.button.addEventListener('click', function() {
    self.toggleNotification();
  });

  self.div.myNoti.addEventListener('click', function() {
    self.div.showButton.style.left = "0";
    self.showJoinNoti = true;
    self.loadNotification(true);
  });

  self.div.myPost.addEventListener('click', function() {
    self.div.showButton.style.left = "125px";
    self.showJoinNoti = false;
    self.loadNotification(true);
  });

  self.div.block.addEventListener("scroll", function () {
    if (self.div.block.scrollTop*2 >= self.div.block.scrollHeight) {
      self.loadNotification();
    }
  });
}

class NotificationDiv {
  constructor(toggleButtonId ,notiBlockId, myNotiButtonId, myPostButtonId, showButtonId, notiMessageId) {
    this.button = document.getElementById(toggleButtonId);
    this.block = document.getElementById(notiBlockId);
    this.myNoti = document.getElementById(myNotiButtonId);
    this.myPost = document.getElementById(myPostButtonId);
    this.showButton = document.getElementById(showButtonId);
    this.notiMessage = document.getElementById(notiMessageId);
  }
}

function NotiMessageLoader(routeOwner, routeJoin) { 
  var xmlhttp = new XMLHttpRequest();
  this.routeOwner = routeOwner;
  this.routeJoin = routeJoin;
  this.route = routeOwner;
  this.loading = false;
  this.hasItem = true;
  this.page = 1;
  this.perPage = 10;
  this.data = "";
  var self = this;

  this.setRouteOwner = function () {
    self.route = self.routeOwner;
  }

  this.setRouteJoin = function () {
    self.route = self.routeJoin;
  }

  this.loadNoti = function (divBody, refresh = false) {
    xmlhttp.onreadystatechange = function () {
      if (xmlhttp.readyState == XMLHttpRequest.DONE) {
        if (xmlhttp.status == 200) {
          divBody.insertAdjacentHTML("beforeEnd", xmlhttp.responseText);
          self.loading = false;
          self.page += 1;
        } else if (xmlhttp.status == 204) {
          self.hasItem = false;
        }
      }
    };

    if (refresh) {
      divBody.innerHTML = "";
      self.page = 1;
      self.hasItem = true;
    }

    if (self.hasItem) {
      self.data =
        "&page=" +
        encodeURIComponent(self.page) +
        "&perPage=" +
        encodeURIComponent(self.perPage);

      self.loading = true;
      xmlhttp.open("GET", self.route + "?" + self.data, true);
      xmlhttp.send();
    }
  };
}
