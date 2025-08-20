# OpenFgaPoc

PoC (**Prova de Conceito**) que integra uma **API .NET 8** com **OpenFGA** usando **Postgres**.  
Tudo é orquestrado com **Docker Compose**.

## Tecnologias

- .NET 8 / ASP.NET Core
- Entity Framework Core (Code-First)
- PostgreSQL
- OpenFGA
- Docker & Docker Compose

---

## Como rodar

### 1. Clonar o projeto
```
git clone https://github.com/artcarvalho/OpenFgaPoc.git
cd OpenFgaPoc
```
### 2. Subir os containers
```
docker compose up --build
```
## Endpoints

- API .NET (Swagger):

  - http://localhost:5002/swagger

  - https://localhost:5001/swagger

- OpenFGA:

  - http://localhost:8080
