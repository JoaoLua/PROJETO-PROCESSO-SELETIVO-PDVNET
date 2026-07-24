# PDVnet - Sistema de Controle de Caixa

Sistema Desktop de Controle de Caixa desenvolvido em **C# / WPF** com banco de dados **SQL Server**, seguindo a arquitetura **MVVM em camadas**.

O sistema permite o lançamento, consulta, edição e exclusão de movimentações financeiras de um estabelecimento, com dashboard de resumo e alertas configuráveis de saldo baixo.

---

## Screenshots da Aplicação

<img width="1366" height="728" alt="movimentações" src="https://github.com/user-attachments/assets/ddc8897e-316c-4cbb-87d0-e03b06f46dc5" />

<img width="417" height="345" alt="formulario" src="https://github.com/user-attachments/assets/512e8c69-9d13-4a6d-9fd4-93f33c35b1da" />

<img width="1366" height="726" alt="dashboard" src="https://github.com/user-attachments/assets/2d82e0c5-582e-44c6-a062-5f437d9bf858" />

---

## Tecnologias Utilizadas

| Tecnologia | Finalidade |
|:---|:---|
| **C# / .NET 10** | Linguagem principal da aplicação |
| **WPF** | Interface gráfica Desktop |
| **SQL Server** | Banco de dados relacional |
| **ADO.NET (SqlConnection / SqlCommand)** | Acesso direto ao banco via SQL |
| **Material Design in XAML Toolkit** | Componentes visuais modernos |
| **MSTest** | Framework de testes unitários |

---

## Padrão Arquitetural: MVVM

O projeto segue o padrão **Model-View-ViewModel (MVVM)** nativo do WPF:

- **View** → Apenas exibição (XAML). Não contém lógica de negócio.
- **ViewModel** → Expõe dados e comandos para a View via `INotifyPropertyChanged`.
- **Service** → Centraliza regras de negócio e validações.
- **Repository** → Executa queries SQL diretamente no banco via ADO.NET.

---

## Estrutura do Projeto

O projeto foi organizado em 5 camadas independentes para garantir separação de responsabilidades, manutenibilidade e testabilidade:

```
PDVnet.ControleCaixa/
│
├── 📂 PDVnet.ControleCaixa.UI/         
│   ├── Views/                          
│   ├── ViewModels/                      
│   ├── Assets/                         
│   ├── App.xaml                        
│   └── App.config                       
│
├── 📂 PDVnet.ControleCaixa.Business/   
│   ├── Services/
│   │   └── MovimentacaoService.cs      
│   └── Validators/
│       └── MovimentacaoValidator.cs    
│
├── 📂 PDVnet.ControleCaixa.Data/        
│   ├── MovimentacaoRepository.cs        
│   ├── ConnectionHelper.cs              
│   └── Scripts/
│       └── CreateDatabase.sql           
│
├── 📂 PDVnet.ControleCaixa.Model/      
│   ├── MovimentacaoCaixa.cs             
│   ├── Enums/TipoMovimentacao.cs        
│   ├── DTOs/DashboardDTO.cs             
│   └── Interfaces/                      
│
├── 📂 PDVnet.ControleCaixa.Tests/      
│   ├── Fakes/FakeMovimentacaoRepository.cs
│   └── Services/MovimentacaoServiceTests.cs
```

---

## Pré-requisitos

Antes de rodar o projeto, certifique-se de ter instalado:

- [x] **Visual Studio 2022** com a carga de trabalho **"Desenvolvimento para Desktop com .NET"**
- [x] **SQL Server LocalDB** 
- [x] **.NET 10 SDK**

---

## Como Rodar o Projeto

### Passo 1 - Clonar o Repositório

Abra o terminal (Git Bash, CMD ou PowerShell) e execute:

```bash
git clone https://github.com/JoaoLua/projeto-teste.git
```

---

### Passo 2 - Criar o Banco de Dados

1. Abra o arquivo `PDVnet.ControleCaixa.Data/Scripts/CreateDatabase.sql` no Visual Studio
2. Clique no botão **Executar** no canto superior esquerdo do editor:

   ![Executar o script](https://github.com/user-attachments/assets/de4ead7b-1eb0-46a5-bf58-62e2f261f77e)

3. Como ainda não há conexão ativa (indicado por "Desconectado" na barra inferior), o Visual Studio abre a janela **Conectar**
4. Na aba **Procurar**, em **Nome do Servidor**, informe `(localdb)\MSSQLLocalDB`
5. Deixe **Autenticação do Windows** selecionado e **Nome do Banco de Dados** como `<padrão>` — o próprio script já cria e seleciona o banco (`CREATE DATABASE` + `USE`):

   ![Conectar ao LocalDB](https://github.com/user-attachments/assets/b249937b-5c5a-4354-bff0-201e092f54bb)

6. Clique em **Conectar**
7. O script roda, criando o banco `PDVnetControleCaixa` e a tabela `MovimentacaoCaixa`

---

### Passo 3 - Verificar a String de Conexão

O projeto já vem configurado para usar o **LocalDB** do Visual Studio. A configuração está no arquivo:

**Arquivo:** `PDVnet.ControleCaixa.UI/App.config`

```xml
<connectionStrings>
    <add name="PDVnetControleCaixa" 
         connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PDVnetControleCaixa;Integrated Security=True;TrustServerCertificate=True" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

> **Atenção:** Se o seu SQL Server usar uma instância diferente (ex: `localhost` ou `.\SQLEXPRESS`), altere o campo `Data Source` para a instância correta.

---

### Passo 4 - Abrir e Compilar a Solução

1. Abra o arquivo **`PDVnet.ControleCaixa.slnx`** no Visual Studio (duplo clique)
2. No **Gerenciador de Soluções**, verifique se todos os 5 projetos estão carregados:
   - `PDVnet.ControleCaixa.UI`
   - `PDVnet.ControleCaixa.Business`
   - `PDVnet.ControleCaixa.Data`
   - `PDVnet.ControleCaixa.Model`
   - `PDVnet.ControleCaixa.Tests`
3. Clique com o botão direito no projeto **`PDVnet.ControleCaixa.UI`** → **Definir como Projeto de Inicialização**
4. Pressione **Ctrl+Shift+B** para compilar

<img width="435" height="245" alt="image" src="https://github.com/user-attachments/assets/9c74e5cb-dbfa-41f5-9059-c3f29e44a63b" />


---

### Passo 5 - Executar a Aplicação

Pressione **F5** (ou clique no botão ▶️ verde) para iniciar.

A aplicação abrirá diretamente na **tela de Movimentações**, pronta para uso!

<img width="1365" height="727" alt="image" src="https://github.com/user-attachments/assets/09b93e63-55f5-42dc-b0a1-5eb37a58f1f9" />

---

## Como Usar a Aplicação

### Cadastrar um Lançamento
1. Na tela de Movimentações, clique no botão **"+ Novo Lançamento"**
2. Preencha os campos: **Descrição**, **Valor**, **Tipo** (Entrada/Saída) e **Categoria**
3. Clique em **"Salvar"**

### Editar um Lançamento
1. Na tabela, clique no ícone de **lápis** na linha desejada
2. Altere os campos necessários
3. Clique em **"Salvar"**

### Excluir um Lançamento
1. Na tabela, clique no ícone de **lixeira** na linha desejada
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

##  Executando os Testes Unitários

O projeto inclui **9 testes unitários** cobrindo as regras de negócio da camada Business (Service):

### Para rodar os testes:

1. No Visual Studio, vá em **Teste → Gerenciador de Testes** (ou `Ctrl+E, T`)
2. Clique em **"Executar Todos os Testes"** 
3. Todos os 9 testes devem aparecer com ✅ verde

### Testes implementados:

| # | Teste | O que valida |
|:---:|:---|:---|
| 1 | `ObterResumoDashboard_DeveCalcularSaldoCorretamente` | Saldo = Entradas − Saídas |
| 2 | `VerificarAlertaSaldoBaixo_DeveRetornarTrue_QuandoAbaixoDoMinimo` | Alerta dispara quando saldo < limite |
| 3 | `VerificarAlertaSaldoBaixo_DeveRetornarFalse_QuandoAcimaDoMinimo` | Alerta NÃO dispara quando saldo > limite |
| 4 | `ListarPorFiltros_DataInicioMaiorQueDataFim_DeveLancarExcecao` | Filtro com datas invertidas é rejeitado |
| 5 | `Inserir_DeveAtribuirDataMovimentoAutomaticamente` | Data é gerada automaticamente ao inserir |
| 6 | `Inserir_ValorNegativo_DeveLancarExcecao` | Valor negativo é rejeitado na inserção |
| 7 | `Atualizar_DeveModificarDescricao` | Edição de movimentação funciona corretamente |
| 8 | `Excluir_DeveRemoverDaListaDeAtivas` | Exclusão lógica remove da lista de ativas |
| 9 | `BuscarPorId_DeveRetornarMovimentacaoCorreta` | Busca por ID retorna a movimentação certa |

<img width="888" height="401" alt="testes" src="https://github.com/user-attachments/assets/d993f360-dc61-474f-acac-b0ddb53ba8f2" />
