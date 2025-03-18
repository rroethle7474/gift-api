# Christmas Gift API

A .NET 8 API for managing a Christmas gift exchange system, allowing users to create wish lists, submit them for approval, and recommend items to others.

The SQL db repo can be found here (https://github.com/rroethle7474/ProjectDb/tree/main/Tables). This uses a custom Users table for users. Please see this script to populate status tables: https://github.com/rroethle7474/ProjectDb/blob/main/Scripts/GiftDb-InitialMigration.sql

## Table of Contents

- [Features](#features)
- [API Endpoints](#api-endpoints)
  - [Users](#users)
  - [Wish List](#wish-list)
  - [Wish List Submissions](#wish-list-submissions)
  - [Hero Content](#hero-content)
  - [Recommend Wish List](#recommend-wish-list)
  - [Settings](#settings)
- [Setup](#setup)
  - [Prerequisites](#prerequisites)
  - [Configuration](#configuration)
  - [Running Locally](#running-locally)
- [Authentication](#authentication)

## Features

- User management with authentication
- Wish list creation and management
- Wish list submission and approval workflow
- Hero content management for the frontend
- Recommendation system for gift ideas
- Email notifications using SendGrid
- JWT authentication

## API Endpoints

### Users

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create a new user |
| PUT | `/api/users/{id}` | Update user details |
| DELETE | `/api/users/{id}` | Delete a user |
| POST | `/api/users/login` | Authenticate a user |
| POST | `/api/users/logout` | Log out a user (primarily for guest users) |
| POST | `/api/users/{userId}/change-password` | Change user password |
| GET | `/api/users/azure-b2c` | Get Azure B2C users |
| POST | `/api/users/azure-b2c/{objectId}/sync` | Sync Azure B2C user to database |

### Wish List

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/wishlist` | Get all wish list items |
| GET | `/api/wishlist/{id}` | Get wish list item by ID |
| GET | `/api/wishlist/user/{userId}` | Get all wish list items for a specific user |
| POST | `/api/wishlist` | Create a new wish list item |
| PUT | `/api/wishlist/{id}` | Update a wish list item |
| DELETE | `/api/wishlist/{id}` | Delete a wish list item |

### Wish List Submissions

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/wishlistsubmission` | Get all wish list submissions |
| GET | `/api/wishlistsubmission/{id}` | Get submission by ID |
| GET | `/api/wishlistsubmission/user/{userId}` | Get all submissions for a specific user |
| POST | `/api/wishlistsubmission` | Create a new submission |
| PUT | `/api/wishlistsubmission/{id}` | Update submission status |
| DELETE | `/api/wishlistsubmission/{id}` | Delete a submission |

### Hero Content

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/herocontent` | Get all hero content items |
| GET | `/api/herocontent/{id}` | Get hero content by ID |
| GET | `/api/herocontent/active` | Get currently active hero content |
| POST | `/api/herocontent` | Create new hero content |
| PUT | `/api/herocontent/{id}` | Update hero content |
| DELETE | `/api/herocontent/{id}` | Delete hero content |

### Recommend Wish List

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/recommendwishlist` | Get all recommended wish list items |
| GET | `/api/recommendwishlist/{id}` | Get recommended item by ID |
| GET | `/api/recommendwishlist/user/{userId}` | Get all recommendations for a specific user |
| POST | `/api/recommendwishlist` | Create a new recommendation |
| PUT | `/api/recommendwishlist/{id}` | Update a recommendation |
| DELETE | `/api/recommendwishlist/{id}` | Delete a recommendation |

### Settings

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/settings` | Get all application settings |
| GET | `/api/settings/{name}` | Get setting by name |

## Setup

### Prerequisites

- .NET 8 SDK
- SQL Server (local or remote)
- SendGrid account (for email notifications)

### Configuration

The application uses the following configuration in `appsettings.json`:

```json
{
  "NotificationSettings": {
    "SendGridApiKey": "your-sendgrid-api-key",
    "SendGridFromEmail": "notifications@example.com",
    "SendGridFromName": "Christmas Gift App",
    "BaseApprovalUrl": "https://yourapp.com/approve"
  },
  "Jwt": {
    "Key": "your-secret-key-with-at-least-16-characters",
    "Issuer": "GiftApi",
    "Audience": "GiftWebApp",
    "ExpirationHours": 24
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ChristmasGiftDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

For Azure B2C authentication (optional and currently not used.):

```json
{
  "AzureAdB2C": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  }
}
```

### Running Locally

1. Clone the repository
   ```
   git clone https://github.com/yourusername/christmas-gift-api.git
   cd christmas-gift-api
   ```

2. Update the connection string in `appsettings.json` to point to your database

3. Apply database migrations (if using Entity Framework Core)
   ```
   dotnet ef database update
   ```

4. Run the application
   ```
   dotnet run
   ```

5. Access the Swagger UI at `https://localhost:5001/swagger`

## Authentication

The API uses JWT bearer authentication. To access protected endpoints:

1. Obtain a token by sending a POST request to `/api/users/login` with valid credentials
2. Include the token in the Authorization header of subsequent requests:
   ```
   Authorization: Bearer your-token-here
   ```