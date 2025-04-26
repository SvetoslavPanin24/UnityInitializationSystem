# Application Initialization System Documentation

## Table of Contents
1. [General Description](#general-description)
2. [System Architecture](#system-architecture)
3. [Using the Initialization System](#using-the-initialization-system)
4. [Creating New Modules](#creating-new-modules)
5. [Creating New Handlers](#creating-new-handlers)
6. [Working with Custom Editor](#working-with-custom-editor)
7. [Usage Examples](#usage-examples)

## General Description

The application initialization system is a modular architecture for Unity that provides a flexible and extensible way to initialize various subsystems in your project. It allows you to create, configure, and manage different initialization modules through the Unity editor, with support for data serialization, advertising, analytics, and other components.

## System Architecture

The system consists of the following main components:

1. **Initializer** - A MonoBehaviour component that starts the application initialization process
2. **ProjectInitSettings** - The main ScriptableObject containing a list of modules for initialization
3. **InitModule** - Base class for all initialization modules
4. **Handler** - Base class for handlers (services) used by modules
5. **Settings** - ScriptableObject classes for storing module settings

## Using the Initialization System

### Step 1: Configure ProjectInitSettings

1. Create a settings object via the menu: `Create > InitializationSystem > Settings > Project Init Settings`
2. Add the created settings object to the scene through the Initializer component

```csharp
// Add this component to a GameObject in the initial scene
[SerializeField] private ProjectInitSettings _initSettings;
```

### Step 2: Adding Initialization Modules

In the ProjectInitSettings editor, you can add, configure, and manage modules:

1. Open the created ProjectInitSettings in the inspector
2. Click the "Add Module" button to add a new module
3. Select the module type from existing ones (SaveModule, AdsModule, AnalyticsModule, etc.)
4. Configure the module parameters

### Step 3: Configure Individual Modules

For each module, you need to:

1. Create and configure the corresponding settings object (Settings)
2. Select appropriate handlers (Handlers) for the module
3. Configure handlers according to your project requirements

## Creating New Modules

To create a new initialization module:

1. Create a new class inheriting from `InitModule`
2. Implement the `StartInit()` method
3. Add the `CreateAssetMenu` attribute to enable creation via menu

```csharp
[CreateAssetMenu(fileName = "MyNewModule", menuName = "InitializationSystem/Modules/My New Module")]
public class MyNewModule : InitModule
{
    [SerializeField, Tooltip("Reference to the settings asset.")]
    private MyModuleSettings _settings;
    
    [SerializeField]
    private List<MyModuleHandler> _handlers;
    
    public override void StartInit() 
    {
        if(_settings.SystemLogs)
            Debug.Log($"[MyNewModule]: Initializing module '{this.name}'.");
            
        // Select handler and initialize
        var handler = _handlers?.FirstOrDefault(h => h != null && h.Enabled);
        if(handler != null)
        {
            handler.Init();
            // Initialize the corresponding manager
            MyManager.Init(handler, _settings);
        }
    }
}
```

## Creating New Handlers

Handlers implement specific functionality for modules. To create a new handler:

1. Create a base abstract class inheriting from `Handler`, defining the interface for a specific type of handlers
2. Implement concrete handlers inheriting from the base class
3. Add the `CreateAssetMenu` attribute to enable creation via menu

Example of creating a base handler class:

```csharp
public abstract class MyModuleHandler : Handler
{
    public abstract void SomeSpecificMethod();
    public abstract bool SomeProperty { get; }
}
```

Example of implementing a specific handler:

```csharp
[CreateAssetMenu(fileName = "MySpecificHandler", menuName = "InitializationSystem/Handlers/My Specific Handler")]
public class MySpecificHandler : MyModuleHandler
{
    [SerializeField] private MySpecificContainer _container;
    
    public override void Init()
    {
        // Handler initialization
        IsInitialized = true;
    }
    
    public override void SomeSpecificMethod()
    {
        // Method implementation
    }
    
    public override bool SomeProperty => true;
}
```

## Working with Custom Editor

The system uses custom editors for convenient configuration of modules and handlers in the Unity inspector.

### ProjectInitSettingsEditor

Editor for configuring the main project parameters and managing modules:

1. **Top Section**: Displays a banner and the "Project Initialization Settings" header
2. **Main Settings**: Displays the basic fields of the ProjectInitSettings object
3. **"Initialization Modules" Section**: List of added modules
4. **"Add Module" Button**: Adds a new module to the list

Module editor capabilities:

- **Drag & Drop**: You can drag modules into the list
- **Expandable Editors**: Each module can be expanded for detailed configuration
- **Module Deletion**: Delete button for each module in the list

### HandlerEditor

Editor for configuring handlers:

1. **Top Section**: Displays a header with the handler name
2. **Handler Type**: Displays the handler type (e.g., "Save", "Analytics")
3. **Settings Fields**: Displays all configurable handler parameters
4. **Nested Objects**: ScriptableObject references are displayed as expandable panels

## Usage Examples

### Setting up Data Saving

1. Create and configure SaveSettings: `Create > InitializationSystem > Settings > Save Settings`
2. Create SaveModule: `Create > InitializationSystem > Modules > Save Module`
3. Create a save handler (e.g., DefaultSaveHandler): `Create > InitializationSystem > Handlers > Default Save Handler`
4. Add SaveModule to ProjectInitSettings
5. Configure SaveModule, specifying the created SaveSettings and handler

After initialization, you can use SaveManager to work with saves:

```csharp
// Get a save object
var myData = SaveManager.GetSaveObject<MyDataClass>("uniqueName");

// Save data
SaveManager.MarkAsSaveIsRequired();
SaveManager.Save();

// Force save
SaveManager.ForceSave();
```

### Setting up Advertising

1. Create and configure AdsSettings: `Create > InitializationSystem > Settings > Ads Settings`
2. Create AdsModule: `Create > InitializationSystem > Modules > Ads Module`
3. Create an ads handler (e.g., DummyAdsHandler): `Create > InitializationSystem > Handlers > Dummy Handler`
4. Add AdsModule to ProjectInitSettings
5. Configure AdsModule, specifying the created AdsSettings and handler

After initialization, you can use AdsManager to work with ads:

```csharp
// Show rewarded ad
AdsManager.ShowRewarded(
    onSuccess: () => { /* Award reward */ },
    onFail: () => { /* Handle error */ }
);

// Show interstitial ad
AdsManager.ShowInterstitial();

// Banner management
AdsManager.ShowBanner();
AdsManager.HideBanner();
```

### Setting up Analytics

1. Create and configure AnalyticsSettings: `Create > InitializationSystem > Settings > Analytics Settings`
2. Create AnalyticsModule: `Create > InitializationSystem > Modules > Analytics Module`
3. Create analytics handlers (e.g., ConsoleAnalyticsHandler): `Create > InitializationSystem > Handlers > Console Handler`
4. Add AnalyticsModule to ProjectInitSettings
5. Configure AnalyticsModule, specifying the created AnalyticsSettings and handlers

After initialization, you can use AnalyticsManager to log events:

```csharp
// Log a simple event
AnalyticsManager.LogEvent("start_game");

// Log an event with parameters
AnalyticsManager.LogEvent("level_complete", new LogParameter[] {
    new LogParameter { Key = "level_id", Value = 5 },
    new LogParameter { Key = "time", Value = 120.5f },
    new LogParameter { Key = "stars", Value = 3 }
});
```
