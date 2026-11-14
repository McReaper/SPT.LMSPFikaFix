# Plate Swap Fix

[![Downloads](https://img.shields.io/github/downloads/McReaper/SPT.PlateSwapFix/total)](https://github.com/McReaper/SPT.PlateSwapFix/releases)

Client mod for **SPT + Fika**. When you swap a plate on armor you're wearing during a raid,
the new plate now protects you, and the old one stops counting.

Made for [LMSP - Let Me Switch Plates](https://forge.sp-tarkov.com/mod/2814/lmsp-let-me-switch-plates)
([GitHub](https://github.com/nessu1n/LetMeSwitchPlates)), which lets you change plates without
taking your armor off. It works the same with any mod that does this. It doesn't need LMSP,
and it doesn't change LMSP.

## The bug

In a Fika raid, after you swap a plate on armor you're wearing, until you take that armor off
or the raid ends:

- **the plate you took out still protects you.** It doesn't matter where it is now (in your
  backpack, or on the ground). Hits use its class and its durability, and it's the plate that
  loses durability.
- **the plate you put in protects nothing.** It doesn't lose durability either.

So if you swap a worn class 4 plate for a new class 5 plate, you are still fighting behind
the worn class 4. Taking the armor off and putting it back on fixes it until the next swap.

Without Fika, this bug doesn't happen.

## Requirements

- SPT 4.1 with Fika 2.4 (built against SPT 4.1.6 and Fika 2.4.4).
- **Every player in the raid, the host included, even players who don't use LMSP.** When
  someone shoots you, *their* game works out whether the bullet goes through, using its
  own copy of your armor. The host does this for every bot. Each game has to see your
  swap.

## Install

Extract the release zip into your SPT folder (the one with `EscapeFromTarkov.exe`), so that
you get `BepInEx/plugins/PlateSwapFix.dll`. There are no settings.

## Check that it works

1. In a raid, swap a plate on the armor you're wearing.
2. Open `BepInEx/LogOutput.log`. The last `[Info   :Plate Swap Fix]` line names the plate,
   the armor, and every armor piece you now wear, with class and durability:

   ```
   <you>: <new plate> put in <armor> (<plate slot>), worn armor is now: <helmet> (class 4, 30/30), <new plate> (class 5, 55/55), ...
   ```

   The plate you took out should not be in that list.

The same line shows up in the other players' logs, for their copy of you.

## How it works

Every player keeps a list of the armor they wear. Hit detection, penetration, durability
loss and the plate hitboxes all read that list.

- **The game** rebuilds the list at the start of every hit.
- **Fika** drops that, to save time on every shot. It rebuilds the list only when one of the
  five armor equipment slots (headwear, face cover, eyewear, rig, body armor) gets a
  different item.

A plate going into or out of armor you keep on doesn't change what's in those slots, so the
list keeps the old plate.

This mod rebuilds the list when an item with armor (a plate or a soft armor insert) goes
into or out of worn armor. It does it for every player on every game in the raid: you, the
copies of the other players, and the bots. It hooks the inventory events the game sends to
each player, and does the same check the game itself uses (`Player.OnItemAddedOrRemoved`).

## Build

Requires the .NET SDK and an SPT install with Fika.

```sh
dotnet build -c Release -p:SPTPath="C:\SPT"
# add -p:InstallToSPT=true to copy the DLL into BepInEx/plugins
```

## License

MIT
