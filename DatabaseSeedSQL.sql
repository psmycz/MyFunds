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
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (3, 25, 2, 1, N'Other')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (4, 35, 1, 1, N'Workplace')

INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (5, 30, 3, 2, N'Warehouse')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (6, 70, 3, 2, N'Sanitary')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (7, 50, 2, 2, N'Warehouse')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (8, 20, 2, 2, N'Warehouse')

INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (9, 29, 5, 3, N'Workplace')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (10, 10, 5, 3, N'Sanitary')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (11, 25, 5, 3, N'Workplace')
INSERT INTO [dbo].[Rooms] ([Id], [Area], [Floor], [BuildingId], [Type]) VALUES (12, 30, 5, 3, N'Undefined')
SET IDENTITY_INSERT [dbo].[Rooms] OFF


SET IDENTITY_INSERT [dbo].[FixedAssets] ON
INSERT INTO [dbo].[FixedAssets] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [RoomId], [UserId], [Type]) VALUES (1, 'false', N'Stol', 750, '10/09/2018', '10/09/2020', 10, NULL, N'Static')
INSERT INTO [dbo].[FixedAssets] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [RoomId], [UserId], [Type]) VALUES (2, 'false', N'Stol', 800, '10/09/2018', '10/09/2020', 10, NULL, N'Static')
INSERT INTO [dbo].[FixedAssets] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [RoomId], [UserId], [Type]) VALUES (3, 'false', N'Stol', 1050, '10/09/2017', '10/09/2019', 9, NULL, N'Static')
INSERT INTO [dbo].[FixedAssets] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [RoomId], [UserId], [Type]) VALUES (4, 'false', N'Stol', 1250, '10/09/2017', '10/09/2019', 9, NULL, N'Static')

INSERT INTO [dbo].[FixedAssets] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [RoomId], [UserId], [Type]) VALUES (5, 'false', N'Komputer', 3900, '10/09/2019', '10/09/2022', 8, NULL, N'Rentable')
INSERT INTO [dbo].[FixedAssets] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [RoomId], [UserId], [Type]) VALUES (6, 'false', N'Komputer', 3800, '10/09/2019', '10/09/2022', 8, NULL, N'Rentable')
INSERT INTO [dbo].[FixedAssets] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [RoomId], [UserId], [Type]) VALUES (7, 'false', N'Monitor', 950, '10/09/2019', '10/09/2021', 8, NULL, N'Rentable')
INSERT INTO [dbo].[FixedAssets] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [RoomId], [UserId], [Type]) VALUES (8, 'false', N'Monitor', 900, '10/09/2019', '10/09/2021', 8, NULL, N'Rentable')

INSERT INTO [dbo].[FixedAssets] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [RoomId], [UserId], [Type]) VALUES (9, 'false', N'Krzeslo', 900, '10/09/2015', '10/09/2017', 10, NULL, N'Undefined')
INSERT INTO [dbo].[FixedAssets] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [RoomId], [UserId], [Type]) VALUES (10, 'false', N'Krzeslo', 900, '10/09/2015', '10/09/2017', 10, NULL, N'Static')
SET IDENTITY_INSERT [dbo].[FixedAssets] OFF

SET IDENTITY_INSERT [dbo].[FixedAssetArchives] ON
INSERT INTO [dbo].[FixedAssetArchives] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [TimeStamp], [RoomId], [UserId], [FixedAssetId], [Type]) VALUES (1, 'false', N'Stol', 758, '10/09/2017', '10/09/2021', '11/09/2018', 10, NULL, 1, N'Rentable')
INSERT INTO [dbo].[FixedAssetArchives] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [TimeStamp], [RoomId], [UserId], [FixedAssetId], [Type]) VALUES (2, 'false', N'Stol', 600, '10/09/2018', '11/09/2021', '11/09/2018', 10, NULL, 1, N'Rentable')
INSERT INTO [dbo].[FixedAssetArchives] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [TimeStamp], [RoomId], [UserId], [FixedAssetId], [Type]) VALUES (3, 'false', N'Stol', 700, '10/09/2018', '12/09/2021', '11/09/2018', 10, NULL, 1, N'Static')

INSERT INTO [dbo].[FixedAssetArchives] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [TimeStamp], [RoomId], [UserId], [FixedAssetId], [Type]) VALUES (4, 'false', N'Stolik', 7000, '10/09/2018', '12/09/2021', '01/07/2018', 8, NULL, 9, N'Static')
INSERT INTO [dbo].[FixedAssetArchives] ([Id], [InUse], [Name], [Price], [PurchaseDate], [WarrantyEndDate], [TimeStamp], [RoomId], [UserId], [FixedAssetId], [Type]) VALUES (5, 'false', N'Monitor', 699, '10/09/2018', '12/09/2020', '01/07/2019', 8, NULL, 8, N'Undefined')
SET IDENTITY_INSERT [dbo].[FixedAssetArchives] OFF


SET IDENTITY_INSERT [dbo].[MobileAssets] ON
INSERT INTO [dbo].[MobileAssets] ([Id], [Name], [InUse], [Price], [PurchaseDate], [WarrantyEndDate], [UserId]) VALUES (1, N'Samsung Galaxy S9', 'false', 2500, '10/09/2018', '10/09/2020', NULL)
INSERT INTO [dbo].[MobileAssets] ([Id], [Name], [InUse], [Price], [PurchaseDate], [WarrantyEndDate], [UserId]) VALUES (2, N'Laptop Acer Aspire', 'false', 2600, '10/09/2015', '10/09/2017', NULL)
INSERT INTO [dbo].[MobileAssets] ([Id], [Name], [InUse], [Price], [PurchaseDate], [WarrantyEndDate], [UserId]) VALUES (3, N'Xiaomi Redmi 7', 'false', 2500, '10/09/2018', '10/09/2020', NULL)
INSERT INTO [dbo].[MobileAssets] ([Id], [Name], [InUse], [Price], [PurchaseDate], [WarrantyEndDate], [UserId]) VALUES (4, N'Samsung Galaxy S5', 'false', 2500, '10/09/2014', '10/09/2015', NULL)
SET IDENTITY_INSERT [dbo].[MobileAssets] OFF

SET IDENTITY_INSERT [dbo].[MobileAssetArchives] ON
INSERT INTO [dbo].[MobileAssetArchives] ([Id], [Name], [InUse], [Price], [PurchaseDate], [WarrantyEndDate], [TimeStamp], [UserId], [MobileAssetId]) VALUES (1, N'Samsung Galaxy S9', 'false', 2799, '10/09/2017', '10/09/2021', '11/09/2018', NULL, 1)
INSERT INTO [dbo].[MobileAssetArchives] ([Id], [Name], [InUse], [Price], [PurchaseDate], [WarrantyEndDate], [TimeStamp], [UserId], [MobileAssetId]) VALUES (2, N'Xiaomi Redmi 7 - XiaomiLepszeXD', 'false', 250, '10/09/2017', '10/09/2021', '11/09/2018', NULL, 3)
SET IDENTITY_INSERT [dbo].[MobileAssetArchives] OFF
