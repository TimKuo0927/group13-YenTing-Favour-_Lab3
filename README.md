# Podcast Management Web App (Lab 3)

A small ASP.NET Core (MVC) podcast management web application with user authentication. It allows creators to publish podcasts and episodes, and enables users to subscribe and play episodes. 

This repository serves as a lab project for **Group 13 (YenTing & Favour)**. It is built for developers to run locally against SQL Server, with optional AWS SDK configurations.

## Tech Stack

* **Language:** C# (Backend), HTML/CSS/JS (Frontend views & static assets)
* **Framework & Runtime:** .NET 8.0 / ASP.NET Core MVC
* **Key Libraries:**
  * `Microsoft.EntityFrameworkCore` (+ `SqlServer` + `Tools`) — ORM for data access and database migrations.
  * `Microsoft.AspNetCore.Identity.EntityFrameworkCore` — Identity framework for user authentication.
  * `AWSSDK.S3`, `AWSSDK.DynamoDBv2`, `AWSSDK.Core` — Optional AWS integrations for cloud storage and database features.

## Project Structure

Below is the layout of the primary project folders and key files:

```text
group#13(YenTing&Favour)_Lab#3/      # Primary project folder (contains .csproj)
  ├── Controllers/                  # MVC controllers handling HTTP endpoints
  ├── Models/                       # EF Core models & DbContext definitions
  │     ├── Lab3Context.cs
  │     ├── Podcast.cs
  │     ├── Episode.cs
  │     ├── Subscription.cs
  │     └── User.cs                 (Scaffolded/commented out)
  ├── Migrations/                   # EF Core database migrations
  ├── Views/                        # Razor views for UI rendering
  ├── ViewModel/                    # View model classes (UI DTOs)
  ├── Services/                     # Business logic helpers (audio upload, subscriptions)
  ├── wwwroot/                      # Static assets (CSS, JS, images)
  ├── Program.cs                    # Application startup, DI configuration, and middleware pipeline
  └── group#13(YenTing&Favour)_Lab#3.csproj
├── .gitignore
└── .gitattributes
```

##  Architecture & Workflow

* **Initialization (`Program.cs`):** Wires up system dependencies, including `AddControllersWithViews()`, connection setup for `Lab3Context` (EF Core + SQL Server), and ASP.NET Identity routing.
* **Data Layer (`Models/`):** `Lab3Context` orchestrates the core data structures, configuring entity relationships for `Podcast`, `Episode`, and `Subscription` models.
* **Authentication:** System user management extends from `IdentityUser`, storing identity credentials via EF Core within the primary database context.
* **Request Pipeline:** Controllers map incoming HTTP actions, execute core business logic housed within `Services/` (e.g., audio file processing), and populate data structures into `ViewModel/` formats to display interactive components within Razor `Views/`.

##  Getting Started

1. **Prerequisites:** Ensure you have the **.NET 8.0 SDK** and **SQL Server** installed locally.
2. **Database Migration:** Run `Update-Database` via the Package Manager Console (or `dotnet ef database update` in your CLI) to generate your local database schema.
3. **Run App:** Launch the project locally using your IDE or by running `dotnet run` inside the main project directory.
