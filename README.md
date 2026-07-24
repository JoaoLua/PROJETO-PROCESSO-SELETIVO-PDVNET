# 💰 PDVnet — Sistema de Controle de Caixa

Sistema Desktop de Controle de Caixa desenvolvido em **C# / WPF** com banco de dados **SQL Server**, seguindo a arquitetura **MVVM em camadas**.

O sistema permite o lançamento, consulta, edição e exclusão de movimentações financeiras de um estabelecimento, com dashboard de resumo e alertas configuráveis de saldo baixo.

---

## 📸 Screenshots da Aplicação

> **Instruções para você:** tire os prints abaixo e salve as imagens na pasta `screenshots/` na raiz do projeto. Depois, descomente as linhas de imagem correspondentes.

<!-- 
### Tela Principal — Listagem de Movimentações
![Tela de Movimentações](screenshots/tela_movimentacoes.png)

### Formulário de Novo Lançamento
![Novo Lançamento](screenshots/formulario_novo.png)

### Dashboard com Resumo Financeiro
![Dashboard](screenshots/dashboard.png)

### Alerta de Saldo Baixo
![Alerta](screenshots/alerta_saldo.png)
-->

**Prints sugeridos para tirar:**
1. **Tela de Movimentações** — Com alguns lançamentos cadastrados na tabela
2. **Formulário** — Modal de "Novo Lançamento" aberto
3. **Dashboard** — Com os cards de Saldo, Entradas, Saídas e Total
4. **Alerta** — Banner amarelo de saldo baixo visível no cabeçalho

> Depois de tirar os prints, crie a pasta `screenshots/` na raiz do projeto, salve as imagens com os nomes acima e descomente o bloco de imagens.

---

## 🚀 Tecnologias Utilizadas

| Tecnologia | Finalidade |
|:---|:---|
| **C# / .NET 10** | Linguagem principal da aplicação |
| **WPF (Windows Presentation Foundation)** | Interface gráfica Desktop |
| **SQL Server (LocalDB)** | Banco de dados relacional |
| **ADO.NET (SqlConnection / SqlCommand)** | Acesso direto ao banco via SQL puro |
| **Material Design in XAML Toolkit** | Componentes visuais modernos |
| **MSTest** | Framework de testes unitários |

---

## 📁 Estrutura do Projeto (Camadas)

O projeto foi organizado em **5 camadas** independentes para garantir separação de responsabilidades, manutenibilidade e testabilidade:

```
PDVnet.ControleCaixa/
│
├── 📂 PDVnet.ControleCaixa.UI/          ← Camada de Apresentação (WPF)
│   ├── Views/                           ← Telas XAML (MainWindow, Movimentações, Dashboard, Formulário)
│   ├── ViewModels/                      ← Lógica de apresentação (MVVM)
│   ├── Assets/                          ← Logo e recursos visuais
│   ├── App.xaml                         ← Configuração de tema e recursos globais
│   └── App.config                       ← String de conexão com o banco de dados
│
├── 📂 PDVnet.ControleCaixa.Business/    ← Camada de Regras de Negócio
│   ├── Services/
│   │   └── MovimentacaoService.cs       ← Orquestra operações e valida regras
│   └── Validators/
│       └── MovimentacaoValidator.cs     ← Validação de campos obrigatórios e restrições
│
├── 📂 PDVnet.ControleCaixa.Data/        ← Camada de Acesso a Dados
│   ├── MovimentacaoRepository.cs        ← Queries SQL (INSERT, SELECT, UPDATE)
│   ├── ConnectionHelper.cs              ← Gerencia a conexão com o SQL Server
│   └── Scripts/
│       └── CreateDatabase.sql           ← Script de criação do banco e tabela
│
├── 📂 PDVnet.ControleCaixa.Model/       ← Camada de Modelos (transversal)
│   ├── MovimentacaoCaixa.cs             ← Entidade principal
│   ├── Enums/TipoMovimentacao.cs        ← Enum: Entrada (1) ou Saída (2)
│   ├── DTOs/DashboardDTO.cs             ← Objeto de transferência do Dashboard
│   └── Interfaces/                      ← Contratos (IMovimentacaoRepository, IMovimentacaoService)
│
├── 📂 PDVnet.ControleCaixa.Tests/       ← Camada de Testes Unitários
│   ├── Fakes/FakeMovimentacaoRepository.cs  ← Repositório simulado em memória
│   ├── Services/MovimentacaoServiceTests.cs ← Testes do serviço (saldo, alertas, filtros)
│   └── Validators/MovimentacaoValidatorTests.cs ← Testes do validador (descrição, valor)
│
└── README.md                            ← Este arquivo
```

---

## 🛠️ Pré-requisitos

Antes de rodar o projeto, certifique-se de ter instalado:

- [x] **Visual Studio 2022** (ou superior) com a carga de trabalho **"Desenvolvimento para Desktop com .NET"**
- [x] **SQL Server LocalDB** (já vem instalado com o Visual Studio por padrão)
- [x] **.NET 10 SDK** (ou a versão correspondente ao `TargetFramework` do projeto)

> **💡 Dica:** Para verificar se o LocalDB está instalado, abra o **Prompt de Comando** e digite:
> ```
> sqllocaldb info
> ```
> Deve aparecer `MSSQLLocalDB` na lista.

---

## ⚙️ Como Rodar o Projeto (Passo a Passo)

### Passo 1️⃣ — Clonar o Repositório

Abra o terminal (Git Bash, CMD ou PowerShell) e execute:

```bash
git clone https://github.com/JoaoLua/projeto-teste.git
```

> 📸 **Print sugerido:** Terminal mostrando o clone concluído com sucesso.

---

### Passo 2️⃣ — Criar o Banco de Dados

1. Abra o **Visual Studio**
2. Vá em **Exibir → Pesquisador de Objetos do SQL Server** (ou `Ctrl+\, Ctrl+S`)
3. Expanda **SQL Server → (localdb)\MSSQLLocalDB**
4. Clique com o botão direito em **Bancos de Dados → Nova Consulta...**
5. Copie e cole o conteúdo do arquivo abaixo:

📄 **Arquivo:** `PDVnet.ControleCaixa.Data/Scripts/CreateDatabase.sql`

```sql
IF DB_ID('PDVnetControleCaixa') IS NULL
    CREATE DATABASE PDVnetControleCaixa;
GO

USE PDVnetControleCaixa;
GO

IF OBJECT_ID('dbo.MovimentacaoCaixa', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MovimentacaoCaixa (
        Id            INT           IDENTITY(1,1) PRIMARY KEY,
        Descricao     VARCHAR(200)                NOT NULL,
        Tipo          INT                         NOT NULL CHECK (Tipo IN (1, 2)),
        Categoria     VARCHAR(100)                NULL,
        Valor         DECIMAL(10,2)               NOT NULL CHECK (Valor > 0),
        DataMovimento DATETIME                    NOT NULL DEFAULT GETDATE(),
        Status        BIT                         NOT NULL DEFAULT 1               
    );
END
GO
```

6. Clique em **Executar** (ou pressione `Ctrl+Shift+E`)

> 📸 **Print sugerido:** SQL Server Object Explorer mostrando o banco `PDVnetControleCaixa` criado com a tabela `MovimentacaoCaixa` expandida.

---

### Passo 3️⃣ — Verificar a String de Conexão

O projeto já vem configurado para usar o **LocalDB** do Visual Studio. A configuração está no arquivo:

📄 **Arquivo:** `PDVnet.ControleCaixa.UI/App.config`

```xml
<connectionStrings>
    <add name="PDVnetControleCaixa" 
         connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PDVnetControleCaixa;Integrated Security=True;TrustServerCertificate=True" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

> **⚠️ Atenção:** Se o seu SQL Server usar uma instância diferente (ex: `localhost` ou `.\SQLEXPRESS`), altere o campo `Data Source` para a instância correta.

---

### Passo 4️⃣ — Abrir e Compilar a Solução

1. Abra o arquivo **`PDVnet.ControleCaixa.slnx`** no Visual Studio (duplo clique)
2. No **Gerenciador de Soluções**, verifique se todos os 5 projetos estão carregados:
   - `PDVnet.ControleCaixa.UI`
   - `PDVnet.ControleCaixa.Business`
   - `PDVnet.ControleCaixa.Data`
   - `PDVnet.ControleCaixa.Model`
   - `PDVnet.ControleCaixa.Tests`
3. Clique com o botão direito no projeto **`PDVnet.ControleCaixa.UI`** → **Definir como Projeto de Inicialização**
4. Pressione **Ctrl+Shift+B** para compilar

> 📸 **Print sugerido:** Gerenciador de Soluções com todos os projetos carregados e a mensagem "Build: 5 com êxito" na barra de status.

---

### Passo 5️⃣ — Executar a Aplicação

Pressione **F5** (ou clique no botão ▶️ verde) para iniciar.

A aplicação abrirá diretamente na **tela de Movimentações**, pronta para uso!

> 📸 **Print sugerido:** Aplicação rodando com a tela principal visível.

---

## 📝 Como Usar a Aplicação

### Cadastrar um Lançamento
1. Na tela de Movimentações, clique no botão **"+ Novo Lançamento"**
2. Preencha os campos: **Descrição**, **Valor**, **Tipo** (Entrada/Saída) e **Categoria**
3. Clique em **"Salvar"**

### Editar um Lançamento
1. Na tabela, clique no ícone de **lápis** (✏️) na linha desejada
2. Altere os campos necessários
3. Clique em **"Salvar"**

### Excluir um Lançamento
1. Na tabela, clique no ícone de **lixeira** (🗑️) na linha desejada
2. Confirme a exclusão na mensagem de confirmação

### Filtrar Movimentações
- **Por descrição:** Digite parte do texto no campo "Buscar por descrição..."
- **Por período:** Selecione a "Data Inicial" e/ou "Data Final" nos campos de data
- Os filtros são aplicados automaticamente em tempo real

### Ver o Dashboard
1. No **menu lateral esquerdo**, clique em **"Dashboard"**
2. Visualize os cards com Saldo Total, Entradas, Saídas e Total de Lançamentos
3. Para configurar o **alerta de saldo baixo**, edite o campo no topo do Dashboard

---

## 🧪 Executando os Testes Unitários

O projeto inclui **9 testes unitários** cobrindo as regras de negócio da camada Business:

### Para rodar os testes:

1. No Visual Studio, vá em **Teste → Gerenciador de Testes** (ou `Ctrl+E, T`)
2. Clique em **"Executar Todos os Testes"** (▶️▶️)
3. Todos os 9 testes devem aparecer com ✅ verde

### Testes implementados:

| Classe | Teste | O que valida |
|:---|:---|:---|
| **ValidatorTests** | `Validar_DescricaoNula_DeveRetornarErro` | Descrição nula é rejeitada |
| **ValidatorTests** | `Validar_DescricaoVazia_DeveRetornarErro` | Descrição em branco é rejeitada |
| **ValidatorTests** | `Validar_ValorZero_DeveRetornarErro` | Valor zero é rejeitado |
| **ValidatorTests** | `Validar_ValorNegativo_DeveRetornarErro` | Valor negativo é rejeitado |
| **ValidatorTests** | `Validar_MovimentacaoValida_NaoDeveRetornarErros` | Dados corretos passam sem erro |
| **ServiceTests** | `ObterResumoDashboard_DeveCalcularSaldoCorretamente` | Saldo = Entradas − Saídas |
| **ServiceTests** | `VerificarAlertaSaldoBaixo_DeveRetornarTrue_QuandoAbaixoDoMinimo` | Alerta dispara quando saldo < limite |
| **ServiceTests** | `VerificarAlertaSaldoBaixo_DeveRetornarFalse_QuandoAcimaDoMinimo` | Alerta NÃO dispara quando saldo > limite |
| **ServiceTests** | `ListarPorFiltros_DataInicioMaiorQueDataFim_DeveLancarExcecao` | Filtro com datas invertidas é rejeitado |

> 📸 **Print sugerido:** Test Explorer mostrando todos os 9 testes passando (✅ verde).

---

## 🗄️ Modelagem do Banco de Dados

### Tabela: `MovimentacaoCaixa`

| Coluna | Tipo | PK | NOT NULL | Default | Descrição |
|:---|:---|:---:|:---:|:---:|:---|
| `Id` | `INT IDENTITY` | ✅ | ✅ | Auto | Identificador único |
| `Descricao` | `VARCHAR(200)` | — | ✅ | — | Descrição da movimentação |
| `Tipo` | `INT` | — | ✅ | — | 1 = Entrada, 2 = Saída |
| `Categoria` | `VARCHAR(100)` | — | — | `NULL` | Categoria (Vendas, Alimentação, etc.) |
| `Valor` | `DECIMAL(10,2)` | — | ✅ | — | Valor (sempre positivo) |
| `DataMovimento` | `DATETIME` | — | ✅ | `GETDATE()` | Data/hora gerada automaticamente |
| `Status` | `BIT` | — | ✅ | `1` | 1 = Ativo, 0 = Inativo (exclusão lógica) |

### Constraints de integridade:
- `CHECK (Tipo IN (1, 2))` — Garante que só aceita Entrada ou Saída
- `CHECK (Valor > 0)` — Garante que o valor é sempre positivo

---

## 📐 Padrão Arquitetural: MVVM

O projeto segue o padrão **Model-View-ViewModel (MVVM)** nativo do WPF:

```
┌─────────────────┐     Data Binding      ┌──────────────────┐     Chamadas      ┌─────────────────┐
│                 │ ◄──────────────────► │                  │ ──────────────► │                 │
│    VIEW         │     (XAML Bindings)   │   VIEW MODEL     │                  │    SERVICE      │
│  (XAML / WPF)   │                      │  (C# / Commands) │ ◄────────────── │  (Regras)       │
│                 │                      │                  │     Resultado    │                 │
└─────────────────┘                      └──────────────────┘                  └────────┬────────┘
                                                                                       │
                                                                                       ▼
                                                                              ┌─────────────────┐
                                                                              │   REPOSITORY    │
                                                                              │  (SQL / ADO.NET)│
                                                                              └─────────────────┘
```

- **View** → Apenas exibição (XAML). Não contém lógica de negócio.
- **ViewModel** → Expõe dados e comandos para a View via `INotifyPropertyChanged`.
- **Service** → Centraliza regras de negócio e validações.
- **Repository** → Executa queries SQL diretamente no banco via ADO.NET.

---

## 🎨 Paleta de Cores

| Cor | Hex | Uso |
|:---|:---|:---|
| 🟦 Azul Escuro | `#021651` | Cor primária (menu lateral, botões, títulos) |
| 🟡 Dourado | `#FFD700` | Cor secundária (ícone de destaque, banner de alerta) |
| 🔴 Vermelho | `#DC143C` | Erros, exclusão, saídas financeiras |
| ⬜ Branco | `#FFFFFF` | Fundo das áreas de conteúdo |
| 🔘 Cinza Claro | `#E5E2E1` | Fundo geral da aplicação |
| ⬛ Cinza Escuro | `#333333` | Textos e rótulos |

---

## 👨‍💻 Autor

**João** — Candidato ao processo seletivo de Desenvolvedor Júnior na PDVnet.
