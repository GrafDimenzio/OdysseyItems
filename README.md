# OdysseyItems

**OdysseyItems** is a plugin for the [DSMOO](https://github.com/GrafDimenzio/DSMOO) server for *Super Mario Odyssey Online*. It adds various collectible items to the game. Items are represented in-game by spinning Marios, and players can collect them simply by walking into them.

---

## Table of Items

| Item | Outfit | Effect | Notes |
|------|--------|--------|-------|
| **Delay** | Crazy Cap Shop Outfit | Delays all position packets from Mario. Other players see his actions with a 1-second delay, making it harder to tag him. | Duration and delay amount are configurable. |
| **Escape** | Space Suit | Teleports Mario to a random location, useful for evading seekers during a chase. | Can be configured to always teleport to a different stage. |
| **Star / Invincibility** | Clown | Makes Mario invincible and untaggable by seekers. Achieved by setting all players as seekers on the client side. | Duration is configurable. |
| **Invisibility** | Tail Coat | Makes Mario invisible to other players. | Duration is configurable. |
| **Offset** | Mario64 | Displays Mario with a positional offset (default: 500 units above). Seekers must guess the real location. | Duration and offset vector are configurable. |
| **Bonus Health**<br>(Requires [SMOO+](https://github.com/DaDev123/SMOO-Plus)) | Doctor | Instantly restores health (default: 12). | Only available with SMOO+. |
| **Kill Item**<br>(Requires [SMOO+](https://github.com/DaDev123/SMOO-Plus)) | Skeleton | Instantly eliminates all seekers upon collection. | Only available with SMOO+. |

---

## Installation

1. Download the `OdysseyItems.dll` file.
2. Place it in your DSMOO plugin directory.
3. After installation, default configuration files are automatically generated.

---

## Usage

- Items spawn automatically when a game starts.
- Start a game using:

```
game start hideandseek <map> <hintPreset> <players> <waitTime> <initialSeekers>
```

- Example:

```
game start hideandseek cap default * 90 2
```

- Parameters:
    - `<map>`: The kingdom to play in (e.g., `cap`, `cascade`).
    - `<hintPreset>`: Automatic hint preset (`default` or `none` to disable).
    - `<players>`: `*` for all players, or a space-separated list of specific players.
    - `<waitTime>`: Time in seconds seekers wait before starting (e.g., `90`).
    - `<initialSeekers>`: Number of seekers at the start.

- Use `game list` to see available stages and hint presets. Each kingdom has variants like:
    - `cap`: Cap Kingdom including all subareas (other stages banned).
    - `cap-nosub`: Only the main stage of Cap Kingdom.
    - `cap-all`: Start in Cap but allow all stages in the game.

---

## OdysseyItemsPlus

**OdysseyItemsPlus** is an extension that adds [SMOO+](https://github.com/DaDev123/SMOO-Plus) support:

- Grants access to two additional items: Bonus Health and Kill Item.
- Provides notifications when items are collected or when their effects expire.
- Recommended for full functionality, but not required.

---

## Configuration

| Config Option | Description |
|---------------|-------------|
| `EnabledItems` | List of items to enable. Include `Bonus Health` and `Kill` to enable SMOO+ exclusive items. |
| `SeekerDestroyItem` | If enabled, seekers despawn items upon touching them. |
| `ItemAmount` | Number of items to spawn. Ensure `maxplayers >= players + items`. Change max players with `config maxplayers <amount>`. |
| `ShowItemNameOnPlayerList` | Show item names indicating their powers (e.g., "Star Item") instead of generic "Item". |
| `InitialWaitTimeToSpawn` | Delay before first items spawn. |
| `RespawnTime` | Interval for respawning available items. |
| `InvisibilityTime` | Duration of invisibility effect. |
| `InvincibilityTime` | Duration of invincibility effect. |
| `AlwaysOtherStageOnEscape` | Escape item always teleports to a different stage. |
| `OffsetTime` | Duration of Offset item. |
| `Offset` | Vector for the Offset item effect. |
| `DelayTime` | Duration of Delay item. |
| `AmountOfDelay` | Number of seconds packets are delayed. |

---

## Gameplay Notes

- Only players in Hider-Mode can collect Items
- Items appear in the player list:
    - **Seekers**: Item not on the map.
    - **Hiders**: Item available on the map.
- While an item effect (like Invisibility) is active, it won’t respawn.
- Multiple instances of the same item type can exist simultaneously. For example, if `ItemAmount = 4`, a single player could temporarily hold four Invisibility items.  