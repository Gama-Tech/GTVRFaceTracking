using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using VRCFaceTracking.Core;
using VRCFaceTracking.Core.Contracts.Services;

namespace VRCFaceTracking.ViewModels;

public class RiskySettingsViewModel : ObservableObject
{
    private readonly IMainService _mainService;
    public readonly IParamSupervisor ParamSupervisor;
    private readonly ILogger<RiskySettingsViewModel> _logger;

    private bool _enabled;
    public bool Enabled
    {
        get => _enabled;
        set => SetProperty(ref _enabled, value);
    }
    
    public RiskySettingsViewModel(IMainService mainService, IParamSupervisor paramSupervisor, ILogger<RiskySettingsViewModel> logger)
    {
        _mainService = mainService;
        ParamSupervisor = paramSupervisor;
        _logger = logger;
    }

    public void ForceReInit()
    {
        if (!Enabled)
            return;
        
        _logger.LogInformation("Reinitializing GT:VR Face Tracking...");
        
        _mainService.Teardown();
        
        _mainService.InitializeAsync();
    }

    public void ResetVRCFT()
    {
        _logger.LogInformation("Resetting GT:VR Face Tracking...");
        
        // Create a file in the GT:VR folder called "reset"
        // This will cause the app to reset on the next launch
        File.Create(Path.Combine(Utils.PersistentDataDirectory, "reset"));
    }

    public void ResetVRCAvatarConf()
    {
        _logger.LogInformation("Resetting GT:VR avatar configuration...");
        try
        {
            if (Directory.Exists(VRChat.VRCOSCDirectory))
            {
                var avatarFiles = Directory.GetFiles(VRChat.VRCOSCDirectory);
                foreach (var avatarFile in avatarFiles)
                    File.Delete(avatarFile);

                _logger.LogInformation($"{avatarFiles.Length} avatars were reset.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to reset GT:VR avatar configuration! {Message}", e.Message);
        }
    }
}