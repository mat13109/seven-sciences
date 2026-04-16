# Agent Notes

## Project Type
Godot 4 .NET (C#) mobile app. This is a 7 Wonders board game science calculator.

## The rules of science in 7 Wonders
The scientific cards earn victory points in two very different ways: from sets of identical symbols and from sets of 3 different symbols.
There are 3 science symbols:
- Gear
- Compass
- Tablet

Be careful: the victory points earned by both methods are cumulative.

### Sets of identical symbols
For each of the 3 existing scientific symbols, the player wins the following points :
- only 1 symbol: 1 victory point
- 2 identical symbols: 4 victory points
- 3 identical symbols: 9 victory points
- 4 identical symbols: 16 victory points
- and so on

Note:
- the number of points gained is equal to the number of symbols squared.

Example: Alexandria has built 6 scientific structures with the following symbols : 3 tablets, 2 compasses, 1 gear. They score 9 points for the family of 3 tablets (3x3), 4 points for its compasses (2x2) and finally 1 for the gear (1x1), for a total of 14 victory points.

### Sets of 3 different symbols
For each group of 3 different symbols, each player scores 7 victory points.

Example: Continuing the above example, Alexandria has built 6 scientific structures but only has a single group of 3 different symbols, they score 7 extra points for a total of 21 victory points. If Alexandria had built an extra structure with the gear symbol, they would’ve scored: (9 + 4 + 4) + (7 + 7) = 31 victory points. 

## Directory Structure
- `Scripts/` - C# code (Calculus/, Performances/, UI/)
- `Scenes/` - Godot scene files (.tscn)
- `Prefabs/` - Reusable scene prefabs
- `Art/` - Assets

## Entry Points
- `project.godot` defines `run/main_scene="res://Scenes/CalculatorInterface.tscn"`
- `Scripts/Main.cs` - main node, orchestrates score calculation
- `Scripts/Calculus/Maths.cs` - core scoring logic with recursive wild card optimization
- `Scripts/Calculus/EScienceSymbol.cs` - enum: Compass, Tablet, Cogwheel, Wild

## Code Style
- `.editorconfig` enforces 4-space indentation, specific C# formatting rules
- Private fields: `_camelCase`, static fields: `s_camelCase`
- File-scoped namespaces (`csharp_style_namespace_declarations = file_scoped`)
