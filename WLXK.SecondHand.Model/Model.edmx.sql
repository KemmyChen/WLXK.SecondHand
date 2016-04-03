
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/19/2015 14:45:25
-- Generated from EDMX file: G:\C#\我要二手网\青职二货街\SecondHand\6.第五版（加失物招领版）\WLXK.SecondHand\WLXK.SecondHand.Model\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [SecondHand];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserInfoGoods]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Goods] DROP CONSTRAINT [FK_UserInfoGoods];
GO
IF OBJECT_ID(N'[dbo].[FK_GoodsSaleInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SaleInfo] DROP CONSTRAINT [FK_GoodsSaleInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_UserInfoShoping]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Shoping] DROP CONSTRAINT [FK_UserInfoShoping];
GO
IF OBJECT_ID(N'[dbo].[FK_ShopingGoods]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Shoping] DROP CONSTRAINT [FK_ShopingGoods];
GO
IF OBJECT_ID(N'[dbo].[FK_UserInfoMySee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MySee] DROP CONSTRAINT [FK_UserInfoMySee];
GO
IF OBJECT_ID(N'[dbo].[FK_GoodsMySee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MySee] DROP CONSTRAINT [FK_GoodsMySee];
GO
IF OBJECT_ID(N'[dbo].[FK_MyNewsUserInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MyNews] DROP CONSTRAINT [FK_MyNewsUserInfo];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[UserInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserInfo];
GO
IF OBJECT_ID(N'[dbo].[Goods]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Goods];
GO
IF OBJECT_ID(N'[dbo].[SaleInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SaleInfo];
GO
IF OBJECT_ID(N'[dbo].[Shoping]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Shoping];
GO
IF OBJECT_ID(N'[dbo].[UserInfoExt]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserInfoExt];
GO
IF OBJECT_ID(N'[dbo].[MySee]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MySee];
GO
IF OBJECT_ID(N'[dbo].[Suggest]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Suggest];
GO
IF OBJECT_ID(N'[dbo].[ListShops]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ListShops];
GO
IF OBJECT_ID(N'[dbo].[MyNews]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MyNews];
GO
IF OBJECT_ID(N'[dbo].[AdminUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdminUser];
GO
IF OBJECT_ID(N'[dbo].[ShiWu]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ShiWu];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'UserInfo'
CREATE TABLE [dbo].[UserInfo] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Uid] nvarchar(64)  NOT NULL,
    [Pwd] nvarchar(64)  NOT NULL,
    [DelFalg] smallint  NOT NULL,
    [Email] nvarchar(128)  NOT NULL,
    [IsValid] smallint  NOT NULL,
    [SendEmailTime] datetime  NULL,
    [SubTime] datetime  NOT NULL,
    [EmailCode] nvarchar(256)  NULL
);
GO

-- Creating table 'Goods'
CREATE TABLE [dbo].[Goods] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(128)  NOT NULL,
    [Type] smallint  NOT NULL,
    [MaiDian] nvarchar(256)  NULL,
    [Price] decimal(18,0)  NOT NULL,
    [Count] int  NOT NULL,
    [LaiYuan] nvarchar(64)  NOT NULL,
    [BigImageAddress] nvarchar(512)  NOT NULL,
    [Descript] nvarchar(2048)  NOT NULL,
    [DelGlag] smallint  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [UserInfoId] int  NOT NULL,
    [SmallImageAddress] nvarchar(512)  NOT NULL,
    [ShopingId] int  NOT NULL,
    [SeeCount] int  NOT NULL
);
GO

-- Creating table 'SaleInfo'
CREATE TABLE [dbo].[SaleInfo] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RealName] nvarchar(128)  NOT NULL,
    [QQ] nvarchar(32)  NOT NULL,
    [PhoneNum] nvarchar(11)  NOT NULL,
    [Address] nvarchar(max)  NOT NULL,
    [GoodsId] int  NOT NULL
);
GO

-- Creating table 'Shoping'
CREATE TABLE [dbo].[Shoping] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserInfoId] int  NOT NULL,
    [GoodsId] int  NOT NULL,
    [DelFlag] smallint  NOT NULL,
    [SubTime] datetime  NOT NULL
);
GO

-- Creating table 'UserInfoExt'
CREATE TABLE [dbo].[UserInfoExt] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserInfoId] int  NOT NULL,
    [NickName] nvarchar(32)  NOT NULL,
    [RealName] nvarchar(32)  NULL,
    [Gender] smallint  NOT NULL,
    [Birthday] datetime  NULL,
    [Address] nvarchar(256)  NULL,
    [TouXiang] nvarchar(256)  NULL
);
GO

-- Creating table 'MySee'
CREATE TABLE [dbo].[MySee] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserInfoId] int  NOT NULL,
    [GoodsId] int  NOT NULL,
    [SubTime] datetime  NOT NULL
);
GO

-- Creating table 'Suggest'
CREATE TABLE [dbo].[Suggest] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Content] nvarchar(512)  NOT NULL,
    [OpenSpeed] smallint  NOT NULL,
    [Beautiful] smallint  NOT NULL,
    [SettingType] smallint  NOT NULL,
    [Convenient] smallint  NOT NULL,
    [Vivid] smallint  NOT NULL,
    [AllPopulation] smallint  NOT NULL,
    [Aim] smallint  NOT NULL,
    [QQNum] nvarchar(16)  NULL,
    [Email] nvarchar(64)  NULL,
    [PhoneNum] nvarchar(64)  NULL,
    [SubTime] datetime  NOT NULL
);
GO

-- Creating table 'ListShops'
CREATE TABLE [dbo].[ListShops] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SubTime] datetime  NOT NULL,
    [UserInfoId] int  NOT NULL,
    [Status] smallint  NOT NULL,
    [RealName] nvarchar(32)  NOT NULL,
    [IdCard] nvarchar(64)  NOT NULL,
    [CardAddress] nvarchar(256)  NOT NULL,
    [CalssName] nvarchar(max)  NOT NULL,
    [IsShenHe] smallint  NOT NULL,
    [SheHeAdminId] int  NULL
);
GO

-- Creating table 'MyNews'
CREATE TABLE [dbo].[MyNews] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SubTime] datetime  NOT NULL,
    [Title] nvarchar(64)  NOT NULL,
    [IsSee] smallint  NOT NULL,
    [UserInfoId] int  NOT NULL
);
GO

-- Creating table 'AdminUser'
CREATE TABLE [dbo].[AdminUser] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Uid] nvarchar(128)  NOT NULL,
    [Pwd] nvarchar(128)  NOT NULL,
    [Email] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'ShiWu'
CREATE TABLE [dbo].[ShiWu] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PhoneNum] nvarchar(max)  NOT NULL,
    [ThingName] nvarchar(max)  NOT NULL,
    [Descript] nvarchar(max)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NOT NULL,
    [Type] smallint  NOT NULL,
    [Time] datetime  NOT NULL
);
GO

-- Creating table 'FriendLink'
CREATE TABLE [dbo].[FriendLink] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [KeyWords] nvarchar(max)  NOT NULL,
    [LinkTo] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'UserInfo'
ALTER TABLE [dbo].[UserInfo]
ADD CONSTRAINT [PK_UserInfo]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Goods'
ALTER TABLE [dbo].[Goods]
ADD CONSTRAINT [PK_Goods]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SaleInfo'
ALTER TABLE [dbo].[SaleInfo]
ADD CONSTRAINT [PK_SaleInfo]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Shoping'
ALTER TABLE [dbo].[Shoping]
ADD CONSTRAINT [PK_Shoping]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserInfoExt'
ALTER TABLE [dbo].[UserInfoExt]
ADD CONSTRAINT [PK_UserInfoExt]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MySee'
ALTER TABLE [dbo].[MySee]
ADD CONSTRAINT [PK_MySee]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Suggest'
ALTER TABLE [dbo].[Suggest]
ADD CONSTRAINT [PK_Suggest]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ListShops'
ALTER TABLE [dbo].[ListShops]
ADD CONSTRAINT [PK_ListShops]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MyNews'
ALTER TABLE [dbo].[MyNews]
ADD CONSTRAINT [PK_MyNews]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AdminUser'
ALTER TABLE [dbo].[AdminUser]
ADD CONSTRAINT [PK_AdminUser]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ShiWu'
ALTER TABLE [dbo].[ShiWu]
ADD CONSTRAINT [PK_ShiWu]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FriendLink'
ALTER TABLE [dbo].[FriendLink]
ADD CONSTRAINT [PK_FriendLink]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UserInfoId] in table 'Goods'
ALTER TABLE [dbo].[Goods]
ADD CONSTRAINT [FK_UserInfoGoods]
    FOREIGN KEY ([UserInfoId])
    REFERENCES [dbo].[UserInfo]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserInfoGoods'
CREATE INDEX [IX_FK_UserInfoGoods]
ON [dbo].[Goods]
    ([UserInfoId]);
GO

-- Creating foreign key on [GoodsId] in table 'SaleInfo'
ALTER TABLE [dbo].[SaleInfo]
ADD CONSTRAINT [FK_GoodsSaleInfo]
    FOREIGN KEY ([GoodsId])
    REFERENCES [dbo].[Goods]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GoodsSaleInfo'
CREATE INDEX [IX_FK_GoodsSaleInfo]
ON [dbo].[SaleInfo]
    ([GoodsId]);
GO

-- Creating foreign key on [UserInfoId] in table 'Shoping'
ALTER TABLE [dbo].[Shoping]
ADD CONSTRAINT [FK_UserInfoShoping]
    FOREIGN KEY ([UserInfoId])
    REFERENCES [dbo].[UserInfo]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserInfoShoping'
CREATE INDEX [IX_FK_UserInfoShoping]
ON [dbo].[Shoping]
    ([UserInfoId]);
GO

-- Creating foreign key on [GoodsId] in table 'Shoping'
ALTER TABLE [dbo].[Shoping]
ADD CONSTRAINT [FK_ShopingGoods]
    FOREIGN KEY ([GoodsId])
    REFERENCES [dbo].[Goods]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ShopingGoods'
CREATE INDEX [IX_FK_ShopingGoods]
ON [dbo].[Shoping]
    ([GoodsId]);
GO

-- Creating foreign key on [UserInfoId] in table 'MySee'
ALTER TABLE [dbo].[MySee]
ADD CONSTRAINT [FK_UserInfoMySee]
    FOREIGN KEY ([UserInfoId])
    REFERENCES [dbo].[UserInfo]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserInfoMySee'
CREATE INDEX [IX_FK_UserInfoMySee]
ON [dbo].[MySee]
    ([UserInfoId]);
GO

-- Creating foreign key on [GoodsId] in table 'MySee'
ALTER TABLE [dbo].[MySee]
ADD CONSTRAINT [FK_GoodsMySee]
    FOREIGN KEY ([GoodsId])
    REFERENCES [dbo].[Goods]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GoodsMySee'
CREATE INDEX [IX_FK_GoodsMySee]
ON [dbo].[MySee]
    ([GoodsId]);
GO

-- Creating foreign key on [UserInfoId] in table 'MyNews'
ALTER TABLE [dbo].[MyNews]
ADD CONSTRAINT [FK_MyNewsUserInfo]
    FOREIGN KEY ([UserInfoId])
    REFERENCES [dbo].[UserInfo]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MyNewsUserInfo'
CREATE INDEX [IX_FK_MyNewsUserInfo]
ON [dbo].[MyNews]
    ([UserInfoId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------