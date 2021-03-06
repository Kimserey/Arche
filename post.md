

# Architecture for web app built in F# and WebSharper

__F# + WebSharper is an awesome combination to build web application__. The only problem is that if you want to build a web app larger than a to-do list, it's hard to find examples to use as references. It is even harder to find tutorials that touches on overall design and answer questions like: 
- _How should I start?_
- _Where should I put the code?_
- _How should I separate the code, put it in different files, with different namespaces or modules?_
- _How many layers?_ 

Asking these questions at the beginning of the development is important. All these questions if answered correctly will lead to a good structure for the application. Answered incorrectly, or not even asked, will most likely lead to unmaintainable/fragile/rigid (or whatever bad adjectives for coding) code.

Over the last few months, I have been working on a web app built in F# with WebSharper and came out with an architecture that caters to extensions and decoupling. Today, I will share this structure and hope that it will give you ideas and help you in your development. 

So let's get started!

## Overall architecture overview

The ideas behind this architecture was highly inspired by [Addy Osmani blog post](https://addyosmani.com/largescalejavascript/) on Patterns for large application and more precisely on modular architecture.

The idea of a modular achitecture is that the application is composed by small pieces (modules) which are completely independent from each other. One lives without knowing the others and none of the modules have dependencies on other modules. The patterns explained in the blog post of Addy Osmani goes much deeper and defines many other patterns but to me the most crucial understanding is that we should strive to manage dependencies. Coupling is the worst enemy of large applications. It stops us from changing or removing pieces of the application and brings [FUD](https://en.wikipedia.org/wiki/Fear,_uncertainty_and_doubt) in our daily development. I've been there.. and it's most certainly not fun.

Here's how the architecture looks like:

![architecture](http://4.bp.blogspot.com/-hxGVE2ZLgLk/VoipqDWakyI/AAAAAAAAADI/F_5eJWjPR0o/s1600/architecture_2.png)

Two important points to note from the diagram:
- We can see clear boundaries represented by the colours
- The dependencies only flow in one direction, top to bottom

### Clear boundaries

From the diagram we can see five clear boundaries where we can place our codes:
- __Core lib / common__ - Contains common types and services like auth.
- __Shell__ - Combines pages, provides nav menu to the pages. The shell will do the necessary to gather all the pages, construct a menu based on the pages options and link the menu buttons to the correct pages.
- __Page__ - Combines webparts to build a page content. Pages specify how they should be displayed (fullpage or with nav) and from where they can be accessed (from nav or just via direct url). A page can reference many webparts.
- __Webpart__ - Combines modules in a reusable piece of the web app. Webparts can be considered as an assemble of modules which serve a common purpose. Therefore webparts can reference multiple modules.
- __Module__ - Smallest piece of the web application.

### One way flow of dependencies

Dependencies flow downward only. Elements don't reference other elements from the same level.
- Pages do not need other pages
- Webparts do not need other webparts
- Modules do not need other modules

Following this rule makes the structure very flexible. We will be able to easily remove or add pages. We will also be able to substitute a module for another in a webpart or substitute a webpart for another in a page without issues as they are independent from each other.

Now that we understand the architecture, let's see how we can apply it in F# with WebSharper.

## A simple app with F# and WebSharper

To see how we can apply this architecture, we will build a sample app which contains 3 pages:
- a home page
- a page to show places on a map
- a page to show the weather

You can play with the app here [http://arche1.azurewebsites.net/](http://arche1.azurewebsites.net/).
![preview](http://3.bp.blogspot.com/-EZCKWQsSnJU/Voi6a1yEi-I/AAAAAAAAAD0/M9vaJuCT7z8/s640/arche.gif)

If you aren't familiar with WebSharper.UI.Next html notation, I wrote [a blog post where I gave some explanations about the UI.Next.Html notation and how to use the reactive model Var/View of UI.Next](http://kimsereyblog.blogspot.sg/2015/08/single-page-app-with-websharper-uinext.html). 

### Building blocks

First, we start by creating empty containers for our future code:

![files](http://4.bp.blogspot.com/-d4Ip0tx-WAE/Voi0h7LGkuI/AAAAAAAAADg/MWH1EJJ0tFw/s1600/Screen%2BShot%2B2016-01-03%2Bat%2B13.32.30.png)

Following the architecture diagram, we place the __common code__ in its own library. The Site project contains the __Shell / Page / Webpart / Module__ categories. 

F# enforces the references direction. Only bottom files can reference top files, your functions must be defined first before you can use it. Therefore, if we keep the modules at the top level, it will indirectly make the module's code have the least dependencies in the project.

### Common - Domain

The domain contains all the domain types. One of this type is the `Page` record type which contains the information about how a page should be displayed and from where it can be accessed.

```
type Page = {
    Title: string
    Content: Doc
    // defines how the page should be displayed
    DisplayOption: DisplayOption
    // defines how the page should be accessed
    AccessOption: AccessOption
    // defines the route url
    Route: Route
} 
and DisplayOption = PageWithMenu | FullPage
and AccessOption = Menu  of string | Other
and Route = Route of string list
```

### Shell

Having defined the page earlier, we can write the code to compose the shell and the navbar.
The links in the navbar are constructed based on the `AccessOption` and the `DisplayOption` is used to define whether the page will be displayed in full screen or embedded with a nav at the top.

```
module Main =
    /// Embeds the content in a page with nav if needed
    let mkContent ctx curr =
        match curr.DisplayOption with
        | DisplayOption.FullPage ->
            curr.Content
            
        | DisplayOption.PageWithMenu ->
            let routes = 
                All.pages |> List.choose (fun p -> 
                    match p.AccessOption with
                    | AccessOption.Menu title -> Some (title, ctx.Link p.Title)
                    | AccessOption.Other      -> None)

            curr.Content 
            |> Menu.Static.embed (ctx.Link "Home") curr.Title routes
    
    /// Builds a sitelet given a page
    let mkPage page =
        Sitelet.Content
        <| page.Route.Value 
        <| page.Title 
        <| fun ctx -> Content.Page (Title = page.Title, Body = [ mkContent ctx page ])
    
    /// Builds the site by summing all sitelets
    let main() =
        All.pages |> List.map mkPage |> Sitelet.Sum
```

Each page is defined as a sitelet using the route and title and then all pages are sum together to form the main sitelet.

The menu is defined as followed:

```
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
```

There are two modules within the Menu module.
- Client
- Static

`Client` contains the code which will be converted to JS. `Static` contains the code that is used by the Sitelet to compose the page. In other modules, there might be one more module called `Server` which will contain the WebSharper `RPC` calls.

Now the shell is ready to welcome all the pages that we define and we won't need to touch it anymore (sounds like the open close... you know, open for extension close to modification).

### Pages

Pages are pretty straightforward:

```
let pages = [
    { Title   = "Home"
      Route   = Route.Create [ "" ]
      Content = client <@ Home.Client.page() @>
      DisplayOption = DisplayOption.PageWithMenu
      AccessOption  = AccessOption.Other }

    { Title   = "Map"
      Route   = Route.Create [ "map" ]
      Content = client <@ Map.Client.webpart() @>
      DisplayOption = DisplayOption.PageWithMenu
      AccessOption  = AccessOption.Menu "Map" }
    
    { Title   = "Weather"
      Route   = Route.Create [ "weather" ]
      Content = 
        divAttr [ attr.style "max-width: 600px; margin: auto;" ] [ client <@ Weather.Client.webpart() @> ]
      DisplayOption = DisplayOption.PageWithMenu
      AccessOption  = AccessOption.Menu "Weather" }
]
```

As expected, they define the title, route, content of the page, how it should be displayed and accessed.

### Webparts

In our sample, the webparts are straightforward. But in other apps those might be more complex. The role of the webpart is to combine the modules together to form a part of functionality that is useful to the user.
Here we just need to combine the `Map module` with the `LocationPicker module` for the `Map webpart` and same for the `Weather webpart`.

Let's see the `Weather webpart`:

```
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
```

`panel` is a helper defined in the `Bootstrap module` in `Common`. You can find [the full code here](https://github.com/Kimserey/Arche/blob/master/Arche.Common/Bootstrap.fs).

The `Weather webpart` uses the `LocationPicker module` which returns its content plus a view on a location variable. The `Weather module` takes a location view and uses it to display the weather for this particular location. 

It is important to note that the `LocationPicker module` is not directly used in the `Weather module`. `Weather module` accepts a view as an argument, it doesn't matter where the view comes from. It is the role of the webpart to bind the `LocationPicker module` location view result to the `Weather module` and to ensure that `LocationPicker` is not dependent on `Weather` and vice versa. Modules should not reference each other.

### Modules

The last part of our architecture is the modules. We will look at `Weather module`, you can have a look at [the full code here](https://github.com/Kimserey/Arche/blob/master/Arche.Site/Modules.Weather.fs). For that we need to define a `Forecast` record type:

```
type private Forecast = {
    Title: string
    Description: string
    ImageUrl: string
    Temperature: decimal
    TemparatureMinMax: decimal * decimal
}
```

To get the weather, I used [http://openweathermap.org/api](http://openweathermap.org/api). Using the `JsonProvider` from `FSharp.Data` simplifies the interactions with the open weather api. We provide a `RPC` call which returns the `Forecast` depending on the city given. `RPC` allows `Server` calls to be called from `Client` code. Serialization is handled automatically by WebSharper and we can use the same types returned from the `Server` in the `Client`.

```
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
```

For the `Client`, we construct a reactive doc based on the city given. Everytime the city changes, the `RPC` is called and the doc is updated. 

`View.MapAsync: ('A -> Async<'B>) -> View<'A> -> View<'B>` takes as first argument a `async` function which is ran every time the view is updated and returns a view of the result of that `async` function. It makes it easy to combine view operations since we don't need to bother about their `async` nature.

```
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
```

Modules are the last element of the architecture. They must be completely independent and can be added or removed with ease from webparts.

## Conclusion

Today, we have seen one way of structuring a web app which reduces coupling between elements and allows rapid changes and adding new features easily. We have built a shell which doesn't need to be touched anymore and automatically add links to its menu based on the pages that are registered. Finally this structure also remove the confusion of where to place code and defined a clear way for components to interact with each other. I hope that this post will help you in your developement and I hope you enjoyed reading this post as much I enjoyed writing it. As usual, if you have any questions, you can hit me on twitter [@Kimserey_Lam](https://twitter.com/Kimserey_Lam). Thanks for reading!

- [You can play with the web app here.](http://arche1.azurewebsites.net/)
- [The source code is available on github.](https://github.com/Kimserey/Arche)
