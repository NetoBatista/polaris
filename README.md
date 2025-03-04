English | [Portuguese](README_ptbr.md)

# Polaris
## What is it?
Polaris is a user and application management API designed to perform login through MFA, password, or Firebase.

## How does it work?
The API provides the following groups of routes:

- **/Application**
    - Allows managing applications.

- **/Authentication**
    - Allows performing authentication (MFA, Password, or Firebase).

- **/Members**
    - Allows linking the user to one or more applications.

- **/User**
    - Allows managing users in the database.

## Getting Started

The first step is to register the desired applications by providing the required fields.

After registering the applications, the second step is to register the user through the POST /User route.

The next step is to create a link between the user and their applications. Each user can be associated with one or more applications and may have different types of authentication.

The association can be made using the POST /Member route, providing the user ID and the application ID. You can specify the user’s password for that application, but it is still possible to perform login via MFA or Firebase.

Finally, you can generate a JWT token for integration with other systems through the POST /Authentication route.

## About the API
The API is developed in C# .NET 8, using the DDD (Domain-Driven Design) methodology. Some of the features included are:
- Entity Framework
- SQL Server
- Migrations
- ServiceBus

## Configuration and Running the API
All variables can be configured as environment variables or through the appSettings.json file.

### Quick Configuration
For a quick execution of the API, simply define an environment variable `ConnectionString` pointing to a SQL Server. On the first run, the migrations will be validated and executed, creating the necessary table structure.

Example of a ConnectionString:
```text
Server=localhost;Initial Catalog=polaris;Persist Security Info=False;User ID=sa;Password=Senha123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;
```

Modify the necessary fields in the example above, such as `Server`, `User ID`, and `Password`, and that’s it. Then, go to `http://localhost:5001/swagger` to view the Swagger UI. You can also edit the ConnectionString in the project’s `launchSettings.json` file.

### Other Parameters
There are other parameters in the application that can be modified, found in the `appsettings.json` file.

One of them is the `ConnectionString`, which is also present in this file. However, there is a hierarchy when running the project: if an environment variable `ConnectionString` is provided, it will be prioritized. If not, the project will read from the `appsettings.json` file.

Example of the ConnectionString configuration through appsettings.json:
```json
"Database": {
  "ConnectionString": "Server=localhost;Initial Catalog=polaris;Persist Security Info=False;User ID=sa;Password=Senha123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
}
```

There are also parameters for the JWT generated in the POST `/Authentication` route. You can specify the secret and the token expiration time (in minutes).

If other applications reuse Polaris' JWT, it's important to check if they have the `ValidateIssuerSigningKey` parameter enabled. If so, they need to have the same `Secret` specified in the `appsettings.json` to function properly.

Example of the JWT configuration through appsettings.json:
```json
"JwtToken": {
  "Secret": "d8d69f45-3d95-494f-a016-cba9143a503d",
  "Expire": "5"
}
```

The JWT Token can also be configured through environment variables.

```
JwtTokenSecret
```

```
JwtTokenExpire
```

## Events

We are adding callbacks through events, and there is currently a pre-configured setup to use Azure's ServiceBus resource.

You can enter the ConnectionString to connect to the resource, as well as the queue name through appSettings.json.

```
"ServiceBus": {
  "ConnectionString": "",
  "Queue": ""
}
```

Or through environment variables:

```
ServiceBusConnectionString
```

```
ServiceBusQueue
```

## Final Considerations
Feel free to fork and make necessary changes. Additionally, you can contribute through the Issues tab or by making Pull Requests (PR).