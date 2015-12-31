namespace Arche.Shell

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client

module Menu =
    module Static =
        open WebSharper.Sitelets
            
        let private nav (ctx: Context<string>) routes =
            
            let navBar left right = 
                let navHeader = 
                    divAttr [ attr.``class`` "navbar-header" ] 
                            [ buttonAttr [ attr.``class`` "navbar-toggle collapsed"
                                           Attr.Create "data-toggle" "collapse"
                                           Attr.Create "data-target" "#menu"
                                           Attr.Create "aria-expanded" "false" ] 
                                         [ spanAttr [ attr.``class`` "sr-only" ]  []
                                           spanAttr [ attr.``class`` "icon-bar" ] []
                                           spanAttr [ attr.``class`` "icon-bar" ] []
                                           spanAttr [ attr.``class`` "icon-bar" ] [] ]
                              text "Arche" ]

                let navMenu = 
                    divAttr [ attr.``class`` "collapse navbar-collapse"; attr.id "menu" ] [ left; right ]

                navAttr [ attr.``class`` "navbar navbar-default" ] [ divAttr [ attr.``class`` "container-fluid" ] [ navHeader; navMenu ] ]

            let navButtons = 
                let liList = 
                    routes |> List.map (fun (title, relativeRoute, route) -> liAttr [ attr.id relativeRoute; attr.style "display: none;" ] [ aAttr [ attr.href route ] [text title] ]) |> Seq.cast |> Seq.toList
                        
                ulAttr [attr.``class`` "navmenu nav navbar-nav"] liList

            navBar navButtons Doc.Empty

        let embed ctx routes doc =
            [ nav ctx routes :> Doc
              divAttr [ attr.``class`` "container-fluid" ] [ doc ] :> Doc
            ] |> Doc.Concat
            