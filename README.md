# WorldsGreatestBankLedger

Run the CreateDB.SQL script to create the database tables, views, and stored procedures needed by the app.
You will need to grab the connection string for your SQL Instance and update it in the DBConnect method 
located in the SQL_Data class in the Private Methods region.
```
private static void DBConnect()//using this so I can easily change Connection String depending on where I am working on this.
        {
            //Steves local desktop (comment out if not using)
            //con.ConnectionString = (@"Data Source = localhost; Initial Catalog = WorldsGreatestBankingLedger; Integrated Security = True");

            //Steves local laptop (comment out if not using)
            con.ConnectionString = (@"Data Source=BELDZB15U31619\MSSQLSERVER2017;Initial Catalog=WorldsGreatestBankingLedger;Integrated Security=True");

            //your custom DB Connection String (comment out if not using)
            //con.ConnectionString = (@"CHANGE ME TO YOUR CONNECTION STRING AND UNCOMMENT");
        }
```
