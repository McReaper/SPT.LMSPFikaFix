**Makes LMSP plate swaps count in Fika raids.** With LMSP you can change plates without taking your armor off. In a Fika raid, the plate you put in didn't protect you, and the plate you took out kept taking the hits. LMSP Fika Fix makes the swap count.

> Install it on **every player's game, the host included**, along with LMSP and Fika.

{.is-warning}

## Details {.tabset}

### The bug

In a Fika raid, after you swap a plate with LMSP on armor you're wearing, until you take that armor off or the raid ends:

- **The plate you took out still protects you.** It doesn't matter where it is now: in your backpack, or on the ground. Hits use its class and its durability, and it's the plate that loses durability.
- **The plate you put in protects nothing.** It doesn't lose durability either.

So if you swap a worn class 4 plate for a new class 5 plate, you are still fighting behind the worn class 4.

Without Fika, this bug doesn't happen.

### Install

1. Install [LMSP](https://sp-mod.com/mod/2814/lmsp-let-me-switch-plates) and [Fika](https://sp-mod.com/mod/2326/project-fika) if you don't have them.
2. Download the zip and extract it into your SPT folder (the one with `EscapeFromTarkov.exe`). You should get `BepInEx/plugins/LMSPFikaFix.dll`.
3. Do the same on **every player's game, the host included**, even for players who don't swap plates themselves. When someone shoots you, *their* game works out whether the bullet goes through, using its own copy of your armor. The host does this for every bot.

There are no settings.

### Check that it works

1. In a raid, swap a plate on the armor you're wearing.
2. Open `BepInEx/LogOutput.log`. The last `[Info   :LMSP Fika Fix]` line names the plate, the armor, and every armor piece you now wear, with class and durability:

```
<you>: <new plate> put in <armor> (<plate slot>), worn armor is now: <helmet> (class 4, 30/30), <new plate> (class 5, 55/55), ...
```

The plate you took out should not be in that list. The same line shows up in the other players' logs, for their copy of you.

### How it works

Every player keeps a list of the armor they wear. Hit detection, penetration, durability loss and the plate hitboxes all read that list.

- **The game** rebuilds the list at the start of every hit.
- **Fika** removes that rebuild. It rebuilds the list only when one of the five armor equipment slots (headwear, face cover, eyewear, rig, body armor) gets a different item.

A plate going into or out of armor you keep on doesn't change what's in those slots, so the list keeps the old plate.

This addon rebuilds the list when an item with armor (a plate or a soft armor insert) goes into or out of worn armor. It does this for every player on every game in the raid: you, the copies of the other players, and the bots. LMSP itself is not changed.

Source code: [GitHub](https://github.com/McReaper/SPT.LMSPFikaFix)

{.endtabset}
