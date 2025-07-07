# Developer Evaluation Sales CRUD

Este projeto é focado em operações de CRUD de vendas, usuários e produtos, utilizando arquitetura moderna, autenticação JWT, cache, banco relacional e NoSQL, além de práticas recomendadas de desenvolvimento .NET.

## Tecnologias Utilizadas

- **.NET 8.0** (WebAPI)
- **Entity Framework Core** (PostgreSQL)
- **Redis** (Cache)
- **MongoDB** (NoSQL)
- **Docker & Docker Compose**
- **MediatR** (CQRS)
- **AutoMapper**
- **FluentValidation**
- **Serilog** (Logging)
- **JWT** (Autenticação)
- **Swagger** (Documentação de API)

## Arquitetura

O projeto está dividido em múltiplas camadas:

- **Domain:** Entidades, enums, eventos de domínio, validações e repositórios.
- **Application:** Handlers, comandos, validações e lógica de aplicação.
- **ORM:** Contexto do Entity Framework, configurações e repositórios concretos.
- **IoC:** Injeção de dependências e inicialização de módulos.
- **WebApi:** Controllers, endpoints, middlewares, configuração de autenticação, documentação e inicialização da aplicação.

## Serviços Docker

O `docker-compose.yml` orquestra os seguintes serviços:

- **WebAPI:** API principal em .NET 8.
- **PostgreSQL:** Banco de dados relacional.
- **MongoDB:** Banco de dados NoSQL.
- **Redis:** Cache distribuído.

## Como Executar

1. **Pré-requisitos:**

   - Docker e Docker Compose instalados.

2. **Execução:**  
   No diretório raiz do projeto, execute:

   ```sh
   docker-compose up
   ```

   Isso irá subir todos os serviços necessários. A API estará disponível em `http://localhost:49959/swagger` para exploração dos endpoints.

3. **Configurações:**
   - As strings de conexão e variáveis de ambiente já estão configuradas para uso local via Docker.
   - O banco de dados será migrado automaticamente ao iniciar a aplicação.

## Testes

O projeto possui estrutura para testes unitários, funcionais e de integração, utilizando xUnit, FluentAssertions, NSubstitute e Bogus.

- Para rodar os testes, utilize os projetos dentro da pasta `tests/`.

## Próximos Passos

- [ ] Adicionar message broker para eventos de vendas
- [ ] Adicionar autenticação e autorização em cada endpoint
- [ ] Criar testes de integração
- [ ] Criar pipeline de CI/CD
