# GoodHamburger - Challenge

Implementação de uma aplicação para montagem de pedidos de hambúrguer com regra de combo e desconto, composta por:

* API ASP.NET Core (cadastro/consulta/atualização/exclusão de pedidos)
* Front-end Blazor (menu, carrinho e histórico de pedidos)
* Persistência com EF Core (SQLite por padrão, com opção de Postgres)

## Objetivo do challenge

Permitir que o usuário monte um pedido com as categorias:

* Sanduíche
* Extra
* Bebida

Aplicando os descontos:

* Sanduíche + Extra + Bebida => 20%
* Sanduíche + Bebida => 15%
* Sanduíche + Extra => 10%
* Qualquer outro caso => sem desconto

## Arquitetura escolhida e justificativa

Foi adotada uma arquitetura em camadas (estilo Clean Architecture simplificada), separando responsabilidades em quatro projetos:

* GoodHamburger.API: camada de exposição HTTP (controllers e configuração)
* GoodHamburger.Application: regras de negócio e casos de uso (ex.: CreateOrderHandler, DiscountService)
* GoodHamburger.Domain: entidades e conceitos centrais (Order, MenuItem, Category)
* GoodHamburger.Infrastructure: acesso a dados com EF Core e repositórios

Motivos da escolha:

* Separação de responsabilidades: regras de negócio ficam fora da API e da persistência.
* Testabilidade: handlers e serviços da camada Application podem ser testados com mocks de repositório.
* Evolução simples: troca de banco, ajuste de endpoint ou mudança de UI sem impactar o domínio.
* Legibilidade para avaliação técnica: fica claro onde cada decisão foi implementada.

## Regra de negócio: um item por categoria

O backend valida que cada pedido possui no máximo um item por categoria. Essa regra está centralizada no handler de pedidos, garantindo consistência mesmo que outro cliente (além do front atual) consuma a API.

## Por que não há mensagem de erro no front ao escolher mais de um item da mesma categoria

Essa foi uma decisão de UX intencional.

No front, o carrinho usa um dicionário por categoria (CartService), então, ao selecionar outro item da mesma categoria, o comportamento é substituir automaticamente o item anterior, e não empilhar.

Em outras palavras:

* Se o usuário escolhe X-Burger e depois X-Bacon, o sanduíche anterior é trocado.
* O usuário permanece em um fluxo simples, sem bloqueios e sem alerta de erro.

Motivação dessa escolha:

* Evitar fricção desnecessária em uma regra previsível.
* Reduzir mensagens de erro para um caso em que o sistema pode se autocorrigir.
* Tornar a experiência mais fluida para montagem rápida de combo.

Observação importante:

* Mesmo com essa prevenção no front, a regra continua protegida no backend (defesa em profundidade).

## Tecnologias

* .NET
* ASP.NET Core Web API
* Blazor (Interactive Server)
* Entity Framework Core
* SQLite (default) / PostgreSQL (opcional)
* Swagger/OpenAPI

## Endpoints principais

* GET /api/menu
* POST /api/orders
* GET /api/orders/pedido/{id}
* GET /api/orders/pedidos
* PUT /api/orders/pedido/{id}
* DELETE /api/orders/pedido/{id}

## Como executar

### 1. API

Na pasta GoodHamburger.API:

```bash
dotnet restore
dotnet run
```

A API aplica migrations e seed inicial automaticamente na inicialização.

### 2. WEB

Na pasta GoodHamburger.WEB:

```bash
dotnet restore
dotnet run
```

Importante: o front está configurado para consumir a API em [http://localhost:5063/](http://localhost:5063/).

## Seed inicial

Categorias criadas automaticamente:

* Sanduíche (SANDWICH)
* Extra (SIDE)
* Bebida (DRINK)

Itens de menu iniciais:

* X Burger
* X Egg
* X Bacon
* Batata frita
* Refrigerante

## Testes unitários

Os testes unitários já foram implementados no projeto `GoodHamburger.Tests`, cobrindo os principais cenários de negócio e API.

Cobertura atual:

* Regras de desconto (`DiscountService`): valida os cenários de 20%, 15%, 10% e ausência de desconto.
* Handler de criação de pedido (`CreateOrderHandler`): valida criação de pedido, cálculo de subtotal/desconto/total, retorno dos produtos e cenários de erro (item não encontrado e combinação inválida).
* Controllers (`MenuController` e `OrdersController`): valida respostas HTTP esperadas para fluxos de sucesso e falha.

Stack de testes:

* xUnit
* Moq
* Microsoft.NET.Test.Sdk
