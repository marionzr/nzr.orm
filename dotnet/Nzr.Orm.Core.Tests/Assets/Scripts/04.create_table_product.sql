CREATE TABLE [core].product (
    product_id INT PRIMARY KEY IDENTITY,    
    description VARCHAR(200) NOT NULL,
    price decimal (10,2) NOT NULL
);