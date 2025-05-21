# Case Project – Level-Based Puzzle Game

This Unity project was developed for a Case Project. The game is a simple, level-based mobile puzzle game inspired by mechanics found in casual titles like *Royal Match*. The core objective is to clear all obstacles on a grid-based board using color-matching cube blasts and special items, all within a limited number of moves.

## Gameplay Overview

- **Grid Mechanics**: A rectangular grid (6x6 to 10x10) filled with colored cubes.
- **Blasting Cubes**: Tapping 2 or more adjacent (non-diagonal) cubes of the same color blasts them.
- **Gravity**: Cubes fall vertically into empty spaces. New cubes spawn from the top using object pooling.
- **Obstacles**: Box, Stone, and Vase types that require different strategies to clear.
- **Special Items**:
  - **TNT**: Created from blasting ≥5 cubes. Explodes in a 5x5 area.
  - **Combos**: TNT-TNT combinations create larger explosions.

## Levels

- 10 pre-configured levels included.
- Each level defined with:
  - Grid width and height
  - Move count
  - Grid layout using item codes (`r`, `g`, `b`, `y`, `bo`, `s`, `v`, etc.)

## Technical Details

- **Engine**: Unity 2022.3.8 (Built-in Renderer)
- **Language**: C#
- **Platform**: Portrait orientation (9:16)
- **Animations**: Implemented using tween libraries (excluding DI libraries like Zenject)
- **Performance**: Object pooling used for new cube generation
- **Editor Tools**: Menu item for adjusting the current level during testing

## Features Implemented

- Level progression system
- Obstacle-clearing logic
- Blast and combo mechanics
- Object pooling for optimized performance
- Persistent local save system
- Replay and return flow for failed levels
- Celebration effects for wins
- 10 fully playable levels

## Getting Started

1. Clone the repository
2. Open with Unity 2022.3.8
3. Load the `MainScene` and press Play
4. Use the LevelButton to start playing

> This project is playable within the Unity Editor and does not require a mobile build.

---

*Developed as a Case Project – 2024.*
