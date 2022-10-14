# InstaPunchout Plugin .NET 4.6.1 Starter
This is a starter kit to developer InstaPunchout Plugin for .NET 4.6.1

## Get Started
            
There is placeholder functions that need to be coded to function properly with the ecommerce system.
All these functions are marked with a // TODO:

The files that include all the code are the 2 controllers.


## Apis to be implemened on eCommerce Side
- Index: this will be opened by the user comming from the procurement system and will log them in automatically.
The index api can be tested with: http://localhost:60771/punchout?id=3J3q6HVhNx66BBZ9mmXcZ1
- Cart: this will be called when the user clicks on "Punchout" button, will load the cart, sends it to instapunchout to be converted to the proper format (cxml), then returned to the procurement system.
- Script: this will be loaded by all pages, and will contain the punchout driver logic when in a punchout session.
None of the apis require authentication and they should be open, authentication for login is done on instapunchout side.

## Script injection
The script /punchout/script needs to be added on everypage, ideally it should be added in the layout template or something similar.
It should be added at the end of the head section before the body.

## Instapunchout APIs

There is 3 apis:
- /proxy: this will be called in the Index Controller, and will control the login flow.
- /cart/{punchoutId}: the cart will be sent to this api, and the response will be used by the injected javascript to redirect the user back to the procurement system. 
- /punchout.js?id={punchoutId}: used by the script controller to load the correct javascript which is identified using the punchoutId.
Details on how these apis should used are in the PunchoutController.cs
