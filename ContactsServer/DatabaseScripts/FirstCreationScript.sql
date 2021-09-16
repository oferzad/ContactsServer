Use master
Create Database ContactsDB
Go

Use ContactsDB
Go


Create Table Users (
ID int Identity primary key,
Email nvarchar(100) not null,
FirstName nvarchar(30) not null,
LastName nvarchar(30) not null,
UserPswd nvarchar(30) not null,
CONSTRAINT UC_Email UNIQUE(Email)
)

Go

INSERT INTO Users (Email, FirstName, LastName, UserPswd)
VALUES ('kuku@kaka.com', N'קוקו', N'קקה', N'1234')
GO

Create Table UserContacts (
UserID int not null FOREIGN KEY REFERENCES Users(ID),
ContactID int IDENTITY Primary Key,
FirstName nvarchar(30) not null,
LastName nvarchar(30) not null,
Email nvarchar(100) not null
)
Go

Create Table PhoneTypes (
TypeID int not null Primary Key,
TypeName nvarchar(10) not null
)
Go


Create Table ContactPhones (
PhoneID int not null IDENTITY Primary Key,
ContactID int not null FOREIGN KEY REFERENCES UserContacts(ContactID),
PhoneTypeID int FOREIGN KEY REFERENCES PhoneTypes(TypeID),
PhoneNumber nvarchar(20) not null
)

Go

INSERT INTO PhoneTypes VALUES (1, N'בית')
INSERT INTO PhoneTypes VALUES (2, N'עבודה')
INSERT INTO PhoneTypes VALUES (3, N'סלולרי')
INSERT INTO PhoneTypes VALUES (4, N'אחר')
Go

