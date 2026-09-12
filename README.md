# LiquidLabsProject

LiquidLabsProject is a simple user API built using **ASP.NET Core Web API, Microsoft SQL Server and ADO.NET**.

The application gets user data from a public API and saves it in SQL Server. When a user is requested, it first checks the database. If the user is not there, it gets the user from the API and saves it to the database.

## Tech Stack

- **Backend:** ASP.NET Core Web API (.Net 10)
- **Language:** C#
- **Database:** Microsoft SQL Server
- **Database Access:** ADO.NET
---

## Project Overview

- Check the database for existing users.
- If the requested user exists, return it from the database.
- If the user is not available, retrieve users from the public API.
- Store the retrieved users in SQL Server.
- Return the requested user.
- Used ADO.NET with parameterized SQL queries.
- No ORM is used.

Public API:

```text
https://gorest.in/public/v2/users
````

---

# How to Run:

## 1. Requirements

Make sure to have:

* .NET SDK
* Microsoft SQL Server / SQL Server LocalDB
* Visual Studio

## 2. Clone the Repository

```bash
git clone https://github.com/aashifameerwork/LiquidLabsProject.git
```

Open the solution in Visual Studio.

## 3. Configure the Database

Create the database and table using the following SQL script:

```sql
CREATE DATABASE LiquidLabsUserDb;
GO

USE LiquidLabsUserDb;

CREATE TABLE Users (
	Id BIGINT PRIMARY KEY,
	Name NVARCHAR(200),
	Email NVARCHAR(200),
	Gender NVARCHAR(10),
	Status NVARCHAR(20)
);
GO
```
