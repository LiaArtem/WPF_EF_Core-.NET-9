# WPF_EF_Core-.NET-6
WPF project - Test project using Entity Framework Core to the databases (Azure SQL, MS SQL, Oracle, MySQL, PostgreSQL, SQLite).

Azure SQL Database
 1) для работы необходимо в Azure создать базу данных - TestDB, пользователь admin-dbserver
    Сортировка - Cyrillic_General_CI_AS
 2) перенести объекты скриптом ./SQL_Azure_Migration.sql

Миграция объектов средствами EF Core
Для создания и выполнения миграции перейдем в Visual Studio к окну Package Manager Console
(Средства > Диспетчер пакетов NuGet > Консоль диспетчера пакетов).
  - введем команду -> Add-Migration InitialCreate
  - введем команду -> Update-Database

