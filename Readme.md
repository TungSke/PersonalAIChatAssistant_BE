# 🤖 Personal AI Assistant Backend

![Build](https://github.com/TungSke/PersonalAIChatAssistant_BE/actions/workflows/test.yaml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-10-blueviolet)
![License](https://img.shields.io/github/license/TungSke/PersonalAIChatAssistant_BE)
![Last Commit](https://img.shields.io/github/last-commit/TungSke/PersonalAIChatAssistant_BE)

A scalable backend for a personal AI chat assistant, built with **ASP.NET Core** following **Clean Architecture**. Users can create and chat with customizable AI characters powered by **Google Gemini**, with secure authentication, OTP verification, and Redis-backed caching.

---

## 📖 Overview

Personal AI Assistant lets users create custom AI "characters" (personality, tone, storyline) and hold persistent conversations with them. The backend handles auth, character/conversation management, chat history, and integrates with Google's Generative AI API for responses.

---

## 📦 Key Features

- 🔐 **Authentication** — JWT-based auth with OTP email verification for registration/password reset
- 🤖 **AI Character Management** — create and configure custom AI personalities (`ModelCharacter`)
- 🗣️ **Conversation System** — create, manage, and persist chat sessions between users and AI characters
- 🧠 **Rich AI Settings** — customizable emotion, storyline, and behavior per character
- 📊 **Chat History** — persistent storage and retrieval of past conversations
- 🛡️ **Role & User Management** — role-based authorization
- ☁️ **Media Uploads** — image handling via Cloudinary
- ⚡ **Performance** — Redis caching and endpoint rate limiting

---

## 🧠 Architecture

Clean Architecture, 4 layers:

| Project | Responsibility |
|---|---|
| `PersonalAIAssistant.Domain` | Core entities, repository interfaces, domain models |
| `PersonalAIAssistant.Application` | Use cases, application services, DTOs, interfaces |
| `PersonalAIAssistant.Infrastructure` | EF Core DbContext, external service integrations (Redis, Cloudinary, Gemini, SMTP) |
| `PersonalAIAssistant.API` | Controllers, DI setup, middleware, rate limiting |
| `PersonalAIAssistant.Test` | Unit tests |

---

## 🚀 Tech Stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core + SQL Server
- Redis (distributed caching)
- JWT Authentication
- Mapster (DTO mapping)
- Swagger / Swashbuckle
- Cloudinary (media storage)
- Google Generative AI (Gemini)

---

## ⚙️ Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or remote instance)
- Redis (local instance or hosted)
- A Google Generative AI (Gemini) API key
- An SMTP account for sending OTP emails (e.g. Gmail App Password)
- (Optional) Cloudinary account for image uploads

---

## 🛠️ Getting Started

**1. Clone the repository**

```bash
git clone https://github.com/TungSke/PersonalAIChatAssistant_BE.git
cd PersonalAIChatAssistant_BE
```

**2. Configure `appsettings.json`**

Open `PersonalAIAssistant.API/appsettings.json` and fill in your own values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=PersonalAIAssistant;Uid=YOUR_UID;Pwd=YOUR_PASSWORD;TrustServerCertificate=True;",
    "RedisConnection": "localhost:6379"
  },
  "Jwt": {
    "Issuer": "https://localhost:7277",
    "Audience": "http://localhost:3000",
    "Key": "YOUR_JWT_SECRET_KEY"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "YOUR_APP_NAME",
    "Password": "YOUR_APP_PASSWORD"
  },
  "CloudinarySettings": {
    "CloudName": "YOUR_CLOUD_NAME",
    "ApiKey": "YOUR_API_KEY",
    "ApiSecret": "YOUR_API_SECRET"
  },
  "GenerativeAI": {
    "AIAPIKey": "YOUR_GEMINI_API_KEY",
    "AIAPIKey2": "YOUR_GEMINI_API_KEY_2",
    "PrimaryModel": "gemini-3.0-flash",
    "FallbackModel": "gemini-2.5-flash"
  }
}
```

> ⚠️ Don't commit real secrets. For production, use environment variables, User Secrets, or a secret manager instead of editing `appsettings.json` directly.

**3. Restore dependencies**

```bash
dotnet restore
```

**4. Apply database migrations**

```bash
cd PersonalAIAssistant.Infrastructure
dotnet ef database update --startup-project ../PersonalAIAssistant.API
```

**5. Run the API**

```bash
cd ../PersonalAIAssistant.API
dotnet run
```

The API will be available at `https://localhost:7277` (or the port shown in your console), with Swagger UI at `/swagger`.

---

## 🐳 Running with Docker

```bash
docker build -t personal-ai-assistant .
docker run -p 8080:8080 -p 8081:8081 personal-ai-assistant
```

> Note: the current `Dockerfile` base images are pinned to `.NET 8` while the project targets `.NET 10` — update the `mcr.microsoft.com/dotnet/aspnet` and `sdk` tags to match before deploying.

---

## 🧪 Testing

```bash
dotnet test
```

---

## 📚 API Documentation

Interactive API docs are available via Swagger once the app is running:

```
https://localhost:7277/swagger
```

---

## 📄 License

Distributed under the terms of the [LICENSE](./LICENSE) file in this repository.