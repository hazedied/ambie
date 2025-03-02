﻿using AmbientSounds.Services;
using AmbientSounds.Tools;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmbientSounds.ViewModels;

public partial class ShareViewModel : ObservableObject
{
    private readonly ISoundService _soundService;
    private readonly IShareService _shareService;
    private readonly IClipboard _clipboard;

    [ObservableProperty]
    private string _shareText = string.Empty;

    [ObservableProperty]
    private string _shareUrl = string.Empty;

    [ObservableProperty]
    private bool _loading;

    [ObservableProperty]
    private bool _copySuccessful;

    public ShareViewModel(
        ISoundService soundService,
        IShareService shareService,
        IClipboard clipboard)
    {
        Guard.IsNotNull(soundService);
        Guard.IsNotNull(shareService);
        Guard.IsNotNull(clipboard);
        _soundService = soundService;
        _shareService = shareService;
        _clipboard = clipboard;
    }

    public async Task InitializeAsync(IReadOnlyList<string> soundIds)
    {
        Loading = true;
        var shareTask = _shareService.GetShareUrlAsync(soundIds);
        var sounds = await _soundService.GetLocalSoundsAsync(soundIds);
        ShareText = string.Join(" • ", sounds.Select(x => x.Name));
        ShareUrl = await shareTask;
        Loading = false;
    }

    public void Uninitialize()
    {
        ShareText = string.Empty;
        ShareUrl = string.Empty;
        CopySuccessful = false;
    }

    [RelayCommand]
    private async Task CopyAsync()
    {
        CopySuccessful = await _clipboard.CopyToClipboardAsync(ShareUrl);
    }
}
