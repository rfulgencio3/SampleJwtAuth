# Desafios para praticar no seu computador

Este roteiro é voltado aos alunos de Sistemas de Informação. Escolha um desafio, implemente a solução no seu próprio projeto e explique o que aprendeu. Os exercícios são propostas: as funcionalidades abaixo ainda não estão implementadas.

## Preparar o ambiente

1. Instale o SDK do .NET 10 e o Git. Use um editor como VS Code ou Visual Studio com suporte ao .NET 10.
2. No GitHub, faça um **fork** deste repositório: isso cria uma cópia na sua conta.
3. Clone o seu fork, substituindo `SEU_USUARIO` pelo seu usuário do GitHub:

```powershell
git clone https://github.com/SEU_USUARIO/SampleJwtAuth.git
cd SampleJwtAuth
git switch -c desafio/minha-solucao
dotnet build SampleJwtAuth.slnx
# Generate a local signing key before starting the API.
$env:Jwt__SigningKey = [guid]::NewGuid().ToString('N') + [guid]::NewGuid().ToString('N')
dotnet run --project src/SampleJwtAuth.csproj
```

Se preferir estudar sem Git, use **Code > Download ZIP** no GitHub, extraia os arquivos e abra a pasta no editor. Para enviar um PR depois, utilize um fork com Git.

Abra `http://localhost:5080/swagger`. Cadastre `user` com a senha `pass@123`, faça login e use o token para consultar os times. Mantenha o terminal em execução durante os testes. Caso a porta esteja ocupada, encerre a outra execução da API ou configure outra porta no perfil local.

## Desafio 1 — Consultar um time pelo identificador

Crie `GET /api/SoccerScoreTeams/{id}` para retornar apenas o time solicitado.

- Exija JWT válido, assim como na consulta da lista.
- Retorne `200 OK` com o time quando o identificador existir.
- Retorne `404 Not Found` quando não existir.
- Mantenha o controller responsável pelo HTTP e coloque a consulta no caso de uso/repositório.
- Documente a nova rota e teste pelo Swagger.

Critérios de conclusão: o identificador `1` retorna Cruzeiro, um identificador inexistente retorna `404` e uma chamada sem token retorna `401`.

## Desafio 2 — Salvar usuários em um banco persistente

Substitua o provedor EF Core InMemory por SQLite. Diferentemente do banco atual, um arquivo SQLite permite manter os dados depois de reiniciar a API.

- Adicione um provedor SQLite compatível com a versão do EF Core utilizada no projeto.
- Configure `AuthDbContext` com uma conexão para um arquivo local.
- Utilize migrations do EF Core para criar e atualizar a estrutura do banco.
- Mantenha o armazenamento de senhas como hash.
- Preserve a normalização dos nomes e a rejeição de cadastros duplicados.
- Adapte a proteção contra duplicidade: o `lock` do exemplo vale somente para um processo; o banco deve garantir a unicidade e a API deve tratar a violação retornando `409`.
- Inclua arquivos locais do banco no `.gitignore` e documente os comandos de preparação.

Critérios de conclusão: cadastre um usuário, encerre a API, execute-a novamente e faça login sem repetir o cadastro. Confirme também senha incorreta (`401`) e cadastro duplicado (`409`).

## Desafio 3 — Cadastrar e consultar times no banco

Após o desafio de persistência, crie `POST /api/SoccerScoreTeams` e substitua a lista fixa por uma tabela.

- Defina os campos obrigatórios e valide os dados recebidos.
- Exija autenticação nas novas rotas.
- Retorne `201 Created` com a localização do time cadastrado.
- Permita consultar o novo time pelo identificador e pela lista.
- Preserve o cadastro inicial de Cruzeiro e dos demais times sem duplicá-los a cada inicialização.

Critério de conclusão: um time criado deve continuar disponível após reiniciar a API.

## Desafio extra — Verificar o comportamento da autenticação

Crie testes de integração para cadastro, login e consulta protegida. Verifique token ausente, assinatura adulterada e token expirado. Para expiração, utilize um relógio controlado no teste, aproveitando o `TimeProvider`, em vez de aguardar 30 minutos.

Não remova a validação do token para fazer os testes passarem. A senha e seu hash não devem aparecer nas respostas nem no JWT.

## Como apresentar e contribuir

Antes de concluir, confira se o projeto compila e se os cenários do desafio funcionam. No seu README, explique a solução, os comandos de execução e como você verificou o resultado. Não publique senhas reais, tokens ou chaves de ambientes pessoais.

```powershell
git add .
git commit -m "Implement student challenge"
git push -u origin desafio/minha-solucao
```

No GitHub, abra um **pull request (PR)** do seu fork para o repositório original. Descreva o desafio escolhido, o que mudou e quais testes realizou. Você também pode manter a solução apenas no seu fork para estudo.

Perguntas para discussão em aula: por que o JWT é validado sem consultar o banco em cada chamada? Por que o hash da senha não deve estar no token? O que muda quando os dados precisam sobreviver ao reinício da aplicação?

A chave local gerada vale para o processo iniciado nesse terminal. Ao trocar a chave, os tokens anteriores deixam de ser aceitos. Sem `Jwt__SigningKey`, a validação de configuração impede a API de iniciar.
