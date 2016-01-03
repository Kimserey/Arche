namespace Arche.Webparts

open WebSharper
open WebSharper.UI.Next
open Arche
open Arche.Modules
open Arche.Common.Bootstrap

module Map =
    
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next.Html
        
        let webpart() =
            let (locationDoc, locationView) = 
                LocationPicker.Client.page()

            panel "Map" [ div [ Map.Client.page locationView ]
                          div [ locationDoc ] ]