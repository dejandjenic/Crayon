create table Customer(
                         ID   uuid not null primary key default uuid(),
                         Name nvarchar(50) not null
);

create table Account
(
    ID   uuid not null primary key default uuid(),
    Name nvarchar(50) not null,
    IDCustomer uuid not null,
    CONSTRAINT `FK_Customer`
        FOREIGN KEY (IDCustomer) REFERENCES Customer (ID)
);


create table Subscription
(
    ID   uuid not null primary key default uuid(),
    Name nvarchar(50) not null,
    IDAccount uuid not null,
    CONSTRAINT `FK_Account`
        FOREIGN KEY (IDAccount) REFERENCES Account (ID)
);


create table Licence
(
    ID   uuid not null primary key default uuid(),
    Value nvarchar(50) not null,
    IDSubscription uuid not null,
    CONSTRAINT `FK_Subscription`
        FOREIGN KEY (IDSubscription) REFERENCES Subscription (ID)
);

