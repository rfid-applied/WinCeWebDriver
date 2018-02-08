# WinCeWebDriver

This is a
[WebDriver](https://w3c.github.io/webdriver/webdriver-spec.html)
remote end written in .NET CF 3.5 for C# Windows CE / Windows Mobile
6.5 Web server is intended to be used for RESTful web services

# Running integration tests

1. Ensure you can connect to your WM6.5 device over Wi-Fi (assigning
   static IP also helps).

2. Deploy `SimpleWebDriver` to a WM6.5 device (ensure .NET CF 3.5 is
   installed on it), then run the executable.

3. Deploy `SimpleWinceGuiAutomation.AppTest` to the device (don't run it)

4. Build and run `SimpleWebDriver.Tests` from command line:

````
$ SimpleWebDriver.Tests.exe <IP> <PORT>
````

where

* <IP> stands for IP adress of the device
* <PORT> stands for the port the webserver is listening on (8080 by default)

# How to use?

Run some scripts following the webdriver protocol.

# Subset of WebDriver protocol supported

1. session create / delete (only one app is supported at a time)
2. status
3. get title
4. source (get the object tree the driver is operating against as JSON)
5. screenshot (both full-screen and selected element)
6. single and multi-element queries
   
   * css selector
   * link text, partial link text
   * tag name

7. element state requests (rect, enabled, selected, text, name)
8. element interaction (send keys & clear, click)

# CSS selectors supported

1. by tag name `tagname` with optional attribute query `[element OP value]`
2. narrowing down: `A B` (descendants), `A ~ B` (followed by),
   `A + B` (immediately followed by), `A > B` (direct children)

To define the "document order", we just sort all controls by Top then
Left coordinates.

# Limitations

1. SendKeys doesn't support characters beyond Latin
2. It's still WinAPI so there isn't too much info about controls
3. Only one app at a time (can be changed, if needed)

# Credits

[WinCeWebServer](https://github.com/snitkjaer/WinCeWebServer)
[SimpleWinCeGuiAutomation](https://github.com/Yavari/simple-wince-gui-automation)

WebDriver implementation contributed by Artyom Shalkhakov (artyom DOT
shalkhakov AT gmail DOT com)
