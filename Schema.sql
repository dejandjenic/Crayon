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
    ID          uuid                                          default uuid()              not null
        primary key,
    Name        varchar(50) charset utf8mb3                                               not null,
    IDAccount   uuid                                                                      not null,
    DateCreated datetime                                      default current_timestamp() not null,
    ExternalId  uuid                                                                      not null,
    Status      enum ('Created', 'Error', 'Active', 'Processing','Cancelled') default 'Created'           null,
    Expires     datetime                                                                  null,
    constraint FK_Account
        foreign key (IDAccount) references Account (ID)
);


create table Licence
(
    ID   uuid not null primary key default uuid(),
    Value nvarchar(50) not null,
    IDSubscription uuid not null,
    CONSTRAINT `FK_Subscription`
        FOREIGN KEY (IDSubscription) REFERENCES Subscription (ID)
);

