namespace Arche.Shell

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client

module Menu =

    [<JavaScript>]
    module private Client =
        open WebSharper.JavaScript

        let brand homeLink =
            divAttr [ attr.``class`` "navbar-brand"
                      attr.style "cursor: pointer;"
                      on.click (fun _ _ -> JS.Window.Location.Replace(homeLink)) ] [ text "Arche" ]
            

    module Static =
        open WebSharper.Sitelets
        
        let private nav homeLink curr routes =
            
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
                              client <@ Client.brand homeLink @>]

                let navMenu = 
                    divAttr [ attr.``class`` "collapse navbar-collapse"; attr.id "menu" ] [ left; right ]

                navAttr [ attr.``class`` "navbar navbar-default" ] [ divAttr [ attr.``class`` "container-fluid" ] [ navHeader; navMenu ] ]

            let navButtons = 
                let liList = 
                    routes 
                    |> List.map (fun (title, route) -> 
                        liAttr [ if curr = title then yield attr.``class`` "active" ] 
                               [ aAttr [ attr.href route ] [text title] ]) 
                    |> Seq.cast 
                    |> Seq.toList
                        
                ulAttr [attr.``class`` "nav navbar-nav"] liList

            navBar navButtons Doc.Empty

        let embed homeLink currRoute routes doc =
            [ nav homeLink currRoute routes :> Doc
              divAttr [ attr.``class`` "container-fluid" ] [ doc ] :> Doc
            ] |> Doc.Concat
            