This is a repo for any cosmoteer mods I make, which currently only consists of a resource sharing mod that enables resources to be taken from allied players via the resource transfer window, as opposed to only being able to send resources. This is intended to make it easier for multiple players to use a single shared cargo/transport ship.

This mod requires [C0dingschmuser's Enhanced Mod Loader](github.com/C0dingschmuser/EnhancedModLoader/) and Andreas Pardeike's [Harmony patching library](https://github.com/pardeike/Harmony) to function.

The mod seems to work fine on v0.27.0, but because of how the mod works it is quite possible it will stop working on newer versions, especially if the resource transfer logic changes. In particular, it just uses harmony to add `|| IsAlliesWithLocalPlayer` to various resource transfer related methods.
