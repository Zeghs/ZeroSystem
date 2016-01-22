{
  "119c108e-059c-4910-b3f8-1892950a82d0": {
    "Charts": [
      {
        "Axis": {
          "AxisScope": 0,
          "MarginBottom": 0.0,
          "MarginTop": 0.0,
          "ScaleMode": 0,
          "ScaleValue": 0.0
        },
        "ChartType": 3,
        "IsShowNewPrice": false,
        "IsSubChart": true,
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
          }
        ]
      },
      {
        "Axis": {
          "AxisScope": 0,
          "MarginBottom": 0.0,
          "MarginTop": 0.0,
          "ScaleMode": 0,
          "ScaleValue": 0.0
        },
        "ChartType": 3,
        "IsShowNewPrice": false,
        "IsSubChart": true,
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
          }
        ]
      }
    ],
    "ProfileId": "119c108e-059c-4910-b3f8-1892950a82d0",
    "Parameters": [
      "1",
      "1"
    ],
    "Script": {
      "Property": {
        "DefaultContracts": 1,
        "InitialCapital": 100000.0,
        "MaximumBarsReference": 60,
        "OrderSource": "Netwings.OrderService;Netwings.RealOrderService"
      },
      "DataRequests": [
        {
          "DataFeed": "Mitake",
          "DataPeriod": "1,Minute",
          "Exchange": "TWSE",
          "Range": "barsBack,3000/12/31;100",
          "SymbolId": "TXF0.tw"
        },
        {
          "DataFeed": "Mitake",
          "DataPeriod": "1,Day",
          "Exchange": "TWSE",
          "Range": "barsBack,3000/12/31;10",
          "SymbolId": "TXF0.tw"
        }
      ]
    },
    "ScriptName": "PowerLanguage.Strategy.__BIAS_Signal",
    "ScriptType": 0,
    "Window": {
      "Height": 344,
      "Left": -3,
      "Top": -3,
      "Width": 557,
      "WindowState": 2
    }
  }
}