CREATE TABLE a_mess_table (
    Mess_Id INT PRIMARY KEY IDENTITY,    
    Mess_Description VARCHAR(25) NOT NULL,
    Mess_Detail VARCHAR(200) NOT NULL,
	created_at DATETIME NOT NULL
);