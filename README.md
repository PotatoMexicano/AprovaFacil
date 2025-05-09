
# AprovaFácil

**AprovaFácil** é uma plataforma moderna de gestão e aprovação de solicitações corporativas. Pensada para empresas que precisam de organização, transparência e agilidade, a solução oferece um sistema multi-tenant com controle por níveis de aprovação, gerenciamento de usuários e notificações em tempo real.

## 🧠 Principais Benefícios

- ✅ Processo de aprovação em dois níveis (gerente e diretor)
- 🔔 Notificações em tempo real via SignalR
- 🧑‍💼 Controle de permissões e cargos por empresa
- 🧾 Multi-tenancy com planos de assinatura via Stripe
- 📄 Geração de relatórios em PDF com DinkToPdf
- 🌐 Front-end moderno com Tailwind + framework JS
- 📦 API robusta com ASP.NET + Entity Framework

---

## 🚀 Como rodar localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Node.js 20+](https://nodejs.org/)
- [SQL Server (opcional)] ou utilize InMemory para testes

### 1. Clonar o repositório

```bash
git clone https://github.com/PotatoMexicano/AprovaFacil.git
cd AprovaFacil
```

### 2. Rodar o back-end

```bash
cd AprovaFacil.Server
dotnet run
```

### 3. Rodar o front-end

```bash
cd AprovaFacil.Client
npm install --force
npm run dev
```

---

## 🐳 Como rodar com Docker

### 1. Build da imagem

```bash
docker build -t aprovafacil-app .
```

### 2. Rodar o container

```bash
docker run -p 80:80 aprovafacil-app
```

> Isso irá expor a aplicação na porta `80`. A build já compila tanto o front quanto o back-end em uma única imagem.

---

## 📂 Estrutura do Projeto

```
AprovaFacil.Application/   # Regras de negócio e serviços
AprovaFacil.Client/        # Interface do usuário (Web)
AprovaFacil.Server/        # API REST (ASP.NET)
Dockerfile                 # Docker multi-stage build
```

---

## 📬 Contato

Para dúvidas, sugestões ou melhorias:
**Gabriel Cordeiro** - [gabrielcordeirow@hotmail.com](mailto:gabrielcordeirow@hotmail.com)

---

Feito com 💙 por [PotatoMexicano](https://github.com/PotatoMexicano)
