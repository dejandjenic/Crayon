insert into Customer(id,name) values('8debd754-286d-4944-8fb5-1a48440f3848','Test');
insert into Account(id,IDCustomer,name) values('be386f83-0f8e-11f0-95c6-34f39a52020b','8debd754-286d-4944-8fb5-1a48440f3848','Department1');
insert into Account(id,IDCustomer,name) values('de386f83-0f8e-11f0-95c6-34f39a52020b','8debd754-286d-4944-8fb5-1a48440f3848','Department2');

insert into Subscription(id,name,IDAccount,ExternalId,Status,Expires) values('d3f91708-0f92-11f0-95c6-34f39a52020b','Microsoft Office','de386f83-0f8e-11f0-95c6-34f39a52020b','69048a5a-fb5f-4a65-b209-557c5ce4cbf7','Active','2025-10-10');
insert into Subscription(id,name,IDAccount,ExternalId,Status,Expires) values('d5d68be0-0f92-11f0-95c6-34f39a52020b','Microsoft SQL Server','de386f83-0f8e-11f0-95c6-34f39a52020b','a2bca648-d138-4072-b8c9-ccbcfd86df96','Active','2025-11-11');

insert into Licence(id,Value,IDSubscription) values('17efb216-0f93-11f0-95c6-34f39a52020b','a3f91708-0f92-11f0-95c6-34f39a52020b','d3f91708-0f92-11f0-95c6-34f39a52020b');
insert into Licence(id,Value,IDSubscription) values('1802829d-0f93-11f0-95c6-34f39a52020b','b3f91708-0f92-11f0-95c6-34f39a52020b','d3f91708-0f92-11f0-95c6-34f39a52020b');
insert into Licence(id,Value,IDSubscription) values('1902829d-0f93-11f0-95c6-34f39a52020b','c5d68be0-0f92-11f0-95c6-34f39a52020b','d5d68be0-0f92-11f0-95c6-34f39a52020b');
