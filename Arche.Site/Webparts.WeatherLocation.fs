namespace Arche.Webparts

open WebSharper
open WebSharper.UI.Next
open Arche
open Arche.Modules

module WeatherLocation =
    
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next.Html
        
        let page() =
            let (locationDoc, locationView) = 
                LocationPicker.Client.page()

            div [ div [ Weather.Client.page locationView ]
                  div [ locationDoc ] ]