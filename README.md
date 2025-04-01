# 📦 API de Pedidos

Esta API permite criar, consultar e listar pedidos com seus respectivos itens, impostos e status.

---

### 🔗 Base URL

```
http://localhost:5098/api/Pedidos
```

```
http://localhost:5098/swagger
```
---

## 🛠️ Tecnologias Utilizadas

- **.NET Core 8** - Framework para desenvolvimento da API.
- **RabbitMQ** - Sistema de mensageria para comunicação assíncrona entre os sistemas.
- **MassTransit** - Biblioteca para integração com RabbitMQ e outras soluções de mensageria.
- **Serilog** - Ferramenta de logging estruturado.
- **Swagger** - Para documentação e testes da API.

---

## 📤 Criar um Pedido

### `POST /api/Pedidos`

Cria um novo pedido com cliente, itens e valores.

#### 📥 Exemplo de Requisição `curl`

```bash
curl -X 'POST'   'http://localhost:5098/api/Pedidos'   -H 'accept: text/plain'   -H 'Content-Type: application/json'   -d '{
    "pedidoId": 16,
    "clienteId": 45,
    "itens": [
      {
        "produtoId": 55,
        "quantidade": 340,
        "valor": 1500.10
      }
    ]
  }'
```

---

## 📄 Consultar Pedidos por Status

### `GET /api/Pedidos?status=Criado`

Retorna a lista de pedidos com o status informado.

#### 📥 Exemplo de Requisição

```bash
curl -X 'GET' 'http://localhost:5098/api/Pedidos?status=Criado'
```

#### 📤 Exemplo de Resposta

```json
[
  {
    "id": 1,
    "pedidoId": 3,
    "clienteId": 2,
    "imposto": 940,
    "itens": [
      {
        "produtoId": 5,
        "quantidade": 47,
        "valor": 100
      }
    ],
    "status": "Criado"
  },
  {
    "id": 2,
    "pedidoId": 14,
    "clienteId": 45,
    "imposto": 102006.8,
    "itens": [
      {
        "produtoId": 55,
        "quantidade": 340,
        "valor": 1500.1
      }
    ],
    "status": "Criado"
  }
]
```

---

## 🔍 Consultar Pedido por ID

### `GET /api/Pedidos/{id}`

Retorna os dados de um pedido específico.

#### 📥 Exemplo de Requisição

```bash
curl -X 'GET' 'http://localhost:5098/api/Pedidos/2'
```

#### 📤 Exemplo de Resposta

```json
{
  "id": 2,
  "pedidoId": 14,
  "clienteId": 45,
  "imposto": 102006.8,
  "itens": [
    {
      "produtoId": 55,
      "quantidade": 340,
      "valor": 1500.1
    }
  ],
  "status": "Criado"
}
```

---

## 🛠️ Possíveis Status de Pedido

- `Criado`
- `Processando`
- `Concluido`
- `Cancelado`
