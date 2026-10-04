# Registro de desenvolvimento — cvhub

## Organização e execução

O trabalho foi dividido em etapas: criação da API, integração com SQL Server, cadastro e consultas, importação de PDF, tratamento de erros e CORS, criação do frontend React, formulário único e consulta de detalhes. A documentação foi preparada após a implementação dos fluxos principais.

Usei Visual Studio 2022 no backend, VS Code no frontend e SQL Server Management Studio para o banco. Mantive frontend e backend em pastas separadas do mesmo projeto.

Priorizei o cadastro manual integrado ao banco antes da extração do PDF, para garantir que uma falha de importação não bloqueasse a funcionalidade principal.

## Decisões técnicas

- **C# e ASP.NET Core:** aproveitamento da minha experiência com C# e desenvolvimento de sistemas.
- **React com JavaScript e Vite:** interface com componentes e ambiente de desenvolvimento simples.
- **SQL Server e Entity Framework Core:** persistência e versionamento da estrutura por migrations.
- **Validação no backend e frontend:** o backend mantém as regras obrigatórias; a interface fornece retorno imediato e exibe os erros da API.
- **Importação separada do cadastro:** o endpoint lê o PDF e retorna sugestões. O formulário permite revisar e completar os dados antes de salvar.
- **PdfPig e regras textuais:** extração sem serviço externo de IA. E-mail e telefone são localizados com expressões regulares; o nome utiliza heurísticas.
- **PDF não armazenado:** o documento é usado temporariamente em memória para leitura; guardar o arquivo não era requisito.
- **User Secrets:** configuração local da conexão fora do repositório, com exemplos sem credenciais na documentação.
- **CORS com origem explícita:** liberação de `http://localhost:5173` para a interface de desenvolvimento.
- **Mensagens de erro:** erros previstos recebem mensagens específicas; falhas inesperadas recebem mensagem geral.

## Participação da IA

Utilizei ChatGPT/Codex como apoio. Modelo: GPT-6.1 Sol Leve.

A IA ajudou a organizar o escopo, sugerir melhorias códigos e explicar as etapas de configuração. Executei as instruções no meu notebook e acompanhei os resultados, trazendo erros para investigação.

Exemplos de pedidos e aproveitamento:

| Pedido | Como a resposta foi aproveitada |
| “Como fazer a leitura do texto de um arquivo PDF.” | Orientação de pacote a ser utilizado e codificação do processo |
| Relato de erro de cadeia de certificação do SQL Server | Ajuste local de confiança no certificado, mantendo a criptografia |
| Criação do código HTML e CSS | Criação das páginas e das folhas de estilo |
| Pedido de documentação | Rascunhos de README e relato, sujeitos à minha revisão |


A aplicação não utiliza IA em tempo de execução para analisar currículos.

## Correções e adaptações
- Mantive a estrutura local em `C:\inetpub\wwwroot\cvhub`, com pastas de backend e frontend.
- Ajustei a conexão de desenvolvimento para o certificado apresentado pelo SQL Server. A configuração de produção deve validar um certificado confiável.
- Configurei a confiança no certificado HTTPS de `localhost` e confirmei o funcionamento no navegador.


## Verificação

Foi confirmado durante o desenvolvimento o funcionamento da criação das tabelas, dos endpoints de cadastro/consulta, da extração de PDF, da execução do frontend e da listagem integrada à API.

Foram implementados testes automatizados utiizando um projeto do tipo xUnit Test Project, executados com sucesso: 13 testes aprovados, 0 falhas. Cobrem validação do cadastro e extração de e-mail/telefone do PDF. Os demais cenários, principalmente os de interface, continuam no roteiro manual do README.

O README contém o roteiro manual dos demais cenários.

## Tempo dedicado

16 horas ou 3 dias em horários alternados.

## Dificuldades e limitações

As dificuldades observadas incluíram configuração de certificados, adaptação dos namespaces e entendimento das ferramentas do frontend.

A extração não usa OCR e pode falhar em PDFs digitalizados, documentos protegidos ou layouts complexos. A identificação de nome pode gerar sugestões incorretas; os dados são sempre revisáveis. A importação não bloqueia o cadastro manual após uma falha.

## Melhorias e pendências

- Ampliar os testes automatizados: hoje há 13 testes de [o que cobrem]; faltam testes de interface (frontend) e de PDFs com layouts diferentes.
- Separar as regras de extração em serviço e testar diferentes layouts.
- Considerar DTOs para separar entrada e persistência conforme o projeto evoluir.
- Implementar paginação e melhorar a acessibilidade com verificação específica.
- Avaliar OCR e limites adicionais de processamento de PDFs.
