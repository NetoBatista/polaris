[Inglês](/README.md) | Português

# Polaris
## O que é?
Polaris é uma API de gerenciamento de usuários e aplicações cujo o objetivo é realizar login através de MFA, senha ou firebase.

## Como funciona?
A API oferece os seguintes grupos de rotas:

- **/Application**
    - Permite gerenciar aplicações

- **/Authentication**
    - Permite realizar autenticação (MFA, Senha ou firebase).

- **/Members**
    - Permite vincular o usuário a uma ou mais aplicações.

- **/User**
    - Permite gerenciar usuários no banco de dados.

## Como começar?

O primeiro passo é registrar as aplicações desejadas fornecendo os campos necessários.

Após registrar as aplicações, o segundo passo é registrar o usuário através da rota POST /User.

O próximo passo é criar um link entre o usuário e suas aplicações. Cada usuário pode ser associado a uma ou mais aplicações e pode ter diferentes tipos de autenticação. 

A associação pode ser realizada através da rota POST /Member, fornecendo o ID do usuário, o ID da aplicação. É possível informar a senha do usuário para aquela aplicação mas ainda sim pode ser possível realizar login através de MFA ou firebase.

Por fim, você pode gerar um token JWT para integração com outros sistemas através da rota POST /Authentication.

## Sobre a API
A API foi desenvolvida em C# .NET 8, utilizando a metodologia DDD (Domain-Driven Design). Algumas das funcionalidades incluídas são:
- Entity Framework
- SQL Server
- Migrations
- ServiceBus

## Configuração e execução da API
Todas as variáveis podem ser configuradas como variáveis de ambiente ou através do arquivo appSettings.json

### Configuração rápida
Para uma execução rápida da API, basta definir uma variável de ambiente `ConnectionString` apontando para um SQL Server. Na primeira execução, as migrações serão validadas e executadas, criando a estrutura de tabelas necessária.

Exemplo de ConnectionString:
```text
Server=localhost;Initial Catalog=polaris;Persist Security Info=False;User ID=sa;Password=Senha123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;
```

Modifique os campos necessários no exemplo acima, como `Server`, `User ID` e `Password`, e pronto. Em seguida, vá para `http://localhost:5001/swagger` para visualizar a interface do Swagger. Você também pode editar a ConnectionString no arquivo `launchSettings.json` do projeto.

### Outros parâmetros
Existem outros parâmetros na aplicação que podem ser modificados, encontrados no arquivo `appsettings.json`.

Um deles é a `ConnectionString`, que também está presente nesse arquivo. No entanto, há uma hierarquia ao rodar o projeto: se uma variável de ambiente `ConnectionString` for fornecida, ela será priorizada. Caso contrário, o projeto lerá do arquivo `appsettings.json`.

Exemplo da configuração da ConnectionString através do appsettings.json:
```json
"Database": {
  "ConnectionString": "Server=localhost;Initial Catalog=polaris;Persist Security Info=False;User ID=sa;Password=Senha123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
}
```

Também há parâmetros para o JWT gerado na rota POST `/Authentication`. Você pode especificar o segredo e o tempo de expiração do token (em minutos).

Se outras aplicações reutilizarem o JWT do Polaris, é importante verificar se elas têm o parâmetro `ValidateIssuerSigningKey` habilitado. Caso afirmativo, elas precisam ter o mesmo `Secret` especificado no `appsettings.json` para funcionar corretamente.

Exemplo da configuração do JWT através do appsettings.json:
```json
"JwtToken": {
  "Secret": "d8d69f45-3d95-494f-a016-cba9143a503d",
  "Expire": "5"
}
```

O Token JWT também pode ser configurado através de variáveis de ambiente.

```
JwtTokenSecret
```

```
JwtTokenExpire
```

## Eventos

Estamos adicionando callbacks através de eventos, e atualmente há uma configuração pronta para utilizar o recurso ServiceBus da Azure.

Você pode informar a ConnectionString para conectar ao recurso, assim como o nome da fila através do appSettings.json.

```
"ServiceBus": {
  "ConnectionString": "",
  "Queue": ""
}
```

Ou através de variáveis de ambiente:

```
ServiceBusConnectionString
```

```
ServiceBusQueue
```

## Considerações finais
Fique à vontade para fazer um fork e realizar as modificações necessárias. Além disso, você pode contribuir através da aba Issues ou fazendo Pull Requests (PR).