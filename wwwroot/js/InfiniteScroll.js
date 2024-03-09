function EventInfiniteScroll(divId, route) {
  var xmlhttp = new XMLHttpRequest();
  const divBody = document.getElementById(divId);
  this.route = route;
  this.loading = false;
  this.hasItem = true;
  this.category = "";
  this.page = 1;
  this.perPage = 10;
  this.data = "";
  var self = this;

  this.setCategory = function (category) {
    self.category = category;
  };

  this.loadEvent = function (refresh = false) {
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
        "category=" +
        encodeURIComponent(self.category) +
        "&page=" +
        encodeURIComponent(self.page) +
        "&perPage=" +
        encodeURIComponent(self.perPage);

      self.loading = true;
      xmlhttp.open("GET", self.route + "?" + self.data, true);
      xmlhttp.send();
    }
  };

  window.onscroll = function (ev) {
    if (
      window.innerHeight + window.scrollY >=
      document.documentElement.scrollHeight
    ) {
      if (!self.loading) {
        self.loadEvent(false);
      }
    }
  };
}
