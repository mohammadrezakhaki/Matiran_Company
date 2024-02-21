﻿-- قدم اول: ابتدا دستور زیر را به تنهایی اجرا کنید تا دیتا بیس ابتدا ایجاد گردد
CREATE DATABASE [MatiranLibrary];


--قدم دوم :بعد از ایجاد شدن دیتا بیس دستورات زیر را اجرا فرمایید تا جداول ساخته شوند 
USE [MatiranLibrary];

CREATE TABLE [dbo].[Books](
    [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Title] [nvarchar](100) NOT NULL,
    [Publisher] [nvarchar](50) NULL,
    [ISBN] [nvarchar](50) NULL,
    [Count] [int] NOT NULL
);

CREATE TABLE [dbo].[Members](
    [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [FName] [nvarchar](50) NOT NULL,
    [LName] [nvarchar](50) NOT NULL,
    [NID] [nchar](15) NULL,
    [Mobile] [nchar](15) NULL
);

CREATE TABLE [dbo].[Rents](
    [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [BookId] [int] NOT NULL,
    [MemberId] [int] NOT NULL,
    [FromDate] [date] NULL,
    [ReturnDate] [date] NULL,
    [IsReturned] [bit] NOT NULL
);


-- قدم سوم : اضافه کردن رکورد های تستی به جداول در صورت تمایل
USE [MatiranLibrary];
insert into dbo.Books (Title,Publisher,ISBN,Count) values ('Hoghogh Yek','Kheyli Sabz','8799545648',5)
insert into dbo.Books (Title,Publisher,ISBN,Count) values ('Riazi Dovom','Samt','876541231312',3)
insert into dbo.Books (Title,Publisher,ISBN,Count) values ('Kafeh Naderi','Neshan','38793545',2)

insert into dbo.Members (FName,LName,NID,Mobile) values ('Reza','Khaki','0076373525','09126474851')
insert into dbo.Members (FName,LName,NID,Mobile) values ('Mohammad Reza','Gerami','1176373531','09134474351')
insert into dbo.Members (FName,LName,NID,Mobile) values ('Sara','Ahmadi','0126373531','09176374359')