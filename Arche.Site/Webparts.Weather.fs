namespace Arche.Webparts

open WebSharper
open WebSharper.UI.Next
open Arche
open Arche.Modules

module Weather =
    
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next.Html
        
        let webpart() =
            let (locationDoc, locationView) = 
                LocationPicker.Client.page()

            div [ div [ Weather.Client.page locationView ]
                  div [ locationDoc ] ]