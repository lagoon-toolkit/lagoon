// Init app when the DOM is ready
document.addEventListener("DOMContentLoaded", function () {
	// Insert link
	var includeLink = (function (url) {
		var link = document.createElement("link");
		//link.type = "text/css";
		link.rel = "stylesheet";
		link.href = url;
		document.body.appendChild(link);
	});
	// Insert script
	var includeJS = (function (url) {
		var script = document.createElement("script");
		script.async = false;
		script.defer = false;
		script.src = url;
		document.body.appendChild(script);
	});


	// Load additionals javascripts/css
	includeJS("_content/Lagoon.UI.Leaflet/js/leafletBlazorInterops.js");

	includeJS("_content/Lagoon.UI.Leaflet/js/leaflet/dist/leaflet.js");
	includeLink("_content/Lagoon.UI.Leaflet/js/leaflet/dist/leaflet.css");

	includeJS("_content/Lagoon.UI.Leaflet/js/leaflet-draw/dist/leaflet.draw.js");
	includeLink("_content/Lagoon.UI.Leaflet/js/leaflet-draw/dist/leaflet.draw.css");

	includeJS("_content/Lagoon.UI.Leaflet/js/leaflet-control-geocoder/dist/Control.Geocoder.js");
	includeLink("_content/Lagoon.UI.Leaflet/js/leaflet-control-geocoder/dist/Control.Geocoder.css");

	includeJS("_content/Lagoon.UI.Leaflet/js/leaflet-routing-machine/dist/leaflet-routing-machine.js");
	includeLink("_content/Lagoon.UI.Leaflet/js/leaflet-routing-machine/dist/leaflet-routing-machine.css");

	includeJS("_content/Lagoon.UI.Leaflet/js/leaflet-overpass-layer/dist/OverPassLayer.bundle.js");
	includeLink("_content/Lagoon.UI.Leaflet/js/leaflet-overpass-layer/dist/OverPassLayer.css");

});