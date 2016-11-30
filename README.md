# NYLT Rest Controller
A basic .NET CRUD controller for interacting with a RESTful service. I developed it for NYLT CampHub, but it's useful for anything. Designed to be compatible with the UWP API version 14393.
## Setup
You'll need Visual Studio 2015 or later. Clone the repo and open the `*.sln` file. This opens the .NET solution that contains the NYLTRestController project. It targets UWP version 14393.

You can compile the class into a `*.dll` for use with your project or simply [grab it from NuGet](https://www.nuget.org/packages/NYLT.RestController/) instead.
## Usage
Instantiate a `RestController` class with the URL of your API endpoint.

`var restController = new RestController('http://example.com/api/collection');`

From this object, you can issue `GET`, `POST`, `PUT`, and `DELETE` commands against that API endpoint. In addition, NYLT Rest Controller features a special `POST` method that allows you to upload an image to a server as part of a `form-multipart` `POST` request. Note that all methods are async.
