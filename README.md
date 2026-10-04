# cvhub

Aplicação desenvolvida para o desafio técnico de cadastro de currículos do CIEE/PR. Permite cadastrar candidatos manualmente ou importar um PDF para sugerir nome, e-mail e telefone. As sugestões podem ser corrigidas antes de salvar.

## Funcionalidades

- Formulário único para os dois caminhos de cadastro.
- Nome e e-mail obrigatórios, com validação do formato do e-mail.
- Telefone, área/cargo de interesse e resumo profissional opcionais.
- Persistência no SQL Server, listagem e consulta de detalhes pela API.
- Importação de PDF de até 5 MB, realizada no backend.
- Falhas de importação não impedem o cadastro manual.
- Mensagens de validação, erro e confirmação de cadastro.

## Tecnologias e versões

Preencha os itens abaixo com as versões efetivamente instaladas antes da entrega.

| Tecnologia | Versão |
| ASP.NET Core / framework | .NET 8  |
| SDK .NET | 9.0.317 |
| Entity Framework Core SQL Server e Tools | 8.0.31 |
| PdfPig | 0.1.16 |
| Swashbuckle.AspNetCore | 8.1.4 |
| React e React DOM | 19.3.0 |
| Vite | 8.3.2 |
| Node.js | 24.13.0 |
| npm | 11.6.2 |
| SQL Server | Microsoft SQL Server 2017  |

Ferramentas de desenvolvimento: Visual Studio 2022, VS Code e SQL Server Management Studio.

## Organização

- `backend/`: projeto ASP.NET Core, controllers, modelo, contexto e migrations.
- `frontend/`: aplicação React, componentes e comunicação com a API.
- `exemplos/curriculo-ficticio.pdf`.
- `DESENVOLVIMENTO.md`: decisões e registro do processo.
- video-demonstracao-cvhub.mp4: Vídeo demonstrando o funcionamento do sistema.

## Pré-requisitos

- SDK .NET 8 compatível com o projeto.
- Node.js compatível com a versão instalada do Vite (para o Vite usado na orientação: 20.19+ na linha 20 ou 22.12+ na linha 22).
- SQL Server acessível e banco dedicado já criado.
- Usuário com permissão para criar as tabelas e executar as operações de cadastro e consulta.
- Visual Studio 2022 com desenvolvimento ASP.NET e Web, ou ferramentas de linha de comando .NET.

## Configuração do backend

Abra a solução em `backend/` no Visual Studio. Clique com o botão direito no projeto `cvhub` e selecione **Gerenciar Segredos do Usuário**. Configure, substituindo os exemplos localmente:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=SEU_BANCO;User Id=SEU_USUARIO;Password=SUA_SENHA;Encrypt=True;TrustServerCertificate=False;"
  }
}
```

Não inclua credenciais reais no repositório. User Secrets é um mecanismo de configuração para desenvolvimento; não é um cofre criptografado nem a configuração de produção.

No desenvolvimento, se o servidor apresentar certificado não confiável, `TrustServerCertificate=True` mantém a criptografia e dispensa a validação do certificado. Em produção, use certificado confiável com essa opção em `False`.

### Criar a estrutura do banco

No Console do Gerenciador de Pacotes do Visual Studio, selecione `cvhub` como projeto padrão e mantenha-o como projeto de inicialização:

```powershell
Update-Database
```

As migrations incluídas criam `Candidatos` e `__EFMigrationsHistory`. Não é necessário gerar novamente a migration inicial. Use um banco exclusivo para a avaliação.

### Executar a API

Confie no certificado local, se necessário:

```powershell
dotnet dev-certs https --trust
```

Selecione o perfil `https` e pressione F5. No ambiente usado no desenvolvimento:

- API: `https://localhost:7189`
- Swagger: `https://localhost:7189/swagger/index.html`

Confirme a porta em `Properties/launchSettings.json` e ajuste o frontend se ela for diferente. O perfil deve usar o ambiente `Development` para carregar os segredos locais e disponibilizar o Swagger.

Alternativa por terminal, dentro da pasta que contém `cvhub.csproj`:

```powershell
dotnet restore
dotnet run --launch-profile https
```

## Configuração do frontend

Na pasta `frontend`, crie `.env.development`:

```env
VITE_API_URL=https://localhost:7189
```

Essa variável contém o endereço da API, nunca credenciais do banco. Instale e execute:

```powershell
npm ci
npm run dev
```

Abra `http://localhost:5173`. Mantenha a API executando. O backend permite essa origem por CORS. Se mudar a origem, ajuste a política `Frontend` no `Program.cs`. Reinicie o Vite após alterar variáveis de ambiente.

Para verificar a geração dos arquivos de publicação:

```powershell
npm run build
```

Os arquivos são gerados em `frontend/dist`. O servidor Vite de desenvolvimento não é a hospedagem de produção.

## Endpoints

| Método | Caminho | Finalidade |
| --- | --- | --- |
| POST | `/api/candidatos` | Validar e salvar candidato |
| GET | `/api/candidatos` | Listar candidatos |
| GET | `/api/candidatos/{id}` | Consultar detalhes |
| POST | `/api/curriculos/extrair` | Receber PDF e sugerir dados |

Exemplo de cadastro:

```json
{
  "nomeCompleto": "Mariana Souza",
  "email": "mariana@example.com",
  "telefone": "(41) 99999-0000",
  "areaInteresse": "Desenvolvimento de Sistemas",
  "resumoProfissional": "Candidata fictícia para testar o CVHub."
}
```

A importação usa `multipart/form-data`, com o campo `arquivo`. Ela não salva candidato nem armazena o PDF. O cadastro ocorre somente ao enviar o formulário para `POST /api/candidatos`.

## Verificação manual

O projeto possui testes automatizados em backend/cvhub/cvhub.Tests, escritos com xUnit.

Para executar a suíte, a partir da pasta do backend:

```bash
dotnet test
```

Resultado da última execução: **13 testes aprovados, 0 falhas, 0 ignorados**.

Cobertura atual: validação do cadastro e extração de e-mail e telefone do PDF.

Foi realizada também uma validação manual pelo Swagger e pela interface. Execute os cenários abaixo em um banco de avaliação, usando dados fictícios:

| Cenário | Resultado esperado |
| --- | --- |
| Cadastro somente com nome e e-mail | 201; candidato persiste e aparece na lista |
| Nome/e-mail ausente ou nome apenas com espaços | 400; nenhum registro criado |
| E-mail inválido | Cadastro impedido |
| PDF fictício com texto | Sugestões preenchidas; nenhum registro criado antes de salvar |
| Corrigir sugestão e salvar | Valor corrigido persiste |
| Importar após preencher nome | Nome já preenchido preservado |
| Arquivo não PDF, inclusive renomeado para `.pdf` | 400; cadastro manual permanece disponível |
| PDF maior que 5 MB | Rejeitado; cadastro manual permanece disponível |
| PDF sem texto extraível ou corrompido | 422; cadastro manual permanece disponível |
| Consultar candidato salvo | 200; detalhes correspondem ao ID |
| Consultar ID inexistente | 404 |
| API indisponível | Mensagem de falha de conexão na interface |

O limite do arquivo é de 5 × 1024 × 1024 bytes. A requisição tem limite de 6 MB para comportar o multipart; arquivos que ultrapassem esse limite podem receber 413 antes de chegar ao controller.

## Limitações e melhorias

- Sem OCR: PDFs digitalizados como imagem podem não fornecer texto extraível.
- Nome identificado por regras simples: rótulo `Nome:`/`Nome completo:` ou linha plausível no início do documento; pode haver falso positivo.
- Colunas, fontes e ordem interna do PDF podem prejudicar a extração.
- Extração de telefone prioriza formatos brasileiros com DDD e retorna a primeira correspondência.
- O formulário preserva dados já preenchidos ao importar; todas as sugestões exigem revisão humana.
- Não há edição, exclusão, autenticação, paginação ou regra de unicidade do e-mail; não eram requisitos obrigatórios.
- Melhorias: testes automatizados, paginação, separação da extração em serviço testável e OCR.

## Uso de IA

Veja `DESENVOLVIMENTO.md`. A IA auxiliou no planejamento, nos exemplos de implementação e na orientação de configuração; a documentação deve ser revisada para refletir os arquivos e verificações efetivamente realizados.
