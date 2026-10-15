# Feature Specification: Task Creation & Query Management (Cadastro e Consulta de Tarefas)

**Feature Branch**: `003-task-management`

**Created**: 2026-10-15

**Status**: Draft

**Input**: User description: "Agora vamos implementar a funcionalidade de cadastro e consulta de tarefas. Com ainda não estamos a especificar a funcionalidade de cadastro e login, vamos focar em usar definir um userid imaginário para teste de cadastro e consulta. As tarefas devem ter incluidas as propriedades 'nome, descriçãos, estado, datas (início e termino(este pode ser null) )'. No final deve ser possível cadastrar uma tarefa, e consultar tarefas por id, por nome e descrição, filtrar por estado e por intervalos de datas de criação e termino."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Cadastrar Nova Tarefa (Priority: P1) 🎯 MVP

Como um usuário gerenciando suas atividades, quero cadastrar uma nova tarefa informando nome, descrição, data de início e opcionalmente uma data de término, para que a atividade fique registrada e organizada no sistema.

**Why this priority**: O cadastro de tarefas é a capacidade primária e indispensável do domínio TaskFy; sem tarefas registradas, nenhuma operação de visualização, filtragem ou acompanhamento tem valor.

**Independent Test**: Enviar uma requisição de cadastro com nome válido, descrição, data de início e data de término opcional sob um contexto de usuário fixo/imaginário. O sistema persiste a tarefa com status inicial `Pending`, gera um identificador único e retorna a representação criada com código HTTP 201 Created.

**Acceptance Scenarios**:

1. **Given** um usuário fornecendo nome, descrição, data de início e data de término válida, **When** solicita o cadastro da tarefa, **Then** o sistema persiste a tarefa com status `Pending`, armazena a data de criação atual e retorna os dados completos com HTTP 201 Created.
2. **Given** um usuário fornecendo nome, descrição e data de início sem data de término, **When** solicita o cadastro, **Then** o sistema persiste a tarefa com a data de término nula e retorna sucesso HTTP 201 Created.
3. **Given** dados de entrada com nome ausente ou vazio, **When** solicita o cadastro, **Then** o sistema rejeita a operação com HTTP 400 Bad Request detalhado em RFC 7807 Problem Details sem persistir nenhum registro.
4. **Given** uma data de término informada anterior à data de início, **When** solicita o cadastro, **Then** o sistema rejeita a operação com erro de validação RFC 7807.

---

### User Story 2 - Consultar Detalhes da Tarefa por ID (Priority: P2)

Como um usuário, quero consultar os detalhes completos de uma tarefa específica informando seu identificador único, para conferir seu estado, prazos e descrição.

**Why this priority**: A visualização individual é essencial para confirmar o cadastro, inspecionar os detalhes de uma atividade e suportar navegações direcionadas da interface.

**Independent Test**: Consultar uma tarefa existente pelo seu ID e receber HTTP 200 OK com payload contendo todas as propriedades; consultar um ID inexistente e receber HTTP 404 Not Found no formato RFC 7807 Problem Details.

**Acceptance Scenarios**:

1. **Given** uma tarefa cadastrada no sistema associada ao usuário, **When** o usuário consulta pelo seu ID, **Then** o sistema retorna os dados completos da tarefa com HTTP 200 OK.
2. **Given** uma solicitação de consulta com um ID inexistente no banco de dados, **When** o usuário executa a busca, **Then** o sistema retorna HTTP 404 Not Found com Problem Details indicando que a tarefa não foi encontrada.

---

### User Story 3 - Listar e Filtrar Tarefas com Busca e Intervalos de Datas (Priority: P3)

Como um usuário com múltiplas tarefas, quero listar minhas tarefas aplicando filtros por nome, descrição, estado e intervalos de datas (criação e término), para localizar rapidamente as atividades que preciso acompanhar.

**Why this priority**: Fornece flexibilidade analítica e produtividade no dia a dia, permitindo ao usuário encontrar tarefas por termos textuais e organizar sua agenda por datas e status.

**Independent Test**: Cadastrar múltiplas tarefas com diferentes nomes, estados e datas; aplicar requisições de listagem com parâmetros de filtro combinados e verificar que apenas as tarefas correspondentes são retornadas em conformidade com os critérios.

**Acceptance Scenarios**:

1. **Given** tarefas cadastradas, **When** o usuário busca por um termo presente no nome ou na descrição, **Then** o sistema retorna apenas as tarefas cujo nome ou descrição contenham o termo pesquisado.
2. **Given** tarefas em diferentes estados (ex.: `Pending`, `InProgress`, `Completed`), **When** o usuário filtra por um estado específico, **Then** o sistema retorna apenas tarefas no estado solicitado.
3. **Given** tarefas com diferentes datas de criação, **When** o usuário filtra por um intervalo de data de criação (`CreatedFrom` e `CreatedTo`), **Then** o sistema retorna apenas as tarefas criadas dentro desse intervalo.
4. **Given** tarefas com diferentes datas de término, **When** o usuário filtra por um intervalo de data de término (`DueFrom` e `DueTo`), **Then** o sistema retorna apenas as tarefas cujo término esteja dentro do intervalo informado.
5. **Given** nenhum filtro especificado, **When** o usuário solicita a listagem, **Then** o sistema retorna todas as tarefas ativas do usuário ordenadas de forma previsível (ex.: mais recentes primeiro).
6. **Given** filtros que não correspondam a nenhuma tarefa existente, **When** a consulta é executada, **Then** o sistema retorna HTTP 200 OK com uma lista vazia, sem erros.

---

### Edge Cases

- **Término Anterior ao Início**: Se o usuário fornecer `EndDate` anterior a `StartDate`, a criação deve ser rejeitada com código 400 Validation Problem Details.
- **Intervalo de Filtro Invertido**: Se o usuário consultar com `CreatedFrom > CreatedTo` ou `DueFrom > DueTo`, a consulta deve tratar de forma consistente (rejeitar com validação 400 ou normalizar).
- **Busca com Caracteres Especiais ou Vazia**: Espaços em branco extras nos termos de busca devem ser ignorados/normalizados.
- **Isolamento de Usuário Simulado**: Operações de consulta e cadastro utilizam um identificador fixo de desenvolvimento/teste (`UserId`), preparando o domínio para isolamento multitenant/multi-user quando a autenticação for adicionada.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema DEVE permitir o cadastro de tarefas contendo `Name` (nome/título), `Description` (descrição), `StartDate` (data de início) e `EndDate` opcional (data de término).
- **FR-002**: Toda tarefa DEVE ser associada a um `UserId`, sendo utilizado um identificador de usuário imaginário/predefinido nesta fase enquanto a autenticação não estiver especificada.
- **FR-003**: O campo `Name` DEVE ser obrigatório, não nulo e conter entre 3 e 150 caracteres.
- **FR-004**: O campo `StartDate` DEVE ser obrigatório e representar uma data válida.
- **FR-005**: O campo `EndDate` DEVE ser opcional; quando fornecido, NÃO PODE ser anterior a `StartDate`.
- **FR-006**: Ao ser cadastrada, a tarefa DEVE receber automaticamente o estado inicial `Pending` e a data/hora de criação `CreatedAt`.
- **FR-007**: O sistema DEVE disponibilizar consulta de tarefa individual por seu identificador único (`Id`).
- **FR-008**: O sistema DEVE retornar resposta RFC 7807 Problem Details com status 404 Not Found caso o `Id` solicitado não exista.
- **FR-009**: O sistema DEVE permitir a listagem de tarefas com suporte a busca textual por correspondência parcial (case-insensitive) nos campos `Name` e `Description`.
- **FR-010**: O sistema DEVE permitir filtrar tarefas por seu estado (`Status`).
- **FR-011**: O sistema DEVE permitir filtrar tarefas por intervalo de data de criação (`CreatedFrom` e `CreatedTo`).
- **FR-012**: O sistema DEVE permitir filtrar tarefas por intervalo de data de término (`DueFrom` e `DueTo`).
- **FR-013**: Todas as consultas de tarefas DEVEM ser restritas ao `UserId` do contexto da requisição.

### Key Entities

- **Task**: Entidade raiz do agregado de tarefas, contendo:
  - `Id`: Identificador único (UUID/Guid).
  - `UserId`: Identificador do usuário proprietário (UUID/Guid).
  - `Name`: Nome/título da atividade (string, obrigatório).
  - `Description`: Detalhamento textual da tarefa (string, opcional).
  - `Status`: Estado da atividade (`TaskStatus`).
  - `StartDate`: Data e hora de início (DateTimeOffset).
  - `EndDate`: Data e hora de término prevista ou realizada (DateTimeOffset, anulável).
  - `CreatedAt`: Timestamp de registro (DateTimeOffset).
  - `UpdatedAt`: Timestamp da última modificação (DateTimeOffset, anulável).
- **TaskStatus**: Enumeração que expressa o ciclo de vida da tarefa:
  - `Pending` (Pendente - valor padrão na criação).
  - `InProgress` (Em Andamento).
  - `Completed` (Concluída).
  - `Cancelled` (Cancelada).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: O cadastro de uma tarefa é confirmado com ID gerado em menos de 200ms em ambiente de desenvolvimento.
- **SC-002**: 100% das requisições com dados inválidos (nome ausente, datas inconsistentes) são rejeitadas com formato RFC 7807 Problem Details sem vazamento de exceções.
- **SC-003**: 100% das consultas por tarefas existentes retornam apenas registros associados ao usuário em menos de 100ms.
- **SC-004**: Filtros textuais, por estado e por intervalos de datas retornam 100% dos registros que atendem aos predicados e 0% de falsos positivos.
- **SC-005**: Todo novo endpoint da API (`POST /tasks`, `GET /tasks/{id}`, `GET /tasks`) possui ao menos um teste de integração de caminho feliz automatizado com 100% de sucesso.

## Assumptions

- Como a funcionalidade de autenticação e login ainda não está especificada, o sistema utilizará um identificador fixo de teste (`00000000-0000-0000-0000-000000000001` ou configurável via header/serviço) para representar o `UserId` imaginário do usuário logado.
- Os fusos horários são padronizados em UTC (`DateTimeOffset`) para evitar inconsistências em filtros de datas.
- A ordenação padrão na listagem sem filtros é decrescente por data de criação (`CreatedAt DESC`).
