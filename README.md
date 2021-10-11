# Tedd.ShortUrl
Simple URL shortening in ASP.Net 5.0 using EF Core and MSSQL backend.

## Web UI

A simple web ui for entering URL and returning shortened URL. It uses ASP.Net 5.0 MVC and POST back to controller.

## Web API

Consists of two methods: Create and Get.

When creating "Expires" is optional. "Expires" is only respected by ShortUrl forwarder. The "Get" API call will retrieve it even if it is expired.

AuthTokens can be added either in appsettings.json or in AuthTokens table in database.

### Create

HTTP POST to /api/Create

```
{
 "AuthToken": "A valid auth token",
 "Url": "https://www.google.com",
 "Expires": "2022-01-01T01:02:03",
 "Metadata": "Some extra metadata if you want"
}
```

Will return something like this

```
{
  "success": true,
  "errorMessage": null,
  "shortUrl": "https://your-domain.com/112c1",
  "expires": "2022-01-01T01:02:03",
  "key": "112c1"
}
```

### Get

HTTP POST to /api/Get

```
{
  "AuthToken": "A valid auth token",
  "Key": "dab22"
}
```

Will return something like this

```
{
  "success": true,
  "errorMessage": null,
  "shortUrl": "https://your-domain.com/112c1",
  "url": "https://www.google.com",
  "expires": "2022-01-01T01:02:03",
  "metadata": "Meta"
}
```

