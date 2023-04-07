# Timestamp Microservice

This is an ASP.NET Web API that returns timestamps for utc and unix, 
based on the specifications for [this freeCodeCamp challenge](https://www.freecodecamp.org/learn/back-end-development-and-apis/back-end-development-and-apis-projects/timestamp-microservice).

# Startup 

To start the web api, run this command in the root folder:

```
dotnet run --project ./TimestampMicroservice/TimestampMicroservice.csproj
```

The console should show the URL that the api is listening on.

# Usage

To get the current timestamp:

```
GET /api/
```

To get a timestamp of a specific unix timestamp:

```
GET /api/{unixTimestamp}
```

To get a timestamp of a specific date string:

```
GET /api/{dateString}
```

# Testing

To run the tests for the web api, run this command in the root folder:

```
dotnet test
```
