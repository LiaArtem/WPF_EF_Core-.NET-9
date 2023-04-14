# WPF_EF_Core-.NET-7
WPF project - Test project using Entity Framework Core to the databases (Oracle, MS SQL, Azure SQL, PostgreSQL, MySQL, MariaDB, IBM DB2, IBM Informix, Firebird, SQLite).

---------------------------------------------------------------------------------
Azure SQL Database
 1) для роботи необхідно в Azure створити базу даних - TestDB, користувач admin-dbserver
    Сортування - Cyrillic_General_CI_AS
 2) перенести об'єкти скриптом ./sql/SQL_Azure_Migration.sql

---------------------------------------------------------------------------------
MySQL та MariaDB
  - якщо встановлюємо дві бази, буде проблема, оскільки вони використовують один порт для роботи за замовчуванням 3306.
    - для MySQL ставимо порт за замовчуванням = 3306
    - для MariaDB ставимо порт за замовчуванням = 3307

---------------------------------------------------------------------------------
IBM DB2
  1) видаємо адмін. права користувача db2admin:
    - запускаємо з Пуск -> Командне вікно DB2 - Адміністратор
    -> db2 connect to SAMPLE
    -> db2 grant DBADM on DATABASE to user db2admin
    -> db2 terminate
 2) перенести об'єкти скриптом в DBeaver ./sql/SQL_IBM_DB2_Migration_1.sql, ./sql/SQL_IBM_DB2_Migration_2.sql
 3) Для тестування та роботи необхідно вказати шлях до драйвера IBM CLI.
    - Драйвер IBM CLI буде встановлений автоматично під час встановлення постачальника IBM Data Server для пакета EntityFramework Core.
    - За замовчуванням драйвер IBM CLI буде розташований у <каталог установки пакета nuget>\<версія>\build\clidriver\bin:
    - У Windows змініть змінне середовище PATH, щоб воно містило %userprofile%\.nuget\packages\IBM.Data.DB2.Core\<версія>\build\clidriver\bin
    - Перезапустіть програму.
    - завантажити та розпакувати clidriver (https://www.ibm.com/support/pages/db2-odbc-cli-driver-download-and-installation-information)
    - За замовчуванням мої настройки: програма розташована ..\Project\Project C#\WPF_EF_Core
      папка clidriver має бути розташована ..\Project\clidriver

---------------------------------------------------------------------------------
IBM Informix
  1) встановлюємо IBM Informix без інсталяції Instance (логін: informix (за замовчуванням), пароль: 12345678)
  2) запускаємо Server Instance Manager та створюємо підключення
     - Dynamic Server Name: informix_test
     - Service Name: turbo_test
     - Port number: 9088 (за замовчуванням)
     - Password: 12345678
  3) підключення DBeaver
     - host: localhost
     - server: informix_test
     - database/schema: sysadmin
     - user: informix
     - password: 12345678
  4) створюємо базу даних SAMPLE скриптом у DBeaver./sql/SQL_IBM_DB2_Migration_1.sql
  5) перепідключаємо з'єднання DBeaver на базу SAMPLE і виконуємо скрипт ./sql/SQL_IBM_DB2_Migration_2.sql
  6) Для тестування та роботи необхідно вказати шлях до драйвера IBM CLI.
    - Драйвер IBM CLI буде встановлений автоматично під час встановлення постачальника IBM Data Server для пакета EntityFramework Core.
    - За замовчуванням драйвер IBM CLI буде розташований у <каталог установки пакета nuget>\<версія>\build\clidriver\bin:
    - У Windows змініть змінне середовище PATH, щоб воно містило %userprofile%\.nuget\packages\IBM.Data.DB2.Core\<версія>\build\clidriver\bin
    - Перезапустіть програму.

    - завантажити та розпакувати clidriver:
      https://www.ibm.com/support/pages/db2-odbc-cli-driver-download-and-installation-information
    - За замовчуванням мої настройки: програма розташована ..\Project\Project C#\WPF_EF_Core
      папка clidriver має бути розташована ..\Project\clidriver

---------------------------------------------------------------------------------
Міграція об'єктів засобами EF Core
Для створення та виконання міграції перейдемо у Visual Studio до вікна Package Manager Console
(Средства > Диспетчер пакетів NuGet > Консоль диспетчера пакетів).
  - введемо команду -> Add-Migration InitialCreate
  - введемо команду -> Update-Database

