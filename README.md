# Tedd.ShortUrl
Simple URL shortening in ASP.Net 6.0 using EF Core and MSSQL backend.

Sample page: [https://go.tedd.no](https://go.tedd.no) 

## Docker image

Docker image available at: [https://github.com/tedd/Tedd.ShortUrl/pkgs/container/tedd.shorturl](https://github.com/tedd/Tedd.ShortUrl/pkgs/container/tedd.shorturl)

`docker pull ghcr.io/tedd/tedd.shorturl:main`

Supports setting environment variables in docker that will override appsettings.json.

| Environment variable          | Value                                                        |
| ----------------------------- | ------------------------------------------------------------ |
| Url:OverrideUrl               | https://go.domain.com/                                       |
| Database:ConnectionString     | Server=tcp:\*\*\*,1433;Initial Catalog=\*\*\*;Persist Security Info=False;User ID=\*\*\*;Password=\*\*\*;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30; |
| Security:AuthenticationTokens | YourLongAndRandomSecurityToken                               |
| Google:AnalyticsId            |                                                              |
| Google:RecaptchaV3SiteKey     |                                                              |
| Google:RecaptchaV3SecretKey   |                                                              |

## Docker compose example

    version: "3.9"
    
    tedd-shorturl:
        image: ghcr.io/tedd/tedd.shorturl:main
        container_name: tedd-shorturl
        environment:
            - Url:OverrideUrl=https://go.domain.com/
            - "Database:ConnectionString=Server=tcp:***,1433;Initial Catalog=***;Persist Security Info=False;User ID=***;Password=***;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
            - Security:AuthenticationTokens=...
            - Google:AnalyticsId=...
            - Google:RecaptchaV3SiteKey=...
            - Google:RecaptchaV3SecretKey=...
        restart: always
        port: always
            - 80:80

## Web UI

A sample web UI for entering URL and returning shortened URL.

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

