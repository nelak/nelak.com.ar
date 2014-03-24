(*@
    Layout = "post";
    Title = "How to deal with implicit casting in F#";
    Date = "2014-03-24T06:57:44";
    Tags = "F#, Computation, Expressions, Builders";
    Description = "How to deal with implicit casting in F#";
*)

(**
How we deal with implicit casting today
---------------------------------------

So far all the solutions I've seen that try to deal with implicit casting `static op_Implicit`
propose solving it in the following way:

*)

let inline (!>) (x:^a) : ^b = ((^a or ^b) : (static member op_Implicit : ^a -> ^b) x) 

(**


##### Sources:
###### [Is there an equivalent to creating a C# implicit operator in F#?][so1]
###### [Is there anyway to use C# implicit operators from F#?][so2]

[so1]: http://stackoverflow.com/questions/1686895/is-there-an-equivalent-to-creating-a-c-sharp-implicit-operator-in-f

[so2]: http://stackoverflow.com/questions/10719770/is-there-anyway-to-use-c-sharp-implicit-operators-from-f

<!--more-->

Implicit casting using inline operator
--------------------------------------

Although this this solutions works it still implies that some kind of explicit casting take place in the form of:

    let casted = |> castMe

I was surprised not being able to find any other kind of solution like:


*)

[<AllowImplicit>]
let casted = castMe

(**

or something more idiomatic to F# like this:

*)

implicit 
{
	let! implicitConversion = Point(1,1)
        return implicitConversion
}

(**

So I decided to try and a write a computation expression builder, and this is what I came up with:

*)

namespace System

[<Sealed>]
type ImplicitBuilder() =

    member inline this.Bind((x:'T), rest: 'U -> 'V) : 'V =
        let inline (!>) (x:^a) : ^b = ((^a or ^b) : (static member op_Implicit : ^a -> ^b) x)
        !> x |> rest

    member this.Return(x:'T) = x

[<AutoOpen>]
module ImplicitBuilderImpl =
    let implicit = ImplicitBuilder()

(**
Using the implicit builder
--------------------------

This implemenation now allows us to opt-in for implicit casting through the `implicit` keyword
without sacrifing type safety, type inference or having to deal with the explicit casts anymore

*)

open System
open System.Drawing

let pointF =
        implicit {
            let! implicitConversion = Point(1,1)
            return implicitConversion
        }

(**
Code and nuget
--------------
[https://github.com/nelak/FSharp.Implicit](https://github.com/nelak/FSharp.Implicit)

[https://www.nuget.org/packages/FSharp.Implicit/](https://www.nuget.org/packages/FSharp.Implicit/)
*)
