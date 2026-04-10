# WaifuAIAssistant Backend

**WaifuAIAssistant** is a virtual AI assistant system developed with ASP.NET Core (.NET 10). It provides a backend API for managing AI characters, conversation interactions, user settings, and chat history. The system supports various AI integrations (such as OpenAI and Local Models), allowing highly personalized user experiences.

---

## 🚀 Technologies Used

- **.NET 10 / ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server** (Primary Database)
- **Redis** (Distributed Caching)
- **JWT Authentication**
- **Swagger / Swashbuckle** (API Documentation)
- **Mapster** (DTO Mapping)
- **Clean Architecture** (Domain-Driven Design - DDD)

---

## 🧠 Project Architecture

The project strictly follows the Clean Architecture pattern and is organized into 4 main layers:

- `WaifuAIAssistant.Domain`: Core entities, repository interfaces, and domain models.
- `WaifuAIAssistant.Application`: Application services, UseCases, DTOs, and interface definitions.
- `WaifuAIAssistant.Infrastructure`: External integrations, database contexts (EF Core), and third-party services.
- `WaifuAIAssistant.API`: Controllers, Dependency Injection (DI) configurations, middlewares, rate-limiting, and API endpoints.

---

## 📦 Key Features

- 🔐 **User Authentication**: Secure Registration/Login using JWT Tokens and OTP Verification.
- 🤖 **AI Character Management**: Create and manage customizable AI personalities (ModelCharacter).
- 🗣️ **Conversation System**: Create, manage, and store chat sessions between users and AI companions.
- 🧠 **Rich AI Settings**: Customize emotions, storylines, and behaviors of the virtual assistants.
- 📊 **Chat History**: Persistent storage and retrieval of user chat histories.
- 🛡️ **Role & User Management**: Secure configuration and authorization.
- ⚡ **Performance Optimized**: Implements Redis caching and endpoint rate limiting.

---