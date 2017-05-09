{
  "4b84e588-adbf-4dd0-b892-c3a93b9d035e": {
    "ChartProperty": {
      "AxisColor": "Yellow",
      "AxisFont": "新細明體, 8pt",
      "BackgroundColor": "Black",
      "ChartSettings": [
        {
          "Axis": {
            "AxisScope": 3,
            "IsCreateInstance": false,
            "MarginBottom": 1.0,
            "MarginTop": 1.0,
            "ScaleMode": 0,
            "ScaleValue": 0.0
          },
          "ChartType": 5,
          "IsShowNewPrice": true,
          "IsSubChart": true,
          "LayerIndex": 0,
          "LegendColor": "Yellow",
          "PenStyles": [
            {
              "Color": "Red",
              "Pattern": -1,
              "Width": 1
            },
            {
              "Color": "Lime",
              "Pattern": -1,
              "Width": 1
            },
            {
              "Color": "Gray",
              "Pattern": -1,
              "Width": 1
            }
          ]
        }
      ],
      "DrawingSource": 0,
      "ForeColor": "White",
      "GridColor": "48, 48, 48",
      "IsShowGrid": true,
      "LegendFont": "新細明體, 8pt",
      "TextFont": "新細明體, 8pt",
      "TitleFont": "新細明體, 8pt",
      "TradeLineColor": "GreenYellow",
      "TradeSymbolColor": "DodgerBlue"
    },
    "ProfileId": "4b84e588-adbf-4dd0-b892-c3a93b9d035e",
    "Parameters": [
      "100",
      "4",
      "25",
      "4",
      "1"
    ],
    "Script": {
      "Property": {
        "DefaultContracts": 1,
        "InitialCapital": 0.0,
        "MaximumBarsReference": 0,
        "OrderSource": "Netwings.OrderService;Netwings.SimulateOrderService"
      },
      "DataRequests": [
        {
          "DataFeed": "Mitake",
          "DataPeriod": "1,Minute",
          "Exchange": "TWSE",
          "Range": "barsBack,2099/12/31;1000",
          "SymbolId": "MXF0.tw"
        }
      ]
    },
    "ScriptName": "PowerLanguage.Strategy.__AvgDealCount_Signal",
    "ScriptType": 0,
    "Window": {
      "IsDock": true,
      "Height": 319,
      "Left": 234,
      "Top": 25,
      "Width": 557,
      "WindowState": 0
    }
  }
}