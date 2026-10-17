# API Aggregation Service

## Overview

This project is a .NET 10 API Aggregation Service built with ASP.NET Core. It combines data from multiple external APIs into a single endpoint, allowing clients to retrieve, filter, and sort data from different sources.

The service also includes parallel API requests, caching, request statistics, error handling, retry logic, fallback data, JWT authentication, and background performance monitoring.

## External APIs

The service integrates with the following APIs:

- **DummyJSON:** https://dummyjson.com/products
- **PokéAPI:** https://pokeapi.co/api/v2/pokemon?limit=20
- **JSONPlaceholder:** https://jsonplaceholder.typicode.com/posts

## Features

### API Aggregation

The service retrieves data from all three external APIs through a single endpoint. Requests are executed in parallel to reduce the total response time.

### Filtering and Sorting

Aggregated results can be filtered by API name or search term and sorted by API name or data content.

Supported query parameters:

| Parameter | Description |
|---|---|
| `ApiName` | Filters results by API name. |
| `SearchTerm` | Searches within the JSON response data. |
| `SortBy` | Selects the sorting field, such as `ApiName`. |
| `SortDescending` | Enables descending sorting when set to `true`. |

### Error Handling, Retries, and Fallback

The service handles external API failures so that one unavailable provider does not prevent the other providers from returning their results.

- Each provider request is attempted up to three times.
- Retries are performed for `HttpRequestException` and `TimeoutException`, which may indicate temporary network or timeout problems.
- The service waits 500 milliseconds between retry attempts.
- If all attempts fail, or an exception is not eligible for retry, the provider returns an empty result list.
- The aggregation endpoint can still return successful results from the other providers.
- The service records the elapsed time for each provider operation in its request statistics.

### Caching

The service caches the complete aggregated response to reduce repeated external API calls. The default cache duration is 5 minutes.

The cache key and duration can be configured in `appsettings.json`.

The service also maintains a separate fallback cache for each provider, allowing previously retrieved data to be used when an external API becomes unavailable.

Both caches are in-memory and are lost when the application restarts.

### Request Statistics

The statistics endpoint provides information about requests made to each external API, including:

- Total recorded requests.
- Average response time in milliseconds.
- Number of fast requests: less than 100 ms.
- Number of average requests: 100–200 ms, inclusive.
- Number of slow requests: greater than 200 ms.

Statistics are maintained in memory and grouped by API name.

### JWT Authentication

The service includes JWT bearer authentication to protect secured endpoints. A token can be obtained through the login endpoint using the configured test credentials.

### Background Performance Monitoring

A background service periodically checks API request statistics and logs warnings when at least half of an API's recorded requests are classified as slow.

## Technologies

- C# and .NET 10
- ASP.NET Core Web API
- HttpClient
- Asynchronous programming and parallel execution
- IMemoryCache
- JWT bearer authentication
- Dependency injection
- xUnit unit testing

## Setup and Configuration

### Prerequisites

- .NET 10 SDK
- Visual Studio 2026 or another compatible .NET development environment

### Run the Application

1. Clone or download the repository.
2. Open the solution in Visual Studio.
3. Restore NuGet packages.
4. Configure the required application settings.
5. Run the `API_Aggregation` project.
6. Open the Swagger UI URL configured by the application's launch settings.

### Configuration

Configure the cache settings and JWT settings in `appsettings.json` or through an appropriate environment-specific configuration source.

Example cache configuration:

```json
{
  "CacheSettings": {
    "AggregationCacheKey": "aggregated-api-data",
    "DurationInMinutes": 5
  }
}
```

Configure JWT issuer, audience, and secret key according to the application's authentication setup. Configure the test username and password using the corresponding `TestUser` settings.

Do not commit real passwords or JWT secret keys to source control.

## API Endpoints

### 1. Retrieve Aggregated Data

**GET** `/api/Aggregation`

Returns the combined results from the configured external APIs.

Example:

```http
GET /api/Aggregation
```

Filtering and sorting example:

```http
GET /api/Aggregation?ApiName=DummyJSON&SortBy=ApiName&SortDescending=true
```

Example response structure:

```json
{
  "results": [
    {
      "apiName": "DummyJSON",
      "data": {
        "title": "Example product"
      }
    }
  ],
  "successfulProviders": 2,
  "failedProviders": 1
}
```

The actual results depend on the responses returned by the external APIs.

### 2. Retrieve Request Statistics

**GET** `/api/Statistics`

Returns request counts, average response times, and performance bucket counts for each API.

### 3. Obtain an Authentication Token

**POST** `/api/Auth/login`

Accepts the configured test username and password and returns a JWT token when the credentials are valid.

Example request body:

```json
{
  "username": "your-configured-username",
  "password": "your-configured-password"
}
```

Use the returned token as a bearer token when accessing protected endpoints.

## Unit Tests

The solution includes a separate `API_Aggregation.Tests` project using xUnit.

To run the tests from the solution directory:

```bash
dotnet test
```
