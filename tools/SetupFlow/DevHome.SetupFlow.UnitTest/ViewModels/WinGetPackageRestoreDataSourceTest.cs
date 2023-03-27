﻿// Copyright (c) Microsoft Corporation and Contributors
// Licensed under the MIT license.

using DevHome.Common.Extensions;
using DevHome.SetupFlow.AppManagement.Exceptions;
using DevHome.SetupFlow.AppManagement.Models;
using DevHome.SetupFlow.AppManagement.Services;
using DevHome.SetupFlow.UnitTest.Helpers;
using Microsoft.Internal.Windows.DevHome.Helpers.Restore;
using Microsoft.Management.Deployment;
using Moq;
using Windows.Storage.Streams;

namespace DevHome.SetupFlow.UnitTest.ViewModels;

[TestClass]
public class WinGetPackageRestoreDataSourceTest : BaseSetupFlowTest
{
    [TestMethod]
    public void LoadCatalogs_EmptyPackages_ReturnsNoCatalogs()
    {
        // Arrange
        var expectedPackages = new List<IWinGetPackage>();
        var restoreApplicationInfoList = expectedPackages.Select(p => CreateRestoreApplicationInfo(p.Id).Object).ToList();
        ConfigureWinGetCatalogPackages(expectedPackages);
        ConfigureRestoreDeviceInfo(RestoreDeviceInfoStatus.Ok, restoreApplicationInfoList);

        // Act
        var loadedPackages = LoadCatalogsFromRestoreDataSource();

        // Assert
        Assert.AreEqual(0, loadedPackages.Count);
    }

    [TestMethod]
    public void LoadCatalogs_ExceptionThrownWhenGettingPackages_ReturnsNoCatalogs()
    {
        // Arrange
        var catalogs = new Mock<IWinGetCatalog>();
        catalogs.Setup(c => c.GetPackagesAsync(It.IsAny<HashSet<string>>())).ThrowsAsync(new FindPackagesException(FindPackagesResultStatus.CatalogError));
        WindowsPackageManager!.Setup(wpm => wpm.WinGetCatalog).Returns(catalogs.Object);
        ConfigureRestoreDeviceInfo(RestoreDeviceInfoStatus.Ok, new List<IRestoreApplicationInfo>());

        // Act
        var loadedPackages = LoadCatalogsFromRestoreDataSource();

        // Assert
        Assert.AreEqual(0, loadedPackages.Count);
    }

    [TestMethod]
    [DataRow(RestoreDeviceInfoStatus.NotAvailable)]
    [DataRow(RestoreDeviceInfoStatus.Error)]
    public void LoadCatalogs_NonSuccessStatus_ReturnsNoCatalogs(RestoreDeviceInfoStatus status)
    {
        // Arrange
        ConfigureRestoreDeviceInfo(status, new List<IRestoreApplicationInfo>());

        // Act
        var loadedPackages = LoadCatalogsFromRestoreDataSource();

        // Assert
        Assert.AreEqual(0, loadedPackages.Count);
    }

    [TestMethod]
    [DataRow("mock1", "mock2")]
    [DataRow("mock2", "mock1")]
    public void LoadCatalogs_OrderedPackages_ReturnsWinGetCatalogWithMatchingInputOrder(string packageId1, string packageId2)
    {
        // Arrange
        var expectedPackages = new List<IWinGetPackage>
        {
            PackageHelper.CreatePackage(packageId1).Object,
            PackageHelper.CreatePackage(packageId2).Object,
        };
        var restoreApplicationInfoList = expectedPackages.Select(p => CreateRestoreApplicationInfo(p.Id).Object).ToList();
        ConfigureWinGetCatalogPackages(expectedPackages);
        ConfigureRestoreDeviceInfo(RestoreDeviceInfoStatus.Ok, restoreApplicationInfoList);

        // Act
        var loadedPackages = LoadCatalogsFromRestoreDataSource();

        // Assert
        Assert.AreEqual(1, loadedPackages.Count);
        Assert.AreEqual(expectedPackages.Count, loadedPackages[0].Packages.Count);
        Assert.AreEqual(expectedPackages[0].Id, loadedPackages[0].Packages.ElementAt(0).Id);
        Assert.IsNotNull(expectedPackages[0].LightThemeIcon);
        Assert.IsNotNull(expectedPackages[0].DarkThemeIcon);
        Assert.AreEqual(expectedPackages[1].Id, loadedPackages[0].Packages.ElementAt(1).Id);
        Assert.IsNotNull(expectedPackages[1].LightThemeIcon);
        Assert.IsNotNull(expectedPackages[1].DarkThemeIcon);
    }

    [TestMethod]
    public void LoadCatalogs_Success_ReturnsWinGetCatalogs()
    {
        // Arrange
        var expectedPackages = new List<IWinGetPackage>
        {
            PackageHelper.CreatePackage("mock1").Object,
            PackageHelper.CreatePackage("mock2").Object,
        };
        var restoreApplicationInfoList = expectedPackages.Select(p => CreateRestoreApplicationInfo(p.Id).Object).ToList();
        ConfigureWinGetCatalogPackages(expectedPackages);
        ConfigureRestoreDeviceInfo(RestoreDeviceInfoStatus.Ok, restoreApplicationInfoList);

        // Act
        var loadedPackages = LoadCatalogsFromRestoreDataSource();

        // Assert
        Assert.AreEqual(1, loadedPackages.Count);
        Assert.AreEqual(expectedPackages.Count, loadedPackages[0].Packages.Count);
        Assert.AreEqual(expectedPackages[0].Id, loadedPackages[0].Packages.ElementAt(0).Id);
        Assert.IsNotNull(expectedPackages[0].LightThemeIcon);
        Assert.IsNotNull(expectedPackages[0].DarkThemeIcon);
        Assert.AreEqual(expectedPackages[1].Id, loadedPackages[0].Packages.ElementAt(1).Id);
        Assert.IsNotNull(expectedPackages[1].LightThemeIcon);
        Assert.IsNotNull(expectedPackages[1].DarkThemeIcon);
    }

    [TestMethod]
    public void LoadCatalogs_ExceptionThrownWhenGettingRestoreApplicationIcon_ReturnsNullForIcon()
    {
        var expectedPackages = new List<IWinGetPackage>
        {
            PackageHelper.CreatePackage("mock").Object,
        };
        var restoreApplicationInfoList = expectedPackages.Select(p =>
        {
            var restoreAppInfo = CreateRestoreApplicationInfo(p.Id);

            // Mock restore application icon not found by throwing an exception
            restoreAppInfo
                .Setup(appInfo => appInfo.GetIconAsync(It.IsAny<RestoreApplicationIconTheme>()))
                .Throws(new ArgumentOutOfRangeException());

            return restoreAppInfo.Object;
        }).ToList();
        ConfigureWinGetCatalogPackages(expectedPackages);
        ConfigureRestoreDeviceInfo(RestoreDeviceInfoStatus.Ok, restoreApplicationInfoList);

        // Act
        var loadedPackages = LoadCatalogsFromRestoreDataSource();

        // Assert
        Assert.AreEqual(1, loadedPackages.Count);
        Assert.AreEqual(expectedPackages.Count, loadedPackages[0].Packages.Count);
        Assert.AreEqual(expectedPackages[0].Id, loadedPackages[0].Packages.ElementAt(0).Id);
        Assert.IsNull(expectedPackages[0].LightThemeIcon);
        Assert.IsNull(expectedPackages[0].DarkThemeIcon);
    }

    /// <summary>
    /// Configure WinGet catalog packages
    /// </summary>
    /// <param name="expectedPackages">Expected packages</param>
    private void ConfigureWinGetCatalogPackages(IList<IWinGetPackage> expectedPackages)
    {
        var catalog = new Mock<IWinGetCatalog>();
        catalog.Setup(c => c.GetPackagesAsync(It.IsAny<HashSet<string>>())).ReturnsAsync(expectedPackages);
        WindowsPackageManager!.Setup(wpm => wpm.WinGetCatalog).Returns(catalog.Object);
    }

    /// <summary>
    /// Load catalogs from restore data source
    /// </summary>
    /// <returns>List of package catalogs</returns>
    private IList<AppManagement.Models.PackageCatalog> LoadCatalogsFromRestoreDataSource()
    {
        var restoreDataSource = TestHost!.GetService<WinGetPackageRestoreDataSource>();
        restoreDataSource.InitializeAsync().GetAwaiter().GetResult();
        return restoreDataSource.LoadCatalogsAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Configure a restore device info including WinGet packages to restore
    /// </summary>
    /// <param name="status">Result status</param>
    /// <param name="restoreApplicationInfoList">Mock a list of application info to restore</param>
    private void ConfigureRestoreDeviceInfo(RestoreDeviceInfoStatus status, IList<IRestoreApplicationInfo> restoreApplicationInfoList)
    {
        // Mock restore device info
        var deviceInfo = new Mock<IRestoreDeviceInfo>();
        deviceInfo.Setup(di => di.WinGetApplicationsInfo).Returns(restoreApplicationInfoList);

        // Mock restore device info result
        var restoreDeviceInfoResult = new Mock<IRestoreDeviceInfoResult>();
        restoreDeviceInfoResult.Setup(result => result.Status).Returns(status);
        restoreDeviceInfoResult.Setup(result => result.RestoreDeviceInfo).Returns(deviceInfo.Object);
        RestoreInfo.Setup(restore => restore.GetRestoreDeviceInfoAsync()).Returns(Task.FromResult(restoreDeviceInfoResult.Object).AsAsyncOperation());
    }

    /// <summary>
    /// Create an application info to restore
    /// </summary>
    /// <param name="packageId">Id of the package corresponding to the application to restore</param>
    /// <returns>Restore application info</returns>
    private Mock<IRestoreApplicationInfo> CreateRestoreApplicationInfo(string packageId)
    {
        var appInfo = new Mock<IRestoreApplicationInfo>();

        // Mock id
        appInfo.Setup(app => app.Id).Returns(packageId);

        // Mock icon
        appInfo
            .Setup(app => app.GetIconAsync(It.IsAny<RestoreApplicationIconTheme>()))
            .Returns(Task.FromResult(new Mock<IRandomAccessStream>().Object).AsAsyncOperation());

        return appInfo;
    }
}