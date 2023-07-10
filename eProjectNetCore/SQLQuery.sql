Create database InstituteOfFineArts1
GO
USE InstituteOfFineArts1
GO
Create table competition (
	id varchar(50) primary key,
	name varchar(50),
	image varchar(255),
	description text,
	start_date DATETIME,
	end_date DATETIME,
	prize text,
	status varchar(20)
)
Create table class (
	id varchar(50) primary key,
	name varchar(250),
	status varchar(20),
	created_date DATETIME default(CURRENT_TIMESTAMP),
	updated_date DATETIME default(CURRENT_TIMESTAMP)
)
Create table account (
	id varchar(50) primary key,
	name varchar(100),
	userName varchar(100),
	email varchar(100),
	phone varchar(100),
	address varchar(100),
	password varchar(100),
	class_id varchar(50),
	status varchar(20),
	birthday DATETIME,
	created_date DATETIME default(CURRENT_TIMESTAMP),
	updated_date DATETIME default(CURRENT_TIMESTAMP)
	Foreign key (class_id) references class(id),
)
Create table project (
	id varchar(50) primary key,
	image varchar(255),
	description text,
	accountId varchar(50),
	price float,
	created_date DATETIME default(CURRENT_TIMESTAMP),
	updated_date DATETIME,
	status varchar(20),
	competitionId varchar(50),
	Foreign key (competitionId) references competition(id),
	Foreign key (accountId) references account(id)
)

Create table subject (
	id varchar(50) primary key,
	name varchar(250),
	status varchar(20),
	session int,
	created_date DATETIME default(CURRENT_TIMESTAMP),
	updated_date DATETIME
)
Create table userGroup(
	id tinyint primary key,
	name varchar(50),
	status varchar(20)
)
Create table [user] (
	id varchar(50) primary key,
	name varchar(100),
	email varchar(100),
	userName varchar(100),
	phone varchar(100),
	address varchar(100),
	password varchar(100),
	status varchar(20),
	groupId tinyint,
	created_date DATETIME default(CURRENT_TIMESTAMP),
	updated_date DATETIME default(CURRENT_TIMESTAMP)
	Foreign key (groupId) references userGroup(id)
)
Create table prize (
	id varchar(50) primary key,
	competition_id varchar(50),
	student_id varchar(50),
	project_id varchar(50),
	staff_id varchar(50),
	description text,
	status varchar(20),
)
Create table transaction (
	id varchar(50) primary key,
	project_id varchar(50),
	student_id varchar(50),
	price float,
	status varchar(20),
	infoCustomer text
)
create table menu(
	id varchar(50) primary key,
	groupId tinyint,
	name varchar(50),
	link varchar(50),
	status varchar(20),
	[order] tinyint,
	Foreign key (groupId) references userGroup(id)
)
Insert into userGroup(id, name, status) VALUES 
(1, 'ADMIN', 'ACTIVE'),
(2, 'STAFF', 'ACTIVE')

Insert into menu (id, groupId, name, link, status, [order]) values 
(1, '1', 'UserGroups', 'UserGroups', 'ACTIVE', 1),
(2, '1', 'Users', 'Users', 'ACTIVE', 2),
(3, '1', 'Menus', 'Menus', 'ACTIVE', 3),
(4, '1', 'Competitions', 'Competitions', 'ACTIVE', 4),
(5, '1', 'Classes', 'Classes', 'ACTIVE', 5),
(6, '1', 'Accounts', 'Accounts', 'ACTIVE', 6)
GO
Insert into [user](id, name, email, userName, phone, address, password, status, groupId) VALUES
('1', 'Admin1', 'Admin@gmail.com', 'Admin', '0987654321', 'HN', 'e10adc3949ba59abbe56e057f20f883e', 'ACTIVE', '1')