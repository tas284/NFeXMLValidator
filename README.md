
# 📄 NFeXMLValidator - ASP.NET Core Web API

Projeto para validação de **XMLs de Nota Fiscal Eletrônica (NFe)** com base nos arquivos **XSD oficiais** disponibilizados pelo [Portal Nacional da NFe](https://www.nfe.fazenda.gov.br/portal/principal.aspx).  
Inspirado e adaptado do projeto [web.api.xml.schema.validation](https://github.com/silvairsoares/web.api.xml.schema.validation).

---

## ✅ Requisitos

- [.NET SDK 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Docker (opcional)

---

## 🚀 Como Executar

### 🔧 Build e Execução Local

Entre no diretório raiz do projeto e execute:

```bash
dotnet build && dotnet run
```

---

## 🐳 Execução com Docker

### 📦 Executar com `docker-compose` (modo desenvolvimento)

```bash
docker compose build
docker compose up -d
```

### 🏭 Executar com `docker-compose` (modo produção)

```bash
docker compose -f .\docker-compose.prod.yml up
```

---

### 🛠️ Build manual da imagem Docker

```bash
docker build -t nfe-xml-validator-api .
```

### ▶️ Executar com `docker run`

```bash
docker run -dp 5000:80 \
  --env-file=".env" \
  --network backend \
  nfe-xml-validator-api
```

---

## 📘 Documentação da API via Swagger

Se a variável de ambiente `ASPNETCORE_ENVIRONMENT` estiver definida como `Development`, a interface [Swagger UI](https://swagger.io/tools/swagger-ui/) estará habilitada automaticamente.

🔗 Acesse: [http://localhost:5001/swagger/index.html](http://localhost:5001/swagger/index.html)

Para ambientes de produção, defina `ASPNETCORE_ENVIRONMENT=Production` para desativar o Swagger.

---

## 🐳 DockerHub

Se quiser apoiar o projeto, você pode avaliá-lo no Docker Hub:

👉 [DockerHub - NFe XML Validator API](https://hub.docker.com/repository/docker/tiagosaldanha/nfe-xml-validator-api/general)

---

## 📄 Licença

Este projeto pode ser licenciado conforme sua preferência. Adicione um arquivo `LICENSE` para mais informações.

---

## 👨‍💻 Desenvolvido por

Tiago (NFeXMLValidator)

---
