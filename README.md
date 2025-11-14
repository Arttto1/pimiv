# Sistema Kanban# Sistema Kanban# 📋 Sistema Kanban# 📋 Sistema Kanban - Gerenciamento de Tarefas Multiplataforma# Kanban Board - Sistema Completo em C#

Sistema de gerenciamento de tarefas com quadro Kanban e sistema de tickets.Sistema de gerenciamento de tarefas com Kanban board e sistema de tickets.

## Como Executar## Como ExecutarSistema de gerenciamento de tarefas estilo Kanban com múltiplas interfaces (Web e Desktop) e sistema de tickets.

### 1. Configurar o Banco### 1. Configurar o Banco de Dados

`powershell`powershell## 🚀 Como Executar![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)Sistema Kanban completo com API REST e dois frontends (Desktop WPF e Web Blazor) integrado com **MS SQL Server**.

.\setup_localdb.ps1

````.\setup_localdb.ps1



Ou manualmente:```

```powershell

sqlcmd -S "(localdb)\MSSQLLocalDB" -i setup_database.sqlOu manualmente:### 1. Configurar o Banco de Dados![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp)

````

````````powershell

### 2. Rodar a API

sqlcmd -S "(localdb)\MSSQLLocalDB" -i setup_database.sql

```powershell

cd KanbanAPI```

dotnet run

```````powershell![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server)**📦 Compatível com .NET 9.0**



API: `http://localhost:5000`### 2. Rodar a API



### 3. Rodar o Cliente.\setup_localdb.ps1



**Desktop:**```powershell

```powershell

cd KanbanDesktopcd KanbanAPI```![WPF](https://img.shields.io/badge/WPF-512BD4?style=for-the-badge&logo=windows)**🗄️ Banco de Dados: Microsoft SQL Server**

dotnet run

```dotnet run



**Web:**````

```powershell

cd KanbanWebA API estará em `http://localhost:5000`Ou manualmente:![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=for-the-badge&logo=blazor)

dotnet run

```### 3. Rodar o Cliente```powershell



Web: `http://localhost:5001`**Desktop (WPF):**sqlcmd -S "(localdb)\MSSQLLocalDB" -i setup_database.sql## 📋 Estrutura do Projeto



## Credenciais```````powershell



**Admin:** `admin` / `senha123`  cd KanbanDesktop```

**Usuários:** `joao`, `maria`, `pedro` / `senha123`

dotnet run

## Tecnologias

```Sistema completo de gerenciamento de tarefas estilo Kanban com múltiplas interfaces (Web e Desktop), sistema de tickets e autenticação segura.

**API (Backend)**

- ASP.NET Core 9.0

- SQL Server LocalDB

- BCrypt**Web (Blazor):**### 2. Executar a API

- Swagger

```powershell

**Desktop (WPF)**

- Windows Presentation Foundationcd KanbanWeb```

- XAML

- HttpClientdotnet run



**Web (Blazor)**``````powershell

- Blazor Server

- Razor Components

- JavaScript Interop

O site estará em `http://localhost:5001`cd KanbanAPI## 🎯 Sobre o Projetopim/

---



Projeto acadêmico - UNIP 2025
````````
