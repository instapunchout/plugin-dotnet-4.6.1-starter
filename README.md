# plugin-dotnet-4.6.1-starter
This is a starter kit to developer InstaPunchout Plugin for .NET 4.6.1

## Get Started
            
    There is placeholder functions that need to be coded to function properly with the ecommerce system.
    All these functions are marked with a // TODO:

    The files that include all the code are the 2 controllers.

    There is 3 apis:
    - Index: this will be opened by the user comming from the procurement system and will log them in automatically.
    - Cart: this will be called when the user clicks on "Punchout" button, will load the cart, sends it to instapunchout to be converted to the proper format (cxml), then returned to the procurement system.
    - Script: this will be loaded by all pages, and will contain the punchout driver logic when in a punchout session.
    None of the apis require authentication and they should be open, authentication for login is done on instapunchout side.
