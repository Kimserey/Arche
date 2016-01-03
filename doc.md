

# Architecture for web app built in F# and WebSharper

__F# + WebSharper is an awesome combination to build web application__. The only problem is that if you want to build a webapp larger than a todo list, it's hard to find examples to use as reference. It is even harder to find tutorials which touch on overall design and answer questions like: 
- _How should I start?_
- _Where should I put the code?_
- _How should I separate the code, put it in different files, with different namespaces or modules?_
- _How many layers?_ 

Asking these questions at the beginning of the development is important. All these questions if answered correctly will lead to a good structure for the application. Answered incorrectly, or not even asked, will most likely lead to unmaintainable/fragile/rigid (or whatever bad adjectives for coding) code.

Over the last few months I have been working on a web app built in F# with WebSharper and came out with an architecture which cares for extensions and support for multiple customers. Today I decided to share this structure and hope that it will give you ideas and help you in your development. 

So let's get started!

## Overall architecture overview

The ideas behind this architecture come from two powerful concepts:
- Modular architecture - inspired by [Addy Osmani blog post](https://addyosmani.com/largescalejavascript/)
- Encapsulation through .FSX files - inspired by my colleague/buddy [Nathan](https://twitter.com/nbevans)

The idea of a modular achitecture is that the application is composed by small pieces (modules) which are completely independent of each others. One lives without knowing the others and none of the modules have dependencies on other modules. The patterns explained in the blog post of Addy Osmani goes much deeper and defines many other patterns but to me the most crutial understanding is that we should strive to manage dependencies. Coupling is the worse enemy of large application. It stops us from changing or removing pieces of the application and brings [FUD](https://en.wikipedia.org/wiki/Fear,_uncertainty_and_doubt) in our daily development. I've been there.. and it's not fun.

The second idea of encapsulation through .FSX files allows the app to be extended with new content contained in .FSX files. It is very useful as it provides a sandbox for other developers to work on features of the web app without the need of undestanding the core nor the need of modifying it. It also gives maximum flexibility as each .FSX files is totally self contained. They can be added or removed without breaking the system.  


Here's how the architecture looks like:

![architecture](http://4.bp.blogspot.com/-hxGVE2ZLgLk/VoipqDWakyI/AAAAAAAAADI/F_5eJWjPR0o/s1600/architecture_2.png)

Two important points to notice from the diagram:
- We can see clear boudaries represented by the colours
- The dependencies only flow in one direction, top to bottom

### Clear boundaries

From the diagram we can see five clear boundaries where we can place our codes:
- __Core lib / common__ - Contains common types and services like auth.
- __Shell__ - Combines pages, provides nav menu to the pages. The shell will do the necessary to gather all the pages, construct a menu based on the pages options and link the menu buttons to the correct pages.
- __Page__ - Combines webparts to build a page content. Pages specifie how they should be displayed (fullpage or with nav) and from where they can be accessed (from nav or just via direct url). A page can reference many webparts.
- __Webpart__ - Combines modules in a reusable piece of the web application. Webparts can be considered as an assemble of modules which serve a common purpose. Therefore webparts can reference multiple modules.
- __Module__ - Smallest piece of the web application.

### One way flow of dependencies

Dependencies are flowing downward only. Elements don't reference other elements from the same level.
- Pages do not need other pages
- Webparts do not need other webparts
- Module do not need other modules

Following this rule will allow us to be very flexible. We will be able to easily remove or add pages. We can also substitute a module for another in a webpart or substitute a webpart for another in a page without issue as they are independent of each other.

Let's see how we can apply this architecture in F# with WebSharper.

F# with WebSharper

To see how we can apply this architecture, we will build a sample app which cobtains a home page and page dedicated to showing the weather.

We start first by creating empty containers for our future code:

[Image]

We start first by defining the Page type which contains all the necessary information to diaplay a page.

[code]

We then write the code to compose the shell and the navbar.
The links in the navbar will be constructed based on what is defined in the pages. If the page is accessible through nav, it will create a button link in the nav. Then the display option is used to define whether the page will be full screen or embeded with nav at the top.

[code]

Each page is defined as a sitelet using the route and title and then sum together to form the main sitelet. The sitelet is then loaded on a owin selft host.

Now the shell is ready to load all the pages and won't need to be touched anymore.
