namespace Arche.Modules

open FSharp.Data
open WebSharper
open WebSharper.UI.Next

module Weather =
    
    type private Forecast = {
        Title: string
        Description: string
        ImageUrl: string
        Temperature: decimal
        TemparatureMinMax: decimal * decimal
    }

    module private Server =
        
        type WeatherApi = 
            JsonProvider<""" {"coord":{"lon":-0.13,"lat":51.51},"weather":[{"id":500,"main":"Rain","description":"light rain","icon":"10d"},{"id":311,"main":"Drizzle","description":"rain and drizzle","icon":"09d"}],"base":"cmc stations","main":{"temp":282.66,"pressure":999,"humidity":87,"temp_min":281.75,"temp_max":283.15},"wind":{"speed":5.7,"deg":140},"rain":{"1h":0.35},"clouds":{"all":75},"dt":1451724700,"sys":{"type":1,"id":5091,"message":0.0043,"country":"GB","sunrise":1451721965,"sunset":1451750585},"id":2643743,"name":"London","cod":200} """>

        let apiKey =
            Arche.Common.Config.value.OpenWeather.ApiKey
        
        [<Rpc>]
        let get city =
            async {
                let! weather = WeatherApi.AsyncLoad(sprintf "http://api.openweathermap.org/data/2.5/weather?q=%s&units=metric&appid=%s" city apiKey)
                return weather.Weather 
                       |> Array.tryHead
                       |> Option.map (fun head -> 
                            { Title = sprintf "%s, %s" weather.Name weather.Sys.Country
                              Description = head.Main
                              ImageUrl = sprintf "http://openweathermap.org/img/w/%s.png" head.Icon
                              Temperature = weather.Main.Temp
                              TemparatureMinMax = weather.Main.TempMin, weather.Main.TempMax })
            }

    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next.Html
        open WebSharper.UI.Next.Client
        
        let page city =
            city
            |> View.MapAsync Server.get
            |> View.Map (function
                | Some forecast ->
                    let (min, max) = 
                        forecast.TemparatureMinMax

                    let temperature txt =
                        span [ text txt; spanAttr [] [ text "°C" ] ]
                    
                    div [ imgAttr [ attr.src forecast.ImageUrl ] []
                          divAttr [ attr.style "" ] 
                                  [ temperature (sprintf "%i" <| int forecast.Temperature)
                                    divAttr [] [ text forecast.Title ] ]
                          pAttr [] [ temperature (sprintf "Min: %i" <| int min)
                                     text " - "
                                     temperature (sprintf "Max: %i" <| int max) ] ]
                | None -> p [ text "No forecrast" ])
            |> Doc.EmbedView