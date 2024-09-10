Consists of 2 applications:
- Console application
- WPF application

Tech-stack
- C#
- WPF
- Redis

Console Application:
- C# code to generate random stocks and prices of that stocks and uploads it in the Redis database.
- After the initial uploading of the stocks and their prices the C# code randomly updates the prices of that stocks every second and pushes on the Redis.
  
WPF Application:
- The stocks and its prices are shown using Grid layout in real time.
- WPF app shows 4 columns
  - Stock Name
  - Opening Price
  - Real time price
  - Last Updated time
- All the changes don on the Redis are visible on the application in REAL TIME.

Redis Server:
- Redis Server is used in the system to create a local in memory DB for doing the above operations.
