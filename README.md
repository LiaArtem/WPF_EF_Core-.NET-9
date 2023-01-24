# WPF_EF_Core-.NET-7
WPF project - Test project using Entity Framework Core to the databases (Oracle, MS SQL, Azure SQL, PostgreSQL, MySQL, MariaDB, IBM DB2, IBM Informix, Firebird, SQLite).

---------------------------------------------------------------------------------
Azure SQL Database
 1) для работы необходимо в Azure создать базу данных - TestDB, пользователь admin-dbserver
    Сортировка - Cyrillic_General_CI_AS
 2) перенести объекты скриптом ./sql/SQL_Azure_Migration.sql

---------------------------------------------------------------------------------
MySQL и MariaDB
  - если устанавливаем две базы, будет проблема так как они используют один порт для работы по умолчанию 3306.
    - для MySQL ставим порт по умолчанию = 3306
    - для MariaDB ставим порт по умолчанию = 3307

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

    - скачать и распаковать clidriver (https://www.ibm.com/support/pages/db2-odbc-cli-driver-download-and-installation-information)
    - По умолчанию мои настройки: приложение расположено d:\Прочие\Project\Project C#\WPF_EF_Core
      папка clidriver должна быть расположена d:\Прочие\Project\clidriver

---------------------------------------------------------------------------------
IBM Informix
  1) устанавливаем IBM Informix без инсталляции Instance (логин: informix (по умолчанию), пароль: 12345678)
  2) запускаем Server Instance Manager и создаем подключение
     - Dynamic Server Name: informix_test
     - Service Name: turbo_test
     - Port number: 9088 (по умолчанию)
     - Password: 12345678
  3) подключение DBeaver
     - host:   localhost
     - server: informix_test
     - database/schema: sysadmin
     - user: informix
     - password: 12345678
  4) создаем базу данных SAMPLE скриптом в DBeaver ./sql/SQL_IBM_DB2_Migration_1.sql
  5) переподключаем соединение DBeaver на базу SAMPLE и выполняем скрипт ./sql/SQL_IBM_DB2_Migration_2.sql
  6) Для тестирования и работы необходимо указать путь к драйверу IBM CLI.
    - Драйвер IBM CLI будет установлен автоматически при установке поставщика IBM Data Server для пакета EntityFramework Core.
    - По умолчанию драйвер IBM CLI будет расположен в <каталог установки пакета nuget>\<версия>\build\clidriver\bin:
    - В Windows измените переменную среды PATH, чтобы она содержала %userprofile%\.nuget\packages\IBM.Data.DB2.Core\<версия>\build\clidriver\bin
    - Перезапустите приложение.

    - скачать и распаковать clidriver (https://www.ibm.com/support/pages/db2-odbc-cli-driver-download-and-installation-information)
    - По умолчанию мои настройки: приложение расположено d:\Прочие\Project\Project C#\WPF_EF_Core
      папка clidriver должна быть расположена d:\Прочие\Project\clidriver

---------------------------------------------------------------------------------
Миграция объектов средствами EF Core
Для создания и выполнения миграции перейдем в Visual Studio к окну Package Manager Console
(Средства > Диспетчер пакетов NuGet > Консоль диспетчера пакетов).
  - введем команду -> Add-Migration InitialCreate
  - введем команду -> Update-Database

