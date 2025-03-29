This is a repo for any cosmoteer mods I make, which currently only consists of a resource sharing mod that enables resources to be taken from allied players via the resource transfer window, as opposed to only being able to send resources. This is intended to make it easier for multiple players to use a single shared cargo/transport ship.

This mod requires the [Enhanced Mod Loader](github.com/ElectroJr/EnhancedModLoader) to function. You'll need a publicised cosmoteer assembly in order to build the mod. I.e., use your preferred publiciser to generate a publicised dll, and then update the references in the `.csproj` file. I just used the [BepInEx.AssemblyPublicizer](https://github.com/BepInEx/BepInEx.AssemblyPublicizer).

The mod seems to work fine on v0.28.2, but because of how the mod works it is quite possible it will stop working on newer versions, especially if the resource transfer logic changes. In particular, it just uses harmony to add `|| IsAlliesWithLocalPlayer` to various resource transfer related methods.

