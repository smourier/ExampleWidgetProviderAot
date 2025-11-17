namespace ExampleWidgetProvider;

// TODO: build you own GUID
[Guid("a73cac52-e431-420d-9759-d510dfb2524e")]
public partial class WidgetProvider : IWidgetProvider
{
    private static readonly ManualResetEvent _emptyWidgetListEvent = new(false);

    public static ManualResetEvent GetEmptyWidgetListEvent() => _emptyWidgetListEvent;

    public WidgetProvider()
    {
        var runningWidgets = WidgetManager.GetDefault().GetWidgetInfos();

        foreach (var widgetInfo in runningWidgets)
        {
            var widgetContext = widgetInfo.WidgetContext;
            var widgetId = widgetContext.Id;
            var widgetName = widgetContext.DefinitionId;
            var customState = widgetInfo.CustomState;
            if (!RunningWidgets.ContainsKey(widgetId))
            {
                CompactWidgetInfo runningWidgetInfo = new() { widgetId = widgetId, widgetName = widgetName };
                try
                {
                    // If we had any save state (in this case we might have some state saved for Counting widget)
                    // convert string to required type if needed.
                    int count = Convert.ToInt32(customState.ToString());
                    runningWidgetInfo.customState = count;
                }
                catch
                {

                }
                RunningWidgets[widgetId] = runningWidgetInfo;
            }
        }
    }

    public void Activate(WidgetContext widgetContext)
    {
        var widgetId = widgetContext.Id;

        if (RunningWidgets.TryGetValue(widgetId, out CompactWidgetInfo? localWidgetInfo))
        {
            localWidgetInfo.isActive = true;

            UpdateWidget(localWidgetInfo);
        }
    }

    public void CreateWidget(WidgetContext widgetContext)
    {
        var widgetId = widgetContext.Id; // To save RPC calls
        var widgetName = widgetContext.DefinitionId;
        CompactWidgetInfo runningWidgetInfo = new() { widgetId = widgetId, widgetName = widgetName };
        RunningWidgets[widgetId] = runningWidgetInfo;


        // Update the widget
        UpdateWidget(runningWidgetInfo);
    }

    public void Deactivate(string widgetId)
    {
        if (RunningWidgets.TryGetValue(widgetId, out CompactWidgetInfo? localWidgetInfo))
        {
            localWidgetInfo.isActive = false;
        }
    }

    public void DeleteWidget(string widgetId, string customState)
    {
        RunningWidgets.Remove(widgetId);

        if (RunningWidgets.Count == 0)
        {
            _emptyWidgetListEvent.Set();
        }
    }

    public void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs)
    {
        var verb = actionInvokedArgs.Verb;
        if (verb == "inc")
        {
            var widgetId = actionInvokedArgs.WidgetContext.Id;
            // If you need to use some data that was passed in after
            // Action was invoked, you can get it from the args:
            //var data = actionInvokedArgs.Data;
            if (RunningWidgets.TryGetValue(widgetId, out CompactWidgetInfo? localWidgetInfo))
            {
                // Increment the count
                localWidgetInfo.customState++;
                UpdateWidget(localWidgetInfo);
            }
        }
    }

    public void OnWidgetContextChanged(WidgetContextChangedArgs contextChangedArgs)
    {
        var widgetContext = contextChangedArgs.WidgetContext;
        var widgetId = widgetContext.Id;
        ///var widgetSize = widgetContext.Size;
        if (RunningWidgets.TryGetValue(widgetId, out CompactWidgetInfo? localWidgetInfo))
        {
            UpdateWidget(localWidgetInfo);
        }
    }

    private static void UpdateWidget(CompactWidgetInfo localWidgetInfo)
    {
        WidgetUpdateRequestOptions updateOptions = new(localWidgetInfo.widgetId);

        string? templateJson = null;
        if (localWidgetInfo.widgetName == "Weather_Widget")
        {
            templateJson = _weatherWidgetTemplate.ToString();
        }
        else if (localWidgetInfo.widgetName == "Counting_Widget")
        {
            templateJson = _countWidgetTemplate.ToString();
        }

        string? dataJson = null;
        if (localWidgetInfo.widgetName == "Weather_Widget")
        {
            dataJson = "{}";
        }
        else if (localWidgetInfo.widgetName == "Counting_Widget")
        {
            dataJson = "{ \"count\": " + localWidgetInfo.customState.ToString() + " }";
        }

        updateOptions.Template = templateJson;
        updateOptions.Data = dataJson;
        // You can store some custom state in the widget service that you will be able to query at any time.
        updateOptions.CustomState = localWidgetInfo.customState.ToString();
        WidgetManager.GetDefault().UpdateWidget(updateOptions);
    }

    const string _weatherWidgetTemplate = """
    {
        "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
        "type": "AdaptiveCard",
        "version": "1.0",
        "speak": "<s>The forecast for Seattle January 20 is mostly clear with a High of 51 degrees and Low of 40 degrees</s>",
        "backgroundImage": "https://messagecardplayground.azurewebsites.net/assets/Mostly%20Cloudy-Background.jpg",
        "body": [
            {
                "type": "TextBlock",
                "text": "Redmond, WA",
                "size": "large",
                "isSubtle": true,
                "wrap": true
            },
            {
                "type": "TextBlock",
                "text": "Mon, Nov 4, 2019 6:21 PM",
                "spacing": "none",
                "wrap": true
            },
            {
                "type": "ColumnSet",
                "columns": [
                    {
                        "type": "Column",
                        "width": "auto",
                        "items": [
                            {
                                "type": "Image",
                                "url": "https://messagecardplayground.azurewebsites.net/assets/Mostly%20Cloudy-Square.png",
                                "size": "small",
                                "altText": "Mostly cloudy weather"
                            }
                        ]
                    },
                    {
                        "type": "Column",
                        "width": "auto",
                        "items": [
                            {
                                "type": "TextBlock",
                                "text": "46",
                                "size": "extraLarge",
                                "spacing": "none",
                                "wrap": true
                            }
                        ]
                    },
                    {
                        "type": "Column",
                        "width": "stretch",
                        "items": [
                            {
                                "type": "TextBlock",
                                "text": "°F",
                                "weight": "bolder",
                                "spacing": "small",
                                "wrap": true
                            }
                        ]
                    },
                    {
                        "type": "Column",
                        "width": "stretch",
                        "items": [
                            {
                                "type": "TextBlock",
                                "text": "Hi 50",
                                "horizontalAlignment": "left",
                                "wrap": true
                            },
                            {
                                "type": "TextBlock",
                                "text": "Lo 41",
                                "horizontalAlignment": "left",
                                "spacing": "none",
                                "wrap": true
                            }
                        ]
                    }
                ]
            }
        ]
    }
    """;

    const string _countWidgetTemplate = """
    {                                                                     
        "type": "AdaptiveCard",                                         
        "body": [                                                         
            {                                                               
                "type": "TextBlock",                                    
                "text": "You have clicked the button ${count} times"    
            },
            {
                    "text":"Rendering Only if Small",
                    "type":"TextBlock",
                    "$when":"${$host.widgetSize==\"small\"}"
            },
            {
                    "text":"Rendering Only if Medium",
                    "type":"TextBlock",
                    "$when":"${$host.widgetSize==\"medium\"}"
            },
            {
                "text":"Rendering Only if Large",
                "type":"TextBlock",
                "$when":"${$host.widgetSize==\"large\"}"
            }                                                                    
        ],                                                                  
        "actions": [                                                      
            {                                                               
                "type": "Action.Execute",                               
                "title": "Increment",                                   
                "verb": "inc"                                           
            }                                                               
        ],                                                                  
        "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
        "version": "1.5"                                                
    }
    """;
    public static IDictionary<string, CompactWidgetInfo> RunningWidgets { get; } = new Dictionary<string, CompactWidgetInfo>();
}

