namespace Arche.Webparts

open WebSharper
open WebSharper.UI.Next
open Arche
open Arche.Modules
open Arche.Common.Bootstrap

module Weather =
    
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next.Html
        
        let webpart() =
            let (locationDoc, locationView) = 
                LocationPicker.Client.page()

            panel "Weather" [ divAttr [ attr.style "text-align: center;" ] 
                                      [ Weather.Client.page locationView ]
                              div [ locationDoc ] ]