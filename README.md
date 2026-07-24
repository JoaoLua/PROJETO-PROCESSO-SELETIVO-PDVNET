# PDVnet Controle Caixa

Sistema de Controle de Caixa desenvolvido como projeto para o processo seletivo de **Desenvolvedor Júnior**. A aplicação é Desktop e foi construída com C# e WPF, utilizando banco de dados SQL Server. 

O sistema contempla o lançamento, consulta, edição e exclusão de movimentações financeiras, bem como um dashboard para resumo das operações e controle de alertas de saldo.

## 🚀 Tecnologias Utilizadas

- **C# / .NET**
- **WPF (Windows Presentation Foundation)**
- **SQL Server** (ADO.NET Nativo sem Entity Framework)
- **MSTest** (Testes Unitários da Camada de Negócios)
- Arquitetura MVVM em Camadas.

## 📁 Estrutura do Projeto (Camadas)

A solução foi estruturada para garantir escalabilidade, manutenibilidade e separação clara de responsabilidades:

- `PDVnet.ControleCaixa.UI`: Camada de Apresentação (WPF). Contém as Telas (Views) e ViewModels baseados no padrão MVVM.
- `PDVnet.ControleCaixa.Business`: Camada de Regras de Negócio. Centraliza serviços (`MovimentacaoService`) e validadores (`MovimentacaoValidator`), garantindo os requisitos do projeto.
- `PDVnet.ControleCaixa.Data`: Camada de Acesso a Dados. Interage com o banco de dados via SQL puro utilizando ADO.NET (`SqlConnection`, `SqlCommand`).
- `PDVnet.ControleCaixa.Model`: Camada transversal de Modelos (Entidades, Enums e DTOs) utilizada em todas as camadas.
- `PDVnet.ControleCaixa.Tests`: Projeto de testes unitários desenvolvidos com **MSTest**, focados em atestar regras de saldo, filtros avançados e criação correta das entidades via TDD.

## ⚙️ Funcionalidades e Regras de Negócio Implementadas

✔️ **CRUD Completo:** Lançamento, listagem, edição e exclusão lógica (status) das movimentações.
✔️ **Regras de Negócio Seguras:** Bloqueio de inserção com valores negativos, descrições vazias ou filtros incongruentes. O tipo "Entrada/Saída" define implicitamente o sinal.
✔️ **Filtros e Consultas Avançadas (Extra):** Filtragem de dados com query dinâmica suportando busca por *Descrição*, *Data de Início* e *Data de Fim*.
✔️ **Dashboards e Alertas:** Tela de resumo calculando totais de Entradas e Saídas e exibindo um Alerta Visual de "Saldo Baixo" paramétrico.
✔️ **Validação na UI (Extra):** Formulários não aceitam letras em campos numéricos através de Regex em eventos.
✔️ **Testes Unitários (Extra):** Cobertura das regras essenciais utilizando repositórios falsos (Fakes).

## 🛠️ Como rodar o projeto

### 1. Banco de Dados
1. Abra o **SQL Server Management Studio (SSMS)**.
2. Localize o script de criação na pasta do projeto: `PDVnet.ControleCaixa.Data\Scripts\CreateDatabase.sql`
3. Execute o script. Ele irá verificar e criar automaticamente o banco de dados `PDVnetControleCaixa` e a tabela `MovimentacaoCaixa`.

### 2. Conexão
1. O projeto acessa o SQL Server via conexão integrada ao Windows (`Integrated Security=true`). 
2. A Connection String padrão é direcionada para `(localdb)\MSSQLLocalDB` (padrão de desenvolvimento rápido no Visual Studio) ou `localhost`. Se o seu SQL Server estiver em outra instância, atualize a variável na classe `ConnectionHelper.cs` (na camada `Data`).

### 3. Execução
1. Abra o arquivo `PDVnet.ControleCaixa.sln` com o **Visual Studio**.
2. Defina o projeto `PDVnet.ControleCaixa.UI` como **Startup Project** (Projeto de Inicialização).
3. Compile (Ctrl+Shift+B) e Execute (F5). O sistema iniciará já na tela principal listando as movimentações.

### 4. Executando os Testes
Para garantir que as regras estão passando, abra a aba **Test Explorer** no Visual Studio e clique em *Run All Tests* para executar os testes contidos em `PDVnet.ControleCaixa.Tests`.
