namespace Arche.Modules

open WebSharper
open Arche.Common

module Map =

    module Server =
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
                return geocode.Results 
                       |> Array.tryHead 
                       |> Option.map (fun head -> 
                            { Lng = head.Geometry.Location.Lng
                              Lat = head.Geometry.Location.Lat })
            }

    module Resource =
        open WebSharper.Resources

        module GoogleMap =
            
            type Js() =
                inherit BaseResource(sprintf "https://maps.googleapis.com/maps/api/js?key=%s&signed_in=true" Config.value.GoogleMap.ApiKey)


    [<JavaScript; Require(typeof<Resource.GoogleMap.Js>)>]
    module Client =
        open WebSharper.UI.Next
        open WebSharper.UI.Next.Html
        open WebSharper.UI.Next.Client
        
        let page city =
            city 
            |> View.MapAsync Server.geocode
            |> View.Map (function
                | Some geocode -> text (sprintf "%A" geocode)
                | None -> text "0")
            |> Doc.EmbedView