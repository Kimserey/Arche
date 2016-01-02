namespace Arche.Modules

open WebSharper
open Arche.Common

module Map =

    module private Resource =
        open WebSharper.Resources

        module GoogleMap =
            
            type Js() =
                inherit BaseResource(sprintf "https://maps.googleapis.com/maps/api/js?key=%s&signed_in=true" Config.value.GoogleMap.ApiKey)


    module private Server =
        open FSharp.Data

        [<Literal>]
        let private Sample = """ { "results" : [ { "address_components" : [ { "long_name" : "Augsburg", "short_name" : "A", "types" : [ "locality", "political" ] }, 
                                                                    { "long_name" : "Swabia", "short_name" : "Swabia", "types" : [ "administrative_area_level_2", "political" ] },
                                                                    { "long_name" : "Bavaria", "short_name" : "BY", "types" : [ "administrative_area_level_1", "political" ] },
                                                                    { "long_name" : "Germany", "short_name" : "DE", "types" : [ "country", "political" ] } ],
                                           "formatted_address" : "Augsburg, Germany", 
                                           "geometry" : { "bounds" : { "northeast" : { "lat" : 48.458654, "lng" : 10.959519 }, "southwest" : { "lat" : 48.2581444, "lng" : 10.7633614 } }, "location" : { "lat" : 48.3705449, "lng" : 10.89779 }, "location_type" : "APPROXIMATE", "viewport" : { "northeast" : { "lat" : 48.458654, "lng" : 10.959519 }, "southwest" : { "lat" : 48.2581444, "lng" : 10.7633614 } } },
                                           "place_id" : "ChIJKSshOsWinkcRoOeL161IHgQ", 
                                           "types" : [ "locality", "political" ] } ],
                          "status" : "OK" } """

        type private GeocoderJson = JsonProvider<Sample>

        type Coordinates = {
            Lng: decimal
            Lat: decimal
        }

        [<Rpc>]
        let geocode city =
            async {
                let! geocode = GeocoderJson.AsyncLoad(sprintf "https://maps.googleapis.com/maps/api/geocode/json?address=%s&key=%s" city Config.value.GoogleMap.ApiKey)
                
                return match geocode.Status, geocode.Results |> Array.toList with
                       | status, head::t when status = "OK" -> 
                            Some { Lng = head.Geometry.Location.Lng
                                   Lat = head.Geometry.Location.Lat }
                       | _ -> None
            }

    [<JavaScript; Require(typeof<Resource.GoogleMap.Js>)>]
    module Client =
        open WebSharper.UI.Next
        open WebSharper.UI.Next.Html
        open WebSharper.UI.Next.Client
        open WebSharper.JavaScript

        module private GoogleMap =
            [<Direct(""" var mapOptions = { center: new google.maps.LatLng($lat, $lng), zoom: 12 }; return new google.maps.Map($elm, mapOptions); """)>]
            let init elm lat lng = X<unit>

        let page city =
            city
            |> View.MapAsync Server.geocode
            |> View.Map (function
                | Some geocode ->
                    divAttr [ attr.style "width: 500px; height: 500px;"
                              on.afterRender (fun div -> do GoogleMap.init div geocode.Lat geocode.Lng) ] [] :> Doc
                | None -> text "Location not found.")
            |> Doc.EmbedView