# CreateProjectsInMongoDB
Creates projects in my Mongo DB database using a ASP.NET Core Web API and a Windows Forms application. Also contains xUnit tests project.  This is a backend for my website portofile project  https://github.com/ByronCoder/ProjectPortfolio    You can see this site live at: https://byroncoder.github.io/ProjectPortfolio/

This project uses Auth0 for authentication.  https://auth0.com/

# What This Project Contains
1. ProjectsAPI  (ASP.NET Core Web API)
   An ASP.NET Core Web API for executing CRUD operations on the MongoDB database via a REST API. It uses Auth0 for authentication so that only an authorized user can create, update or delete projects.

2. API.Tests  (xUnit Test Project)
    This is an xUnit Test project for the ProjectsAPI that tests all of the operations that we want to execute with the API. It tests whether you can get the project data and if unauthorized operations return an HTTP Unauthorized status code.  It also tests if the create, update and delete operations work correctly if authenticated.

3. CreateProjectsInMongoDB  (Windows Forms Application)
Allows you to view the projects data in the MongoDB database by retreiving the data from the ProjectsAPI as well as save new projects to the database. Uses Auth0 to allow you to authenticate. 

# Application Settings Files
Note: The appsettings.json file settings for both the ASP.NET Core Web API and the xUnit Test project as well as the App.config settings for the Windows Forms Application contains senstive information which is not included in this repoistory.  You will need to create seperate settings files and add your configuration for both the connection strings for MongoDB an the Auth0 configuration.   See documention for both MongoDB and Auth0 for information on how to obtain these settings. 

You will need to create an appsettings.Development.json file in the root directory of both the ProjectAPI ASP.NET Core Web API and the xUnit Test project API.Tests that contains settings for MongoDB and Auth0.  You may also create files for different enviorments if you like such as Staging or Production. See Auth0 documentation for more information.

Example appsettings.Development.json file for the ProjectAPI: 

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "ProjectDb": "mongodb://username:password@hostname/database"
  },
  "Auth0": {
    "Authority": "https://some-site.auth0.com/",
    "Audience": "https://some-site.com/"
  }
}

```

Example appsettings.Development.json file for the API.Tests: 

```json
{
  "Logging": {
    "IncludeScopes": false,
    "Debug": {
      "LogLevel": {
        "Default": "Warning"
      }
    },
    "Console": {
      "LogLevel": {
        "Default": "Warning"
      }
    }
  },
  "ConnectionStrings": {
    "ProjectDb": "mongodb://username:password@hostname/database"
  },
  "Auth0": {
    "Authority": "https://some-site.auth0.com/",
    "Audience": "https://some-site.com/",
    "ClientId": "abcd123454643efg",
    "ClientSecret": "-abcd123454643efg"
  }
}

```

You will also need to create a file called Auth0.config in the root directory of the Windows Forms Project CreateProjectsInMongoDB


```xml
<appSettings>
    <add key="Auth0:Domain" value="some-site.auth0.com/"/>
    <add key="Auth0:ClientId" value="abcd123454643efg"/>
</appSettings>
```


