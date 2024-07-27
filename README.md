# Polaris
## What is it?
Polaris is a user management API that allows for the registration and management of users across different applications.

## How does it work?
The API provides the following groups of routes:

- **/Application**
    - Allows managing applications (Create, Retrieve, Update, and Delete).

- **/Authentication**
    - Allows performing authentication (MFA or Password), updating the token, updating the password, or changing a member's authentication type.

- **/Members**
    - Allows linking the user to an application.

- **/User**
    - Allows creating users in the database.

The idea is to register multiple applications with various users, each being associated with one or more applications.

The first step is to register one or more desired applications by providing the required fields.

After registering the applications, the second step is to register the user through the POST /User route by providing the necessary fields.

The next step is to create a link between the user and their applications. Each user can be associated with one or more applications and can have different types of authentication. For example, in one application, they can use MFA, and in another, they can use a password. The link can be created using the POST /Member route, providing the user ID, application ID, and the type of authentication for that user.

To finalize, you can generate a JWT token for integration with other systems through the POST /Authentication route.

## About the API
The API is developed in C# .NET 8, using the DDD (Domain-Driven Design) methodology. Some of the features included are:
- Entity Framework
- SQL Server
- Migrations

## Configuration and running the API
### Quick configuration
For a quick execution of the API, simply define an environment variable `ConnectionString` pointing to a SQL Server. On the first run, the migrations will be validated and executed, creating the necessary table structure.

Example of a ConnectionString: 
```text
Server=localhost;Initial Catalog=polaris;Persist Security Info=False;User ID=sa;Password=Senha123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;
```

Modify the necessary fields in the example above, such as `Server`, `User ID`, and `Password`, and that's it. Then, go to `http://localhost:5001/swagger` to view the Swagger UI. You can also edit the ConnectionString in the project's `launchSettings.json`.

### Other parameters
There are other parameters in the application that can be modified, found in the `appsettings.json` file.

One of them is the `ConnectionString`, which is also present in this file. However, there is a hierarchy when running the project: if an environment variable `ConnectionString` is provided, it will be prioritized. If not, the project will read from the `appsettings.json` file.

Example of the ConnectionString configuration through appsettings.json:
```json
"Database": {
  "ConnectionString": "Server=localhost;Initial Catalog=polaris;Persist Security Info=False;User ID=sa;Password=Senha123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
}
```

There are also parameters for the JWT generated in the POST `/Authentication` route. You can specify the secret and the token expiration time (in minutes).

If other applications reuse the Polaris JWT, it is important to check if they have the `ValidateIssuerSigningKey` parameter enabled. If so, they need to have the same `Secret` specified in the `appsettings.json` to work properly.

Example of the JWT configuration through appsettings.json:
```json
"JwtToken": {
  "Secret": "d8d69f45-3d95-494f-a016-cba9143a503d",
  "Expire": "5"
}
```

## Final considerations
Feel free to fork and make your changes if necessary. Additionally, you can contribute through the Issues tab or by making Pull Requests (PR).