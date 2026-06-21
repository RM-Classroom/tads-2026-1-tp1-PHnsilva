# Rota Certa — Locadora de Veículos

Aplicação ASP.NET Core 8 para gestão de uma locadora, com API REST, Entity Framework Core, SQL Server e um painel MVC responsivo. O frontend oferece dashboard e CRUD visual completo para clientes, fabricantes, categorias, veículos e aluguéis.

## Executar o projeto

Pré-requisitos: .NET SDK 8 e SQL Server Express disponível em `.\SQLEXPRESS`.

```powershell
dotnet tool restore
dotnet ef database update
dotnet run --launch-profile http
```

Acesse:

- aplicação: `http://localhost:5062/`
- Swagger: `http://localhost:5062/swagger`
- wireframes: abra [`docs/wireframes/index.html`](docs/wireframes/index.html) diretamente no navegador

Se o SQL Server estiver indisponível, o painel entra automaticamente em modo de demonstração e usa no navegador os dados fictícios registrados nas evidências originais do Swagger. Esse fallback não altera a API nem o banco. Para reiniciá-lo, remova a chave `rota-certa-demo-v2` do `localStorage`.

## Funcionalidades

- dashboard com totais de frota, clientes, contratos e receita;
- gráfico de veículos por fabricante e painel de veículos sem aluguel;
- listagem, criação, edição e exclusão das cinco entidades;
- busca e filtros combinados específicos de cada entidade;
- formulários com relacionamentos entre cliente, veículo, fabricante e categoria;
- feedback visual de sucesso, validação e erros de integridade;
- layout responsivo para desktop, tablet e celular;
- integração com os endpoints existentes em `/api`.

## Arquitetura

```text
Controllers/            entrada HTTP e endpoints REST
Application/DTOs/       contratos de entrada e saída
Domain/Entities/        entidades do domínio
Infrastructure/         DbContext e migrations
Shared/                 middleware e helpers
Views/Home/             shell Razor do painel
wwwroot/css e js/       frontend do painel
docs/wireframes/        protótipo navegável compartilhável
```

## Documentação da entrega

- [Wireframes navegáveis](docs/wireframes/index.html)
- [Evidências originais do Swagger](testes/testes_swagger.pdf)

O diretório `docs/` pode ser publicado pelo GitHub Pages, Vercel ou serviço equivalente. No GitHub Pages, configure a publicação a partir da branch principal e use `docs/wireframes/index.html` como endereço do protótipo.

## Validação rápida

```powershell
dotnet build CRUD.sln
node --check wwwroot/js/app.js
```

Depois, percorra cada item do menu, combine pelo menos dois filtros, crie/edite/exclua um registro sem vínculos e confirme os indicadores do dashboard.
