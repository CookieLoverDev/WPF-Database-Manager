import sqlite3
from randomuser import RandomUser as r

db = r"C:\Users\Victus\source\repos\Text Editor\bin\Debug\net8.0-windows\mainDB.db"
query = "INSERT INTO info (PersonID, Name, Surname, Email, Role, Description) VALUES (?, ?, ?, ?, ?, ?)"
peopleList = r.generate_users(50)

i = 2

connection = sqlite3.connect(db)
cur = connection.cursor()
for person in peopleList:
    cur.execute(query, (str(i), 
                        person.get_first_name(), 
                        person.get_last_name(), 
                        person.get_email(), 
                        person.get_nat(), 
                        "Rab"))
    i += 1
  
connection.commit()
connection.close()