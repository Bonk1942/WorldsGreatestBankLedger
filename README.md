# WorldsGreatestBankLedger

Run the CreateDB.SQL script to create the database tables, views, and stored procedures needed by the app.
You will need to grab the connection string for your SQL Instance and update it in the app.config.
```
<connectionStrings>
    <!-- SQL DB (uncomment the appropriate connectionString-->
    <add name="dbCon" connectionString="Data Source=BELDZB15U31619\MSSQLSERVER2017;
                                        Initial Catalog=WorldsGreatestBankingLedger;
                                        Integrated Security=True" />
    <!--
    <add name="dbCon" connectionString="Data Source = localhost; 
                                        Initial Catalog = WorldsGreatestBankingLedger; 
                                        Integrated Security = True" />
    -->
  </connectionStrings>
```
