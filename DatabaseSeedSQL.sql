SET IDENTITY_INSERT [dbo].[Buildings] ON
INSERT INTO [dbo].[Buildings] ([Id]) VALUES (1)
INSERT INTO [dbo].[Buildings] ([Id]) VALUES (2)
INSERT INTO [dbo].[Buildings] ([Id]) VALUES (3)
SET IDENTITY_INSERT [dbo].[Buildings] OFF

SET IDENTITY_INSERT [dbo].[Addresses] ON
INSERT INTO [dbo].[Addresses] ([Id], [Country], [City], [Street], [Postcode], [BuildingId]) VALUES (1, N'Poland', N'Rzeszów', N'Maczka 66', N'35-001', 1)
INSERT INTO [dbo].[Addresses] ([Id], [Country], [City], [Street], [Postcode], [BuildingId]) VALUES (2, N'Poland', N'Rzeszów', N'Grunwaldzka 4', N'25-052', 2)
INSERT INTO [dbo].[Addresses] ([Id], [Country], [City], [Street], [Postcode], [BuildingId]) VALUES (3, N'Poland', N'Rzeszów', N'Matejki 96', N'35-111', 3)
SET IDENTITY_INSERT [dbo].[Addresses] OFF


SET IDENTITY_INSERT [dbo].[Rooms] ON
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (1, 29, 1, 1, N'Workplace')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (2, 15, 2, 1, N'Workplace')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (3, 25, 2, 1, N'Workplace')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (4, 35, 1, 1, N'Workplace')

INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (5, 30, 3, 2, N'Workplace')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (6, 70, 3, 2, N'Workplace')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (7, 50, 2, 2, N'Workplace')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (8, 20, 2, 2, N'Workplace')

INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (9, 29, 5, 3, N'Workplace')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (10, 10, 5, 3, N'Workplace')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (11, 25, 5, 3, N'Workplace')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (12, 30, 5, 3, N'Workplace')
SET IDENTITY_INSERT [dbo].[Rooms] OFF


