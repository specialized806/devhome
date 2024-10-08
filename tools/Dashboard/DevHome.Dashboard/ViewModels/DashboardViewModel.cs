﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DevHome.Common.Helpers;
using DevHome.Dashboard.Services;
using Microsoft.UI.Xaml;

namespace DevHome.Dashboard.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    public IWidgetServiceService WidgetServiceService { get; }

    public IWidgetHostingService WidgetHostingService { get; }

    public IWidgetIconService WidgetIconService { get; }

    public IWidgetScreenshotService WidgetScreenshotService { get; }

    public ObservableCollection<WidgetViewModel> PinnedWidgets { get; set; }

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasWidgetServiceInitialized;

    public DashboardViewModel(
        IWidgetServiceService widgetServiceService,
        IWidgetHostingService widgetHostingService,
        IWidgetIconService widgetIconService,
        IWidgetScreenshotService widgetScreenshotService)
    {
        WidgetServiceService = widgetServiceService;
        WidgetHostingService = widgetHostingService;
        WidgetIconService = widgetIconService;
        WidgetScreenshotService = widgetScreenshotService;

        PinnedWidgets = new ObservableCollection<WidgetViewModel>();
    }

    public Visibility GetNoWidgetMessageVisibility(int widgetCount, bool isLoading)
    {
        return (widgetCount == 0 && !isLoading && HasWidgetServiceInitialized) ? Visibility.Visible : Visibility.Collapsed;
    }

    public bool IsRunningElevated()
    {
        return RuntimeHelper.IsCurrentProcessRunningElevated();
    }
}
