# Golden Raspberry Awards API

Esta é uma API RESTful para leitura da lista de indicados e vencedores da categoria Pior Filme do Golden Raspberry Awards.

## Requisitos do Sistema

1. Ler o arquivo CSV dos filmes e inserir os dados em uma base de dados ao iniciar a aplicação.

## Tecnologias Utilizadas

- .NET Core 8
- Entity Framework Core
- Banco de dados Sqlite

## Estrutura do Projeto

O projeto está estruturado da seguinte forma:

- **Controllers**: Contém os controladores da API.
- **Models**: Contém os modelos de dados.
- **Data**: Contém o contexto do banco de dados e a classe de inicialização dos dados.
- **Tests**: Contém os testes de integração.

## Como Rodar o Projeto

1. Clone o repositório para sua máquina local.
   ```bash
   git clone https://github.com/isaquepf/goldenRaspberryWard
   cd goldenRaspberryWard
   dotnet run
