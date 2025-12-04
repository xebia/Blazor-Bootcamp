# Modernization Lab

This lab is intended to bring together the concepts learned in previous labs and apply them to modernize a legacy application. You will work through a series of steps to refactor, containerize, and deploy an application using contemporary tools and practices.

At a high level, you will start with a working WFP application and migrate (rewrite) it to a Blazor web application.

The existing WPF application uses a LocalDB database to store data, and the code uses ADO.NET for data access.

The WPF app doesn't use dependency injection or any modern design patterns. It is very similar to a lot of WPF apps written in the 2005-2010 timeframe.

## Lab Steps

### Step 1: Analyze the Existing Application

Notice the:

1. Use of SQL Server LocalDB for data storage.
2. Use of ADO.NET for data access. Consider the use of Entity Framework Core as a modern alternative.
3. Navigation pattern, and think about how you would implement this in a Blazor application (same? different?).

### Step 2: Set Up the Blazor Project

1. Create a new Blazor Web App project using Visual Studio. Call it `ClaimTrackerBlazor`.
  a. ![Create Blazor App](readme-images/new-project-info.png)
2. Copy the `App_Data` folder from the WPF project to the Blazor _server_ project to maintain the database.
3. Add the necessary NuGet packages for Entity Framework Core and SQL Server.
  a. `dotnet add package Microsoft.EntityFrameworkCore.SqlServer`
  b. `dotnet add package Microsoft.EntityFrameworkCore.Tools`

### Step 3: Migrate the Data Access Layer

1. Create a new folder called `Data` in the Blazor project.
2. Create a new class called `ClaimContext` that inherits from `DbContext`.

