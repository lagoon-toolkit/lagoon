maps = {};
layers = {};
mapsObjectReferences = {};
mapsDrawnItems = {};
mapsLayerControls = {};
mapsOverPassLayers = {};

window.leafletBlazor = {
    create: function (map, objectReference) {
        var leafletMap = L.map(map.id, {
            center: map.center,
            zoom: map.zoom,
            zoomControl: map.zoomControl,
            minZoom: map.minZoom ? map.minZoom : undefined,
            maxZoom: map.maxZoom ? map.maxZoom : undefined,
            maxBounds: map.maxBounds && map.maxBounds.item1 && map.maxBounds.item2 ? L.latLngBounds(map.maxBounds.item1, map.maxBounds.item2) : undefined,
        });

        connectMapEvents(leafletMap, objectReference);
        maps[map.id] = leafletMap;
        layers[map.id] = [];
        mapsObjectReferences[map.id] = objectReference;
    },
    addTilelayer: function (mapId, tileLayer, objectReference) {
        const layer = L.tileLayer(tileLayer.urlTemplate, {
            attribution: tileLayer.attribution,
            pane: tileLayer.pane,
            // ---
            tileSize: tileLayer.tileSize ? L.point(tileLayer.tileSize.width, tileLayer.tileSize.height) : undefined,
            opacity: tileLayer.opacity,
            updateWhenZooming: tileLayer.updateWhenZooming,
            updateInterval: tileLayer.updateInterval,
            zIndex: tileLayer.zIndex,
            bounds: tileLayer.bounds && tileLayer.bounds.item1 && tileLayer.bounds.item2 ? L.latLngBounds(tileLayer.bounds.item1, tileLayer.bounds.item2) : undefined,
            // ---
            minZoom: tileLayer.minimumZoom,
            maxZoom: tileLayer.maximumZoom,
            subdomains: tileLayer.subdomains,
            errorTileUrl: tileLayer.errorTileUrl,
            zoomOffset: tileLayer.zoomOffset,
            // TMS
            zoomReverse: tileLayer.isZoomReversed,
            detectRetina: tileLayer.detectRetina,
            // crossOrigin,
            noWrap: tileLayer.noWrap || false,
        });
        addLayer(mapId, layer, tileLayer.id);
    },
    addMbTilesLayer: function (mapId, mbTilesLayer, objectReference) {
        const layer = L.tileLayer.mbTiles(mbTilesLayer.urlTemplate, {
            attribution: mbTilesLayer.attribution,
            minZoom: mbTilesLayer.minimumZoom,
            maxZoom: mbTilesLayer.maximumZoom
        });
        addLayer(mapId, layer, mbTilesLayer.id);
    },
    addShapefileLayer: function (mapId, shapefileLayer, objectReference) {
        const layer = L.shapefile(shapefileLayer.urlTemplate);
        addLayer(mapId, layer, shapefileLayer.id);
    },
    addMarker: function (mapId, marker, objectReference) {
        var options = {
            ...createInteractiveLayer(marker),
            keyboard: marker.isKeyboardAccessible,
            title: marker.title,
            alt: marker.alt,
            zIndexOffset: marker.zIndexOffset,
            opacity: marker.opacity,
            riseOnHover: marker.riseOnHover,
            riseOffset: marker.riseOffset,
            pane: marker.pane,
            bubblingMouseEvents: marker.isBubblingMouseEvents,
            draggable: marker.draggable,
            autoPan: marker.useAutopan,
            autoPanPadding: marker.autoPanPadding,
            autoPanSpeed: marker.autoPanSpeed
        };

        if (marker.icon !== null) {
            options.icon = createIcon(marker.icon);
        }
        const mkr = L.marker(marker.position, options);
        connectMarkerEvents(mkr, objectReference);
        addLayer(mapId, mkr, marker.id);
        setTooltipAndPopupIfDefined(marker, mkr);
    },
    addPolyline: function (mapId, polyline, objectReference) {
        const layer = L.polyline(shapeToLatLngArray(polyline.shape), createPolyline(polyline));
        addLayer(mapId, layer, polyline.id);
        setTooltipAndPopupIfDefined(polyline, layer);
    },
    updatePolyline: function (mapId, polyline) {
        let layer = layers[mapId].find(l => l.id === polyline.id);
        if (layer !== undefined) {
            layer.setLatLngs(shapeToLatLngArray(polyline.shape));
        }
    },
    addPolygon: function (mapId, polygon, objectReference) {
        const layer = L.polygon(shapeToLatLngArray(polygon.shape), createPolyline(polygon));
        addLayer(mapId, layer, polygon.id);
        setTooltipAndPopupIfDefined(polygon, layer);
    },
    updatePolygon: function (mapId, polygon) {
        let layer = layers[mapId].find(l => l.id === polygon.id);
        if (layer !== undefined) {
            layer.setLatLngs(shapeToLatLngArray(polygon.shape));
        }
    },
    addRectangle: function (mapId, rectangle, objectReference) {
        const layer = L.rectangle([[rectangle.shape.bottom, rectangle.shape.left], [rectangle.shape.top, rectangle.shape.right]], createPolyline(rectangle));
        addLayer(mapId, layer, rectangle.id);
        setTooltipAndPopupIfDefined(rectangle, layer);
    },
    updateRectangle: function (mapId, rectangle) {
        let layer = layers[mapId].find(l => l.id === rectangle.id);
        if (layer !== undefined) {
            layer.setBounds([[rectangle.shape.bottom, rectangle.shape.left], [rectangle.shape.top, rectangle.shape.right]]);
        }
    },
    addCircle: function (mapId, circle, objectReference) {
        const layer = L.circle(circle.position,
            {
                ...createPath(circle),
                radius: circle.radius
            });
        addLayer(mapId, layer, circle.id);
        setTooltipAndPopupIfDefined(circle, layer);
    },
    updateCircle: function (mapId, circle) {
        let layer = layers[mapId].find(l => l.id === circle.id);
        if (layer !== undefined) {
            layer.setRadius(circle.radius);
            layer.setLatLng(circle.position);
        }
    },
    addImageLayer: function (mapId, image, objectReference) {
        const layerOptions = {
            ...createInteractiveLayer(image),
            opacity: image.opacity,
            alt: image.alt,
            crossOrigin: image.crossOrigin,
            errorOverlayUrl: image.errorOverlayUrl,
            zIndex: image.zIndex,
            className: image.className
        };

        const corner1 = L.latLng(image.corner1.x, image.corner1.y);
        const corner2 = L.latLng(image.corner2.x, image.corner2.y);
        const bounds = L.latLngBounds(corner1, corner2);

        const imgLayer = L.imageOverlay(image.url, bounds, layerOptions);
        addLayer(mapId, imgLayer);
    },
    addGeoJsonLayer: function (mapId, geodata, objectReference) {
        const geoDataObject = JSON.parse(geodata.geoJsonData);
        var options = {
            ...createInteractiveLayer(geodata),
            title: geodata.title,
            bubblingMouseEvents: geodata.isBubblingMouseEvents,
            pointToLayer: (feature, latlng) => {
                if (feature.properties.radius) {
                    return new L.Circle(latlng, feature.properties.radius);
                } else {
                    return new L.Marker(latlng);
                }
            },
            onEachFeature: function onEachFeature(feature, layer) {
                connectInteractionEvents(layer, objectReference);
            }
        };

        const geoJsonLayer = L.geoJson(geoDataObject, options);
        addLayer(mapId, geoJsonLayer, geodata.id);
    },
    removeLayer: function (mapId, layerId) {
        const remainingLayers = layers[mapId].filter((layer) => layer.id !== layerId);
        const layersToBeRemoved = layers[mapId].filter((layer) => layer.id === layerId); // should be only one ...
        layers[mapId] = remainingLayers;

        layersToBeRemoved.forEach(m => m.removeFrom(maps[mapId]));
    },
    updatePopupContent: function (mapId, layerId, content) {
        let layer = layers[mapId].find(l => l.id === layerId);
        if (layer !== undefined) {
            var popup = layer.getPopup();
            if (popup !== undefined) {
                popup.setContent(content);
            }
        }
    },
    updateTooltipContent: function (mapId, layerId, content) {
        let layer = layers[mapId].find(l => l.id === layerId);
        if (layer !== undefined) {
            var tooltip = layer.getTooltip();
            if (tooltip !== undefined) {
                tooltip.setContent(content);
            }
        }
    },
    fitBounds: function (mapId, corner1, corner2, padding, maxZoom) {
        const corner1LL = L.latLng(corner1.x, corner1.y);
        const corner2LL = L.latLng(corner2.x, corner2.y);
        const mapBounds = L.latLngBounds(corner1LL, corner2LL);
        maps[mapId].fitBounds(mapBounds, {
            padding: padding == null ? null : L.point(padding.x, padding.y),
            maxZoom: maxZoom
        });
    },
    panTo: function (mapId, position, animate, duration, easeLinearity, noMoveStart) {
        const pos = L.latLng(position.x, position.y);
        maps[mapId].panTo(pos, {
            animate: animate,
            duration: duration,
            easeLinearity: easeLinearity,
            noMoveStart: noMoveStart
        });
    },
    getCenter: function (mapId) {
        return maps[mapId].getCenter();
    },
    getZoom: function (mapId) {
        return maps[mapId].getZoom();
    },
    zoomIn: function (mapId, e) {
        const map = maps[mapId];

        if (map.getZoom() < map.getMaxZoom()) {
            map.zoomIn(map.options.zoomDelta * (e.shiftKey ? 3 : 1));
        }
    },
    zoomOut: function (mapId, e) {
        const map = maps[mapId];

        if (map.getZoom() > map.getMinZoom()) {
            map.zoomOut(map.options.zoomDelta * (e.shiftKey ? 3 : 1));
        }
    },
    locate: function (mapId, watch, setView, maxZoom, timeout, maximumAge, enableHighAccuracy) {
        maps[mapId].locate({
            watch: watch,
            setView: setView,
            maxZoom: maxZoom,
            timeout: timeout,
            maximumAge: maximumAge,
            enableHighAccuracy: enableHighAccuracy
        });
    },
    stopLocate: function (mapId) {
        maps[mapId].stopLocate();
    },
    addDrawControl: function (mapId) {
        // FeatureGroup is to store editable layers
        let drawnItems = new L.FeatureGroup();
        maps[mapId].addLayer(drawnItems);
        var drawControl = new L.Control.Draw({
            edit: {
                featureGroup: drawnItems
            }
        });
        maps[mapId].addControl(drawControl);
        mapsDrawnItems[mapId] = drawnItems;

        maps[mapId].on("draw:created", function (e) {
            mapsObjectReferences[mapId].invokeMethodAsync("NotifyDrawCreated", ConvertLayertoGeoJSON(e.layer, e.layerType))
        });

        maps[mapId].on("draw:edited", function (e) {
            let editedLayers = new Object();
            e.layers.eachLayer(layer => {
                editedLayers[layer.id] = ConvertLayertoGeoJSON(layer, layer.layerType);
            });
            mapsObjectReferences[mapId].invokeMethodAsync("NotifyDrawEdited", editedLayers);
        });

        maps[mapId].on("draw:deleted", function (e) {
            let listIds = [];
            e.layers.eachLayer(layer => {
                listIds.push(layer.id);
            });
            mapsObjectReferences[mapId].invokeMethodAsync("NotifyDrawDeleted", listIds);
        });
    },
    toGeoJSON: function (mapId, layerId) {
        var layerFound = layers[mapId].find(elem => elem.id == layerId);
        return ConvertLayertoGeoJSON(layerFound, layerFound.layerType);
    },
    setMaxBounds: function (mapId, bounds) {
        maps[mapId].setMaxBounds(bounds && bounds.item1 && bounds.item2 ? L.latLngBounds(bounds.item1, bounds.item2) : undefined);
    },
    addBaseLayer(mapId, layerId, name) {
        var layerFound = layers[mapId].find(elem => elem.id == layerId);

        if (mapsLayerControls[mapId] == null) {
            mapsLayerControls[mapId] = L.control.layers().addTo(maps[mapId]);
        }

        mapsLayerControls[mapId].addBaseLayer(layerFound, name);
    },
    addOverlay(mapId, layerId, name) {
        var layerFound = layers[mapId].find(elem => elem.id == layerId);

        if (mapsLayerControls[mapId] == null) {
            mapsLayerControls[mapId] = L.control.layers().addTo(maps[mapId]);
        }

        mapsLayerControls[mapId].addOverlay(layerFound, name);
    },
    addSearchControl(mapId) {
        L.Control.geocoder({
            geocoder: L.Control.Geocoder.nominatim()
        }).addTo(maps[mapId]);
    },
    addOsrmRoutingControl(mapId, routingControlOptions) {
        // Create a plan for the routing engine
        // This plan will create specific geocoding buttons
        // Extend L.Routing.Plan to create a custom plan for Osrm
        var geoOsrmPlan = L.Routing.Plan.extend({

            createGeocoders: function () {

                var container = L.Routing.Plan.prototype.createGeocoders.call(this);

                // Create a reverse waypoints button
                reverseButton = createButton('<img src="_content/Lagoon.UI.Leaflet/js/images/reverse.svg" width="15px" height="20px"' + 'style="-webkit-clip-path: inset(0 0 5px 0); -moz-clip-path: inset(0 0 5px 0); clip-path: inset(0 0 5px 0);">', "Reverse waypoints", container);

                // Create a button for walking routes
                walkButton = createButton('<img src="_content/Lagoon.UI.Leaflet/js/images/walk.svg" width="15px" height="20px"' + 'style="-webkit-clip-path: inset(0 0 5px 0); -moz-clip-path: inset(0 0 5px 0); clip-path: inset(0 0 5px 0);">', "Walking route", container);

                // Create a button for biking routes
                bikeButton = createButton('<img src="_content/Lagoon.UI.Leaflet/js/images/bike.svg" width="15px" height="20px"' + 'style="-webkit-clip-path: inset(0 0 5px 0); -moz-clip-path: inset(0 0 5px 0); clip-path: inset(0 0 5px 0);">', "Biking route", container);

                // Create a button for driving routes
                carButton = createButton('<img src="_content/Lagoon.UI.Leaflet/js/images/car.svg" width="15px" height="20px"' + 'style="-webkit-clip-path: inset(0 0 5px 0); -moz-clip-path: inset(0 0 5px 0); clip-path: inset(0 0 5px 0);">', "Car route", container);


                // Event to reverse the waypoints
                L.DomEvent.on(reverseButton, 'click', function () {
                    var waypoints = this.getWaypoints();
                    this.setWaypoints(waypoints.reverse());
                }, this);

                // Event to generate walking routes
                L.DomEvent.on(walkButton, 'click', function () {
                    osrmRouting.getRouter().options.profile = 'walking';
                    osrmRouting.route();
                    osrmRouting.setWaypoints(osrmRouting.getWaypoints());
                }, this);

                // Event to generate biking routes
                L.DomEvent.on(bikeButton, 'click', function () {
                    osrmRouting.getRouter().options.profile = 'cycling';
                    osrmRouting.route();
                    osrmRouting.setWaypoints(osrmRouting.getWaypoints());
                }, this);

                // Event to generate driving routes
                L.DomEvent.on(carButton, 'click', function () {
                    osrmRouting.getRouter().options.profile = 'driving';
                    osrmRouting.route();
                    osrmRouting.setWaypoints(osrmRouting.getWaypoints());
                }, this);

                // Event to generate alternative routes
                //L.DomEvent.on(altButton, 'click', function () {
                //    osrmRouting.getRouter().options.urlParameters.algorithm = 'alternativeroute';
                //    osrmRouting.route();
                //    osrmRouting.setWaypoints(osrmRouting.getWaypoints());
                //}, this);

                return container;
            }
        });

        // Create a plan for the routing
        var osrmPlan = new geoOsrmPlan(

            routingControlOptions.waypoints,

            {
                // Default geocoder
                geocoder: new L.Control.Geocoder.Nominatim(),

                // Create routes while dragging markers
                routeWhileDragging: true,
            });

        // Call the Osrm routing engine
        osrmRouting = L.Routing.control({
            waypoints: routingControlOptions.waypoints,
            collapsible: routingControlOptions.collapsible ? routingControlOptions.collapsible : undefined,
            showAlternatives: routingControlOptions.showAlternatives,
            router: L.Routing.osrmv1({
                serviceUrl: routingControlOptions.router.serviceUrl,
                timeout: routingControlOptions.router.timeout,
                profile: routingControlOptions.router.profile,
                polylinePrecision: routingControlOptions.router.polylinePrecision,
                useHints: routingControlOptions.router.useHints
            }),
            geocoder: L.Control.Geocoder.nominatim(),
            // Use the created plan for Osrm routing
            plan: osrmPlan,
        }).addTo(maps[mapId]);
    },
    addMapBoxRoutingControl(mapId, accessToken, routingControlOptions) {
        // Create a plan for the routing engine
        // This plan will create specific geocoding buttons
        // Extend L.Routing.Plan to create a custom plan for MapBox
        var geoMapBoxPlan = L.Routing.Plan.extend({

            createGeocoders: function () {

                var container = L.Routing.Plan.prototype.createGeocoders.call(this);

                // Create a reverse waypoints button
                reverseButton = createButton('<img src="_content/Lagoon.UI.Leaflet/js/images/reverse.svg" width="15px" height="20px"' + 'style="-webkit-clip-path: inset(0 0 5px 0); -moz-clip-path: inset(0 0 5px 0); clip-path: inset(0 0 5px 0);">', "Reverse waypoints", container);

                // Create a button for walking routes
                walkButton = createButton('<img src="_content/Lagoon.UI.Leaflet/js/images/walk.svg" width="15px" height="20px"' + 'style="-webkit-clip-path: inset(0 0 5px 0); -moz-clip-path: inset(0 0 5px 0); clip-path: inset(0 0 5px 0);">', "Walking route", container);

                // Create a button for biking routes
                bikeButton = createButton('<img src="_content/Lagoon.UI.Leaflet/js/images/bike.svg" width="15px" height="20px"' + 'style="-webkit-clip-path: inset(0 0 5px 0); -moz-clip-path: inset(0 0 5px 0); clip-path: inset(0 0 5px 0);">', "Biking route", container);

                // Create a button for driving routes
                carButton = createButton('<img src="_content/Lagoon.UI.Leaflet/js/images/car.svg" width="15px" height="20px"' + 'style="-webkit-clip-path: inset(0 0 5px 0); -moz-clip-path: inset(0 0 5px 0); clip-path: inset(0 0 5px 0);">', "Car route", container);


                // Event to reverse the waypoints
                L.DomEvent.on(reverseButton, 'click', function () {
                    var waypoints = this.getWaypoints();
                    this.setWaypoints(waypoints.reverse());
                }, this);

                // Event to generate walking routes
                L.DomEvent.on(walkButton, 'click', function () {
                    mapBoxRouting.getRouter().options.profile = 'mapbox/walking';
                    mapBoxRouting.route();
                    mapBoxRouting.setWaypoints(mapBoxRouting.getWaypoints());
                }, this);

                // Event to generate biking routes
                L.DomEvent.on(bikeButton, 'click', function () {
                    mapBoxRouting.getRouter().options.profile = 'mapbox/cycling';
                    mapBoxRouting.route();
                    mapBoxRouting.setWaypoints(mapBoxRouting.getWaypoints());
                }, this);

                // Event to generate driving routes
                L.DomEvent.on(carButton, 'click', function () {
                    mapBoxRouting.getRouter().options.profile = 'mapbox/driving';
                    mapBoxRouting.route();
                    mapBoxRouting.setWaypoints(mapBoxRouting.getWaypoints());
                }, this);

                // Event to generate alternative routes
                //L.DomEvent.on(altButton, 'click', function () {
                //    mapBoxRouting.getRouter().options.urlParameters.algorithm = 'alternativeroute';
                //    mapBoxRouting.route();
                //    mapBoxRouting.setWaypoints(mapBoxRouting.getWaypoints());
                //}, this);

                return container;
            }
        });

        // Create a plan for the routing
        var mapBoxPlan = new geoMapBoxPlan(
            routingControlOptions.waypoints,

            {
                // Default geocoder
                geocoder: new L.Control.Geocoder.Nominatim(),

                // Create routes while dragging markers
                routeWhileDragging: true,
            });

        // Call the MapBox routing engine
        mapBoxRouting = L.Routing.control({
            waypoints: routingControlOptions.waypoints,
            collapsible: routingControlOptions.collapsible ? routingControlOptions.collapsible : undefined,
            showAlternatives: routingControlOptions.showAlternatives,
            router: L.Routing.mapbox(accessToken, routingControlOptions.router),
            geocoder: L.Control.Geocoder.Mapbox(accessToken, {}),
            // Use the created plan for MapBox routing
            plan: mapBoxPlan,
        }).addTo(maps[mapId]);
    },
    addOverPassLayer(mapId, query, endPoint) {
        if (mapsOverPassLayers[mapId]) {
            maps[mapId].removeLayer(mapsOverPassLayers[mapId]);
        }
        var opl = new L.OverPassLayer({
            endPoint: endPoint,
            query: query
        });
        mapsOverPassLayers[mapId] = opl;
        maps[mapId].addLayer(opl);
    }
};

function ConvertLayertoGeoJSON(layerToConvert, layerType) {
    var geoJson = layerToConvert.toGeoJSON();
    if (layerType) {
        geoJson.properties.layerType = layerType;
        if (layerType === "circle" || layerType === "circlemarker")
            geoJson.properties.radius = layerToConvert.getRadius();
    }
    else if (layerToConvert instanceof L.Circle) {
        geoJson.properties.layerType = "circle";
        geoJson.properties.radius = layerToConvert.getRadius();
    }
    return JSON.stringify(geoJson);
}

function createIcon(icon) {
    return L.icon({
        iconUrl: icon.url,
        iconRetinaUrl: icon.retinaUrl,
        iconSize: icon.size ? L.point(icon.size.value.width, icon.size.value.height) : null,
        iconAnchor: icon.anchor ? L.point(icon.anchor.value.x, icon.anchor.value.y) : null,
        popupAnchor: L.point(icon.popupAnchor.x, icon.popupAnchor.y),
        tooltipAnchor: L.point(icon.tooltipAnchor.x, icon.tooltipAnchor.y),
        shadowUrl: icon.shadowUrl,
        shadowRetinaUrl: icon.shadowRetinaUrl,
        shadowSize: icon.shadowSize ? L.point(icon.shadowSize.value.width, icon.shadowSize.value.height) : null,
        shadowSizeAnchor: icon.shadowSizeAnchor ? L.point(icon.shadowSizeAnchor.value.width, icon.shadowSizeAnchor.value.height) : null,
        className: icon.className
    })
}

function shapeToLatLngArray(shape) {
    var latlngs = [];
    shape.forEach(pts => {
        var ll = [];
        pts.forEach(p => ll.push([p.x, p.y]));
        latlngs.push(ll);
    });

    return latlngs;
}

function createPolyline(polyline) {
    return {
        ...createPath(polyline),
        smoothFactor: polyline.smoothFactor,
        noClip: polyline.noClipEnabled
    };
}

function createPath(path) {
    return {
        ...createInteractiveLayer(path),
        stroke: path.drawStroke,
        color: getColorString(path.strokeColor),
        weight: path.strokeWidth,
        opacity: path.strokeOpacity,
        lineCap: path.lineCap,
        lineJoin: path.lineJoin,
        dashArray: path.strokeDashArray,
        dashOffset: path.strokeDashOffset,
        fill: path.fill,
        fillColor: getColorString(path.fillColor),
        fillOpacity: path.fillOpacity,
        fillRule: path.fillRule
    };
}

function createInteractiveLayer(layer) {
    return {
        ...createLayer(layer),
        interactive: layer.isInteractive,
        bubblingMouseEvents: layer.isBubblingMouseEvents
    };
}

function createLayer(obj) {
    return {
        id: obj.id,
        pane: obj.pane,
        attribution: obj.attribution
    };
}

function getColorString(color) {
    return "rgb(" + color.r + "," + color.g + "," + color.b + ")";
}

function unbindTooltipAndPopupIfDefined(layer) {
    if (layer.getTooltip()) {
        layer.unbindTooltip();
    }
    if (layer.getPopup()) {
        layer.unbindPopup();
    }
}

function setTooltipAndPopupIfDefined(layer, jsLayer) {
    if (layer.tooltip) {
        addTooltip(jsLayer, layer.tooltip);
    }
    if (layer.popup) {
        addPopup(jsLayer, layer.popup);
    }
}

function addTooltip(layerObj, tooltip) {
    layerObj.bindTooltip(tooltip.content,
        {
            pane: tooltip.pane,
            offset: L.point(tooltip.offset.x, tooltip.offset.y),
            direction: tooltip.direction,
            permanent: tooltip.isPermanent,
            sticky: tooltip.isSticky,
            opacity: tooltip.opacity
        });
}

function addPopup(layerObj, popup) {
    layerObj.bindPopup(popup.content, {
        pane: popup.pane,
        className: popup.className,
        maxWidth: popup.maximumWidth,
        minWidth: popup.minimumWidth,
        autoPan: popup.autoPanEnabled,
        autoPanPaddingTopLeft: popup.autoPanPaddingTopLeft ? L.point(popup.autoPanPaddingTopLeft.x, popup.autoPanPaddingTopLeft.y) : null,
        autoPanPaddingBottomRight: popup.autoPanPaddingBottomRight ? L.point(popup.autoPanPaddingBottomRight.x, popup.autoPanPaddingBottomRight.y) : null,
        autoPanPadding: L.point(popup.autoPanPadding.x, popup.autoPanPadding.y),
        keepInView: popup.keepInView,
        closeButton: popup.showCloseButton,
        autoClose: popup.autoClose,
        closeOnEscapeKey: popup.closeOnEscapeKey,
    });
}

function addLayer(mapId, layer, layerId) {
    layer.id = layerId;
    layers[mapId].push(layer);
    layer.addTo(maps[mapId]);
    if (layer.editing && mapsDrawnItems[mapId]) {
        mapsDrawnItems[mapId].addLayer(layer);
    }
}

// #region events

// removes properties that can cause circular references
function cleanupEventArgsForSerialization(eventArgs) {

    const propertiesToRemove = [
        "target",
        "sourceTarget",
        "propagatedFrom",
        "originalEvent",
        "tooltip",
        "popup"
    ];

    const copy = {};

    for (let key in eventArgs) {
        if (!propertiesToRemove.includes(key) && eventArgs.hasOwnProperty(key)) {
            copy[key] = eventArgs[key];
        }
    }

    return copy;
}

function mapEvents(mapElement, objectReference, eventHandlerDict) {
    for (let key in eventHandlerDict) {

        const handlerName = eventHandlerDict[key];

        mapElement.on(key, function (eventArgs) {
            objectReference.invokeMethodAsync(handlerName,
                cleanupEventArgsForSerialization(eventArgs));
        });
    }
}

function connectMapEvents(map, objectReference) {

    connectInteractionEvents(map, objectReference);

    mapEvents(map, objectReference, {
        "zoomlevelschange": "NotifyZoomLevelsChange",
        "resize": "NotifyResize",
        "unload": "NotifyUnload",
        "viewreset": "NotifyViewReset",
        "load": "NotifyLoad",
        "zoomstart": "NotifyZoomStart",
        "movestart": "NotifyMoveStart",
        "zoom": "NotifyZoom",
        "move": "NotifyMove",
        "zoomend": "NotifyZoomEnd",
        "moveend": "NotifyMoveEnd",
        "mousemove": "NotifyMouseMove",
        "keypress": "NotifyKeyPress",
        "keydown": "NotifyKeyDown",
        "keyup": "NotifyKeyUp",
        "preclick": "NotifyPreClick",
        "locationfound": "NotifyLocationFound"
    });
}

function connectLayerEvents(layer, objectReference) {
    mapEvents(layer, objectReference, {
        "add": "NotifyAdd",
        "remove": "NotifyRemove",
        "popupopen": "NotifyPopupOpen",
        "popupclose": "NotifyPopupClose",
        "tooltipopen": "NotifyTooltipOpen",
        "tooltipclose": "NotifyTooltipClose",
    });
}

function connectInteractiveLayerEvents(interactiveLayer, objectReference) {

    connectLayerEvents(interactiveLayer, objectReference);
    connectInteractionEvents(interactiveLayer, objectReference);
}

function connectMarkerEvents(marker, objectReference) {

    connectInteractiveLayerEvents(marker, objectReference);

    mapEvents(marker, objectReference, {
        "move": "NotifyMove",
        "dragstart": "NotifyDragStart",
        "movestart": "NotifyMoveStart",
        "drag": "NotifyDrag",
        "dragend": "NotifyDragEnd",
        "moveend": "NotifyMoveEnd",
    });
}

function connectInteractionEvents(interactiveObject, objectReference) {

    mapEvents(interactiveObject, objectReference, {
        "click": "NotifyClick",
        "dblclick": "NotifyDblClick",
        "mousedown": "NotifyMouseDown",
        "mouseup": "NotifyMouseUp",
        "mouseover": "NotifyMouseOver",
        "mouseout": "NotifyMouseOut",
        "contextmenu": "NotifyContextMenu",
    });
}

function createButton(label, title, container) {
    var btn = L.DomUtil.create('button', '', container);
    btn.setAttribute('type', 'button');
    btn.innerHTML = label;
    btn.title = title;
    return btn;
}
