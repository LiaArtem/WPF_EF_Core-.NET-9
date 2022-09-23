# WPF_EF_Core-.NET-6
WPF project - Test project using Entity Framework Core to the databases (Oracle, MS SQL, Azure SQL, PostgreSQL, MySQL, IBM DB2, SQLite).

---------------------------------------------------------------------------------
Azure SQL Database
 1) для работы необходимо в Azure создать базу данных - TestDB, пользователь admin-dbserver
    Сортировка - Cyrillic_General_CI_AS
 2) перенести объекты скриптом ./sql/SQL_Azure_Migration.sql

---------------------------------------------------------------------------------
IBM DB2
  1) выдаем админ. права пользователю db2admin:
    - запускаем из Пуск -> Командное окно DB2 - Администратор
    -> db2 connect to SAMPLE
    -> db2 grant DBADM on DATABASE to user db2admin
    -> db2 terminate
 2) перенести объекты скриптом в DBeaver ./sql/SQL_IBM_DB2_Migration_1.sql, ./sql/SQL_IBM_DB2_Migration_2.sql
 3) Для тестирования и работы необходимо указать путь к драйверу IBM CLI.
    - Драйвер IBM CLI будет установлен автоматически при установке поставщика IBM Data Server для пакета EntityFramework Core.
    - По умолчанию драйвер IBM CLI будет расположен в <каталог установки пакета nuget>\<версия>\build\clidriver\bin:
    - В Windows измените переменную среды PATH, чтобы она содержала %userprofile%\.nuget\packages\IBM.Data.DB2.Core\<версия>\build\clidriver\bin
    - Перезапустите приложение.

    - распаковать clidriver.7z.
    - По умолчанию мои настройки: приложение расположено d:\Прочие\Project\Project C#\WPF_EF_Core
      папка clidriver должна быть расположена d:\Прочие\Project\clidriver

---------------------------------------------------------------------------------
Миграция объектов средствами EF Core
Для создания и выполнения миграции перейдем в Visual Studio к окну Package Manager Console
(Средства > Диспетчер пакетов NuGet > Консоль диспетчера пакетов).
  - введем команду -> Add-Migration InitialCreate
  - введем команду -> Update-Database

