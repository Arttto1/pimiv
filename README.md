# Sistema Kanban# 📋 Sistema Kanban# 📋 Sistema Kanban - Gerenciamento de Tarefas Multiplataforma# Kanban Board - Sistema Completo em C#

Sistema de gerenciamento de tarefas com Kanban board e sistema de tickets.

## Como ExecutarSistema de gerenciamento de tarefas estilo Kanban com múltiplas interfaces (Web e Desktop) e sistema de tickets.

### 1. Configurar o Banco de Dados

```powershell## 🚀 Como Executar![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)Sistema Kanban completo com API REST e dois frontends (Desktop WPF e Web Blazor) integrado com **MS SQL Server**.

.\setup_localdb.ps1

```

Ou manualmente:### 1. Configurar o Banco de Dados![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp)

```powershell

sqlcmd -S "(localdb)\MSSQLLocalDB" -i setup_database.sql

```

````powershell![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server)**📦 Compatível com .NET 9.0**

### 2. Rodar a API

.\setup_localdb.ps1

```powershell

cd KanbanAPI```![WPF](https://img.shields.io/badge/WPF-512BD4?style=for-the-badge&logo=windows)**🗄️ Banco de Dados: Microsoft SQL Server**

dotnet run

````

A API estará em `http://localhost:5000`Ou manualmente:![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=for-the-badge&logo=blazor)

### 3. Rodar o Cliente```powershell

**Desktop (WPF):**sqlcmd -S "(localdb)\MSSQLLocalDB" -i setup_database.sql## 📋 Estrutura do Projeto

```````powershell

cd KanbanDesktop```

dotnet run

```Sistema completo de gerenciamento de tarefas estilo Kanban com múltiplas interfaces (Web e Desktop), sistema de tickets e autenticação segura.



**Web (Blazor):**### 2. Executar a API

```powershell

cd KanbanWeb```

dotnet run

``````powershell



O site estará em `http://localhost:5001`cd KanbanAPI## 🎯 Sobre o Projetopim/



## Credenciais de Testedotnet run



**Admin:** `admin` / `senha123`  ```├── KanbanAPI/          # API REST (ASP.NET Core)

**Usuários:** `joao`, `maria`, `pedro` / `senha123`

API disponível em: `http://localhost:5000`

## Tecnologias

O **Sistema Kanban** é uma aplicação multiplataforma desenvolvida como projeto acadêmico da UNIP que implementa um quadro Kanban completo com recursos avançados de gerenciamento de tarefas e sistema de suporte via tickets.├── KanbanDesktop/      # Frontend Desktop (WPF)

**KanbanAPI (Backend)**

- ASP.NET Core 9.0### 3. Executar o Cliente

- SQL Server LocalDB

- BCrypt├── KanbanWeb/          # Frontend Web (Blazor Server)

- Swagger

**Desktop (WPF):**

**KanbanDesktop (WPF)**

- Windows Presentation Foundation```powershell### ✨ Principais Funcionalidades├── migration_sqlserver.sql      # Script de criação do banco

- XAML

- HttpClientcd KanbanDesktop



**KanbanWeb (Blazor)**dotnet run└── DEPLOY_WINDOWS_SERVER.md     # Guia completo de deploy

- Blazor Server

- Razor Components```

- JavaScript Interop

- 🔐 **Autenticação segura** com BCrypt```

---

**Web (Blazor):**

Projeto acadêmico - UNIP 2025

```powershell- 📊 **Quadro Kanban personalizável** (colunas e cards)

cd KanbanWeb

dotnet run- 🎫 **Sistema de Tickets** para usuários não-admin## 🚀 Funcionalidades

```````

Web disponível em: `http://localhost:5001`- 🖥️ **Interface Desktop** em WPF com tema dark

## 🔑 Credenciais- 🌐 **Interface Web** em Blazor Server### ✅ Gerenciamento de Colunas

- **Admin:** `admin` / `senha123` (acesso ao Kanban)- 👥 **Gestão de usuários** (admin e usuários comuns)- Criar colunas com nome e cor personalizados

- **Usuários:** `joao`, `maria`, `pedro` / `senha123` (sistema de tickets)

- 💾 **Persistência de sessão** (login automático)- Excluir colunas (deleta automaticamente todos os cards)

## 🛠️ Tecnologias

- 🎨 **UI moderna** com design responsivo- Coluna "Chamados" protegida contra exclusão

### KanbanAPI (Backend)

- ASP.NET Core 9.0- Cores disponíveis: vermelho, verde, azul, amarelo, laranja, rosa, marrom, preto, branco, cinza

- SQL Server LocalDB

- BCrypt para criptografia### 🏗️ Arquitetura

- Swagger para documentação

### ✅ Gerenciamento de Cards

### KanbanDesktop (Frontend)

- WPF (Windows Presentation Foundation)```- Criar cards com título e descrição

- XAML

- HttpClientKanbanSystem/- Editar cards existentes

### KanbanWeb (Frontend)├── 🔌 KanbanAPI # Backend REST API (.NET 9.0)- Excluir cards

- Blazor Server

- Razor Components├── 🖥️ KanbanDesktop # Cliente Desktop (WPF)- Mover cards entre colunas via drag-drop

- JavaScript Interop

└── 🌐 KanbanWeb # Cliente Web (Blazor Server)- Mover cards entre colunas via dropdown no modal

---

````

**Projeto acadêmico - UNIP 2025**

### ✅ Sistema de Tickets (Chamados)

**Padrões utilizados:**- Usuários não-admin podem criar tickets para admins

- Factory Pattern (DatabaseService)- Tickets são automaticamente convertidos em cards na coluna "Chamados"

- Repository Pattern (Controllers)- Acompanhamento de status em tempo real

- Dependency Injection- Integração completa com o fluxo Kanban

- RESTful API

### ✅ Integração com IA

## 🚀 Como Executar- Botão para reescrever descrição do card usando IA

- Integração com endpoint N8N personalizado

### Pré-requisitos- Melhoria de textos de tickets antes de enviar



- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)### ✅ Autenticação

- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)- Login com usuário e senha

- SQL Server LocalDB (incluído no Visual Studio)- Senhas armazenadas com hash BCrypt

- Diferenciação entre usuários admin e não-admin

### 1️⃣ Configurar o Banco de Dados- Cada usuário tem seu próprio board

- Sessão persistente

```powershell

# Executar script de configuração (PowerShell)## 🔧 Configuração

.\setup_localdb.ps1

```### 1. Configurar SQL Server



Ou executar manualmente:#### Opção A: SQL Server Local



```powershellEdite o arquivo `KanbanAPI/appsettings.json`:

# Conectar ao LocalDB

sqlcmd -S "(localdb)\MSSQLLocalDB" -i setup_database.sql```json

```{

  "ConnectionStrings": {

Isso vai criar:    "SqlServer": "Server=localhost;Database=KanbanDB;User Id=kanban_user;Password=SUA_SENHA;TrustServerCertificate=True;Encrypt=True;"

- ✅ Banco de dados `KanbanDB`  },

- ✅ Tabelas (Users, Columns, Cards, Tickets)  "AI": {

- ✅ Usuários de teste    "N8nEndpoint": "https://seu-endpoint-n8n.com/webhook/rewrite"

- ✅ Dados de exemplo  }

}

### 2️⃣ Executar a API (Backend)```



```powershell#### Opção B: SQL Server Remoto (Windows Server)

cd KanbanAPI

dotnet run```json

```{

  "ConnectionStrings": {

A API estará disponível em: `http://localhost:5000`    "SqlServer": "Server=SEU_SERVIDOR_IP,1433;Database=KanbanDB;User Id=kanban_user;Password=SUA_SENHA;TrustServerCertificate=True;Encrypt=True;"

  }

### 3️⃣ Executar o Cliente (escolha um)}

````

#### 🖥️ Desktop (WPF)

### 2. Criar Database

```````powershell

cd KanbanDesktopExecute o script SQL no SQL Server Management Studio (SSMS):

dotnet run

``````powershell

# Via SQL Server Management Studio

#### 🌐 Web (Blazor)# 1. Abra o arquivo migration_sqlserver.sql

# 2. Execute o script

```powershell

cd KanbanWeb# OU via linha de comando:

dotnet runsqlcmd -S localhost -U sa -P "SUA_SENHA" -i migration_sqlserver.sql

```````

Acesse: `http://localhost:5001`### 3. Configurar Endpoint de IA (N8N)

## 🔑 Credenciais de TesteO endpoint N8N deve:

- Aceitar POST com JSON: `{ "text": "texto original" }`

### Administrador- Retornar JSON: `{ "text": "texto reescrito" }`

- **Usuário:** `admin`

- **Senha:** `senha123`Edite o endpoint no `appsettings.json` da API:

- **Permissões:** Acesso total ao Kanban

```````json

### Usuários Comuns{

- **Usuários:** `joao`, `maria`, `pedro`  "AI": {

- **Senha:** `senha123`    "N8nEndpoint": "https://seu-webhook-n8n.com/rewrite"

- **Permissões:** Sistema de tickets apenas  }

}

## 📚 Documentação da API```



Com a API rodando, acesse a documentação Swagger:### 4. Configurar URLs dos Frontends



👉 `http://localhost:5000/swagger`#### Desktop (WPF)

Edite `KanbanDesktop/Services/ApiService.cs`:

### Principais Endpoints```csharp

public class ApiService

#### 🔐 Autenticação{

```http    private readonly HttpClient _httpClient;

POST /api/auth/register    # Registrar novo usuário

POST /api/auth/login       # Fazer login    public ApiService()

```    {

        _httpClient = new HttpClient

#### 📊 Kanban (Requer autenticação admin)        {

```http            BaseAddress = new Uri("http://localhost:5000/"),

GET    /api/columns              # Listar colunas            // Para servidor remoto: new Uri("http://SEU-SERVIDOR-IP/")

POST   /api/columns              # Criar coluna            Timeout = TimeSpan.FromSeconds(30)

DELETE /api/columns/{id}         # Deletar coluna        };

    }

GET    /api/cards                # Listar cards}

POST   /api/cards                # Criar card```

PUT    /api/cards/{id}           # Atualizar card

DELETE /api/cards/{id}           # Deletar card#### Web (Blazor)

```Edite `KanbanWeb/Services/ApiService.cs` da mesma forma.



#### 🎫 Tickets (Todos os usuários)## 🏃 Como Executar

```http

GET    /api/tickets/user/{userId}    # Tickets do usuário### Opção 1: Desenvolvimento Local

POST   /api/tickets                  # Criar ticket

PUT    /api/tickets/{id}/status      # Atualizar statusExecute o script de setup para restaurar e compilar tudo:

GET    /api/tickets/admins           # Listar admins (para atribuição)

``````powershell

cd pim

## 🛠️ Tecnologias Utilizadas.\setup.ps1

```````

### Backend

- **.NET 9.0** - Framework principalDepois, inicie a API:

- **ASP.NET Core** - Web API

- **Microsoft.Data.SqlClient** - Driver SQL Server```powershell

- **BCrypt.Net** - Criptografia de senhas.\start-api.ps1

- **Swashbuckle** - Documentação Swagger```

### Frontend DesktopCrie um usuário de teste (em outro terminal):

- **WPF (Windows Presentation Foundation)** - Interface desktop

- **XAML** - Markup de interface```powershell

- **HttpClient** - Comunicação com API.\create-user.ps1

```````

### Frontend Web

- **Blazor Server** - Framework web interativoOu com credenciais customizadas:

- **Razor Components** - Componentes reutilizáveis

- **JavaScript Interop** - sessionStorage```powershell

.\create-user.ps1 -username "meuusuario" -password "minhasenha"

### Banco de Dados```

- **SQL Server LocalDB** - Banco de dados local

- **T-SQL** - Queries e procedures### Opção 2: Manual



## 📂 Estrutura do Projeto### 1. Iniciar a API



``````powershell

KanbanAPI/cd pim/KanbanAPI

├── Controllers/          # Endpoints da APIdotnet restore

│   ├── AuthController.csdotnet run

│   ├── CardsController.cs```

│   ├── ColumnsController.cs

│   └── TicketsController.csA API estará disponível em `http://localhost:5000`

├── Models/              # Modelos de dados

│   └── Models.cs### 2. Iniciar o Frontend Desktop (WPF)

├── DTOs/                # Data Transfer Objects

│   └── DTOs.cs```powershell

├── Services/            # Lógica de negóciocd pim/KanbanDesktop

│   ├── DatabaseService.csdotnet restore

│   └── AIService.csdotnet run

└── Program.cs           # Configuração da API```



KanbanDesktop/### 3. Iniciar o Frontend Web (Blazor)

├── Views/               # Janelas e diálogos

│   ├── LoginWindow.xaml```powershell

│   ├── MainWindow.xamlcd pim/KanbanWeb

│   ├── TicketsWindow.xamldotnet restore

│   └── ...dotnet run

├── Services/            # Serviços de comunicação```

│   ├── ApiService.cs

│   ├── SessionManager.csO frontend web estará disponível em `http://localhost:5001` ou similar.

│   └── TicketService.cs

└── App.xaml            # Configuração do app## 📊 Estrutura do Banco de Dados



KanbanWeb/As tabelas já devem existir no Supabase:

├── Components/

│   └── Pages/          # Páginas Blazor### pim_users

│       ├── Home.razor- `id` (uuid, PK)

│       └── Tickets.razor- `username` (varchar)

├── Services/           # Serviços de comunicação- `password` (varchar, hash BCrypt)

│   ├── ApiService.cs- `created_at` (timestamp)

│   ├── AuthService.cs

│   └── SessionService.cs### pim_columns

└── Program.cs          # Configuração do app- `id` (uuid, PK)

```- `user_id` (uuid, FK)

- `name` (varchar)

## 🎨 Recursos Visuais- `color` (varchar)

- `position` (integer)

### Interface Desktop (WPF)- `created_at` (timestamp)

- ✨ Tema dark moderno

- 🎯 Drag-and-drop de cards (planejado)### pim_cards

- 💬 Diálogos modais para criação/edição- `id` (uuid, PK)

- 🔄 Atualização em tempo real- `column_id` (uuid, FK)

- `title` (varchar)

### Interface Web (Blazor)- `description` (text)

- 🎨 Design responsivo- `position` (integer)

- 🌊 Animações suaves- `created_at` (timestamp)

- 📱 Mobile-friendly (planejado)- `updated_at` (timestamp)

- ⚡ Renderização interativa

## 🎨 Paleta de Cores

## 🔒 Segurança

- **Primária**: Verde (#00FF00)

- ✅ Senhas criptografadas com BCrypt (custo 11)- **Secundária**: Verde Escuro (#00AA00)

- ✅ Validação de entrada em todos os endpoints- **Background**: Preto (#0A0A0A)

- ✅ CORS configurado- **Cards**: Cinza Escuro (#1A1A1A)

- ✅ SQL parametrizado (proteção contra SQL Injection)- **Texto**: Branco (#FFFFFF)

- ✅ Gestão segura de sessões

## 📝 Endpoints da API

## 🐛 Troubleshooting

### Autenticação

### Erro: "A network-related or instance-specific error occurred"- `POST /api/auth/login` - Login

- `POST /api/auth/register` - Registrar novo usuário

**Solução:**

```powershell### Colunas

# Verificar se o LocalDB está rodando- `GET /api/columns/user/{userId}` - Listar colunas do usuário

sqllocaldb info MSSQLLocalDB- `POST /api/columns` - Criar coluna

- `PUT /api/columns/{id}` - Atualizar coluna

# Iniciar se necessário- `DELETE /api/columns/{id}` - Deletar coluna

sqllocaldb start MSSQLLocalDB

```### Cards

- `GET /api/cards/column/{columnId}` - Listar cards de uma coluna

### Erro: "Login failed for user"- `GET /api/cards/user/{userId}` - Listar todos os cards do usuário

- `POST /api/cards` - Criar card

**Solução:** Use **Windows Authentication**. A connection string no `appsettings.json` está configurada corretamente:- `PUT /api/cards/{id}` - Atualizar card

```json- `DELETE /api/cards/{id}` - Deletar card

"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=KanbanDB;Trusted_Connection=True;TrustServerCertificate=True;"- `POST /api/cards/{id}/rewrite` - Reescrever descrição com IA

```````

## 🐛 Logs e Debug

### API não responde

O sistema possui logs extensivos no console para debug:

Verifique se a porta 5000 está livre:- `[API]` - Logs da camada de API

```powershell- `[DB]` - Logs de banco de dados

netstat -ano | findstr :5000- `[AUTH]` - Logs de autenticação

```- `[COLUMNS]` - Logs de colunas

- `[CARDS]` - Logs de cards

## 📈 Roadmap- `[AI]` - Logs de integração com IA

- `[DRAG]` - Logs de drag-drop

- [ ] Drag-and-drop de cards- `[LOGIN]` - Logs de login

- [ ] Notificações em tempo real (SignalR)- `[MAIN]` - Logs gerais

- [ ] Upload de anexos em tickets- `[DIALOG]` - Logs de diálogos/modais

- [ ] Comentários em cards

- [ ] Histórico de alterações## 📦 Dependências

- [ ] Relatórios e estatísticas

- [ ] Integração com IA (sugestões)### API

- [ ] Aplicativo mobile (MAUI)- Microsoft.AspNetCore.OpenApi

- Swashbuckle.AspNetCore

## 👨‍💻 Autor- Npgsql

- BCrypt.Net-Next

**Arthur Pagiatto Nunes**

- Instituição: UNIP - Universidade Paulista### Desktop (WPF)

- Curso: Análise e Desenvolvimento de Sistemas- Newtonsoft.Json

- Projeto: TCC 2025

### Web (Blazor)

## 📄 Licença- Newtonsoft.Json

Este projeto é licenciado sob a [MIT License](LICENSE.md).## 💡 Dicas

## 🤝 Contribuindo1. **Primeiro Usuário**: Use a rota `/api/auth/register` para criar o primeiro usuário

2. **Drag and Drop**: Arraste os cards entre as colunas para movê-los

Contribuições são bem-vindas! Para contribuir:3. **Edição Rápida**: Clique no card para abrir o modal de detalhes

4. **IA**: Certifique-se de configurar o endpoint N8N antes de usar a reescrita com IA

1. Faça um Fork do projeto5. **Logs**: Monitore o console para debugar problemas

1. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)

1. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)## 🔒 Segurança

1. Push para a branch (`git push origin feature/MinhaFeature`)

1. Abra um Pull Request- Senhas são armazenadas com hash BCrypt

- Conexão com Supabase via SSL

## 📞 Suporte- CORS habilitado na API (ajuste em produção)

Em caso de dúvidas ou problemas:## 🎯 To-Do para Produção

1. Verifique a [documentação](documentaçao.html)- [ ] Adicionar autenticação JWT na API

2. Consulte os [exemplos de API](API_EXAMPLES.md)- [ ] Implementar validações mais robustas

3. Abra uma [Issue](../../issues)- [ ] Adicionar testes unitários

- [ ] Configurar CORS corretamente

---- [ ] Adicionar rate limiting

- [ ] Implementar cache

<div align="center">- [ ] Adicionar paginação nos endpoints

**Desenvolvido com ❤️ usando .NET e C#**---

⭐ Se este projeto foi útil, considere dar uma estrela!Desenvolvido para trabalho de faculdade - PIM

</div>
