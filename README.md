# TaskFy --- Estudo de Desenvolvimento Orientado por Especificações (SDD)

## 1. Sobre o projeto

O **TaskFy** é um projeto de aprendizagem criado para praticar o
desenvolvimento de software com **Spec-Driven Development (SDD)**,
utilizando o **GitHub Spec Kit** como apoio ao processo.

O objetivo principal não é apenas construir uma aplicação funcional de
gestão de tarefas, mas aprender a transformar uma ideia em requisitos
claros, especificações verificáveis, decisões técnicas, planos de
implementação e tarefas executáveis antes de escrever o código.

O projeto deve ser desenvolvido de forma incremental. Cada
funcionalidade será uma oportunidade para praticar o fluxo do Spec Kit,
validar decisões e melhorar a qualidade da documentação e da
implementação.

## 2. Objetivos de aprendizagem

-   Compreender os princípios do desenvolvimento orientado por
    especificações.
-   Aprender o papel de cada artefacto produzido pelo Spec Kit.
-   Escrever requisitos funcionais e não funcionais claros e
    verificáveis.
-   Identificar ambiguidades e resolvê-las antes da implementação.
-   Elaborar planos técnicos coerentes com as especificações aprovadas.
-   Decompor planos em tarefas pequenas, ordenadas e verificáveis.
-   Implementar funcionalidades de forma incremental, respeitando os
    requisitos.
-   Aplicar Test-Driven Development (TDD) quando adequado, seguindo o
    ciclo Red-Green-Refactor.
-   Utilizar Git com commits pequenos, atómicos e semanticamente
    descritos.
-   Rever a implementação para confirmar que corresponde à
    especificação.

## 3. O que será construído

O TaskFy será uma aplicação de gestão pessoal de tarefas. O âmbito
inicial é deliberadamente reduzido para permitir concentrar o estudo no
processo SDD.

### Funcionalidades iniciais

-   Criar uma tarefa com título obrigatório e descrição opcional.
-   Consultar a lista de tarefas.
-   Consultar os detalhes de uma tarefa.
-   Atualizar os dados de uma tarefa.
-   Marcar uma tarefa como concluída.
-   Validar os dados recebidos e apresentar erros compreensíveis.
-   Tratar tentativas de consultar ou alterar tarefas inexistentes.

### Fora do âmbito inicial

-   Autenticação e autorização.
-   Colaboração entre utilizadores.
-   Notificações.
-   Prioridades avançadas.
-   Gestão de projetos e equipas.

Estas funcionalidades não devem ser adicionadas ao âmbito inicial sem
uma nova decisão explícita e uma especificação própria.

## 4. Tecnologias e arquitetura

A implementação técnica deverá respeitar as decisões aprovadas nos
artefactos do projeto, especialmente na constituição
(`.specify/memory/constitution.md`) e no plano da funcionalidade.

A stack de referência definida para o projeto é:

### Backend

-   .NET 10 e ASP.NET Core Web API.
-   Clean Architecture, CQRS e Vertical Slice Architecture.
-   SQLite para persistência local, com migrações de esquema.
-   Repository Pattern e Unit of Work para operações de persistência.
-   Result Pattern ou Notification Pattern para representar falhas
    previsíveis.
-   Respostas de erro da API no formato RFC 7807 Problem Details.
-   xUnit e Moq para testes automatizados.

### Frontend

-   Angular 20.
-   Organização por funcionalidades (*Feature-First*).
-   Componentes standalone e carregamento diferido (*lazy loading*) das
    rotas.
-   TypeScript com tipagem estrita.
-   Testes automatizados para componentes e serviços.

### Regras técnicas

-   Respeitar a direção de dependências definida pela Clean
    Architecture.
-   Manter as regras de negócio independentes de frameworks e
    infraestrutura.
-   Colocar cada classe, interface, enumeração ou tipo principal num
    ficheiro próprio.
-   Não adicionar dependências externas sem autorização e justificação
    documentadas.
-   Não guardar segredos no controlo de versão.
-   Escrever testes de acordo com a estratégia de qualidade definida na
    constituição.

A stack acima é uma referência do projeto, não uma autorização para
introduzir qualquer biblioteca adicional. Em caso de conflito, consultar
a constituição e as especificações aprovadas.

## 5. Como trabalhar com o Spec Kit

O processo de desenvolvimento deve seguir, de forma geral, este ciclo:

1.  **Constituição (`constitution`)** --- estabelecer os princípios,
    restrições e regras de qualidade que governam o projeto.
2.  **Especificação (`specify`)** --- descrever o problema, os
    utilizadores, os resultados esperados e os requisitos da
    funcionalidade sem começar pela implementação.
3.  **Clarificação (`clarify`)** --- identificar ambiguidades, lacunas e
    decisões em aberto; esclarecer os pontos relevantes antes de
    avançar.
4.  **Planeamento (`plan`)** --- definir a abordagem técnica, a
    arquitetura afetada, os contratos e as decisões de implementação.
5.  **Tarefas (`tasks`)** --- dividir o plano em tarefas pequenas,
    ordenadas e verificáveis.
6.  **Implementação (`implement`)** --- executar as tarefas, respeitando
    a especificação, o plano e a constituição.
7.  **Verificação** --- executar testes e verificações de qualidade e
    confirmar que os critérios de aceitação foram satisfeitos.

Os nomes e comandos disponíveis podem variar conforme a versão do Spec
Kit e o agente utilizado. Consultar a ajuda da versão instalada em caso
de dúvida.

### Princípio fundamental

A especificação descreve **o que** deve ser alcançado e como o sucesso
será verificado. O plano explica **como** a solução será construída. As
tarefas organizam **o trabalho necessário**. A implementação deve
respeitar estes artefactos, e não substituí-los silenciosamente.

Se durante a implementação surgir uma necessidade que contradiga ou
ultrapasse os artefactos aprovados, interromper o trabalho nessa parte,
rever a especificação ou o plano e registar a decisão antes de
continuar.

## 6. Organização esperada do repositório

A estrutura exata deverá ser confirmada pelo plano de implementação e
pelas decisões do Spec Kit. A organização conceptual esperada é:

``` text
TaskFy/
├── .specify/
│   └── memory/
│       └── constitution.md
├── specs/
│   └── <id-e-nome-da-funcionalidade>/
│       ├── spec.md
│       ├── plan.md
│       └── tasks.md
├── src/
│   ├── backend/
│   └── frontend/
├── tests/
├── .gitignore
└── README.md
```

Esta árvore é ilustrativa. O Spec Kit pode criar ou utilizar outros
diretórios e ficheiros, incluindo templates e scripts. Não renomear nem
mover artefactos gerados pela ferramenta sem compreender o impacto no
fluxo de trabalho.

## 7. Regras de trabalho

### Antes de implementar

-   Confirmar que a especificação tem objetivos, requisitos e critérios
    de aceitação compreensíveis.
-   Resolver ambiguidades relevantes antes do planeamento.
-   Confirmar que o plano respeita a constituição.
-   Verificar se as tarefas cobrem os requisitos e os critérios de
    aceitação.
-   Identificar dependências entre tarefas e definir uma ordem lógica.

### Durante a implementação

-   Trabalhar numa tarefa de cada vez, sempre que possível.
-   Evitar alterações que não estejam relacionadas com a tarefa atual.
-   Escrever ou atualizar testes de acordo com a estratégia TDD do
    projeto.
-   Não contornar uma regra da constituição apenas para fazer os testes
    passar.
-   Registar decisões técnicas importantes nos artefactos apropriados.
-   Atualizar o estado das tarefas conforme o trabalho real realizado.

### Depois da implementação

-   Executar os testes relevantes e, antes de concluir a funcionalidade,
    a suite aplicável.
-   Confirmar os critérios de aceitação da especificação.
-   Rever erros, validações, contratos e efeitos na persistência.
-   Verificar se a documentação e as tarefas refletem o resultado final.
-   Criar commits atómicos, sem incluir ficheiros gerados, segredos ou
    alterações alheias.

## 8. Git e mensagens de commit

As mensagens de commit devem seguir **Conventional Commits**, ser
escritas em português e usar uma descrição curta no imperativo,
começando por minúscula após os dois-pontos e sem ponto final.

Formato:

``` text
<tipo>(<escopo opcional>): <descrição no imperativo>
```

Tipos autorizados na constituição do projeto:

-   `feat` --- nova funcionalidade.
-   `fix` --- correção de comportamento incorreto.
-   `docs` --- documentação e artefactos de especificação.
-   `style` --- formatação sem alteração de lógica.
-   `refactor` --- melhoria estrutural sem nova funcionalidade nem
    correção de bug.
-   `test` --- criação ou alteração de testes.
-   `chore` --- configuração, scripts, ferramentas ou dependências
    aprovadas.
-   `perf` --- melhoria de desempenho mensurável.

Exemplos:

``` text
docs(spec): clarificar critérios de aceitação das tarefas
test(api): adicionar teste para criação de tarefa
feat(tasks): permitir marcar uma tarefa como concluída
fix(api): devolver erro ao consultar tarefa inexistente
```

Cada commit deve representar uma unidade lógica, coerente e estável.
Evitar mensagens vagas como `ajustes`, `update` ou `wip`.

## 9. Critérios de conclusão de uma funcionalidade

Uma funcionalidade só deve ser considerada concluída quando:

-   Os requisitos e critérios de aceitação foram implementados.
-   Os testes relevantes passam.
-   Os erros previsíveis são tratados de forma consistente.
-   A implementação respeita a constituição e as decisões do plano.
-   Não foram introduzidas dependências não autorizadas.
-   A documentação e o estado das tarefas foram atualizados.
-   As alterações estão organizadas em commits atómicos e
    compreensíveis.

## 10. Como avaliar a aprendizagem

No final de cada funcionalidade, responder às seguintes perguntas:

1.  A especificação estava suficientemente clara antes de começar?
2.  Que ambiguidades foram identificadas e como foram resolvidas?
3.  O plano técnico cobriu todos os requisitos?
4.  As tarefas estavam suficientemente pequenas e verificáveis?
5.  A implementação respeitou os artefactos aprovados?
6.  Que testes demonstram que os critérios de aceitação foram cumpridos?
7.  Que decisões precisaram de ser revistas durante o trabalho?
8.  O que faria de forma diferente numa próxima funcionalidade?

O objetivo é aprender tanto com o resultado técnico como com o processo
utilizado para chegar até ele.

## 11. Documentos de referência

-   **Constituição do projeto:** `.specify/memory/constitution.md`
-   **Especificação da funcionalidade:** `spec.md`
-   **Plano técnico:** `plan.md`
-   **Lista de tarefas:** `tasks.md`
-   **Documentação do GitHub Spec Kit:**
    https://github.com/github/spec-kit

Os ficheiros de especificação, plano e tarefas devem ser consultados
dentro da pasta da funcionalidade correspondente.

## 12. Princípio orientador

**Compreender antes de especificar, esclarecer antes de planear, planear
antes de implementar e verificar antes de concluir.**

O TaskFy deve evoluir de forma incremental, com decisões explícitas,
requisitos verificáveis e uma ligação clara entre aquilo que foi pedido,
aquilo que foi planeado e aquilo que foi efetivamente entregue.
