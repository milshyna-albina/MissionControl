# Mission Control

<h3>Traveler’s Adventure | Innovation campus of NTU KHPI | C# WPF project</h3>

Welcome to *Traveler’s Adventure* — a graphical desktop application built in *C#* with *WPF* for planning travel simulator! ✈️  
Plan your journey, explore new cities and find the shortest path to your destination! 🧭

---

## 🎮 Application overview

Traveler’s Adventure is organized into 3 main pages — each reflecting a key stage of your journey. Navigating through the app is simple and intuitive with friendly messages and error handling. 

### 📜 Menu
Start your adventure by creating or loading a traveler profile. Once you’ve chosen suitable option, you’ll move to the setup stage where you can enter traveler’s data and load map file.

### 🗺️ Route Planning
Once both traveler and map are set, press Plan Route to calculate the shortest path between your traveler’s current location and destination. The application uses Dijkstra’s algorithm to ensure you always get the most efficient route possible.

### 🧪 Experimental Mode
This mode lets you manually modify your route — add or remove cities, explore “what if” scenarios and rebuild paths dynamically without changing the main route. When you’re done, simply return to the main route view or save your progress for later.

---

## ✨ Features

### 🧍 Traveler Management
Create and manage your traveler manually or from a .json file — perfect for continuing your journey anytime!

### 🗺️ Map Loading
Load a map of cities and distances directly from a .txt file. The app automatically builds a graph of cities behind the scenes, ready for smart route planning.

### 🎯 Smart Route Planning
Let the app find the shortest path using Dijkstra’s algorithm. The full route and total travel distance are displayed in a clear format.

### 🏙️ City Management
Want to experiment? Add or remove cities manually and instantly see how it affects your route — ideal for testing new travel possibilities!

### 💾 Save & Load Progress
Save your traveler’s progress to a .json file and load it later to continue your journey exactly where you left off.

### 🔘 Navigation Buttons  
Each page includes an Exit button with a confirmation dialog and a Return button for navigating back.

### 💬 User-Friendly Error Handling
All common errors are caught and explained clearly — no crashes, no confusion.

---

## 🚀 How to Use the App

### 🧩 Step 1 – Clone the repository
Clone the Git repository and open the project in Visual Studio. Then hit `Run ▶️` to launch the app.

### 🧍 Step 2 — Create or Load a Traveler
You can either:
- **Load Data Manually ✍️** (enter the traveler’s name, current location and destination)
- **Load from JSON File 📂** (restore previously saved traveler information)

Example of a valid `traveler.json` file:

```json
{
  "name": "Alice",
  "currentLocation": "Kyiv",
  "route": ["Lviv", "Warsaw"]
}
```

### 🗺️ Step 3 — Load a Map

Load your `map.txt` file, which contains the cities and distances. Each line defines a connection in the following format: 
```
CityA-CityB,Distance
```
Example of a valid `map.txt`:
```txt
Kyiv-Lviv,540
Lviv-Warsaw,390
Kyiv-Odesa,480
```
📎 Note:
- The map can contain any number of city connections.
- Distances are in kilometers.


### 🎯 Step 4 — Plan the Route

Press **Plan Route** — the app calculates the shortest path and displays:
- The complete route
- All cities in order
- The total travel distance

Available actions:
- **Save 💾** export traveler data and route as JSON
- **Clear Route 🧹** reset all route information
- **Modify Route 🔧** switch to experimental mode for adjustments


### 🧪 Step 5 — Experiment (Optional)

Experimental Mode lets you modify the city network manually:
- **Add City ➕** adds a new city and automatically finds the shortest possible connection to your current route. The system extends your route intelligently including any intermediate cities needed for the optimal path.
- **Remove City ➖** removes a city and attempts to rebuild a valid route between the remaining ones. If there are alternative paths (for example, avoiding a specific city), the program automatically finds the next best route that keeps your journey connected — even if it means traveling through other cities instead.

---

## 🧠 Troubleshooting

Here’s how to handle common issues 👇

### ❌ File Not Found

Make sure:
- The file exists in the selected folder
- The file extension is correct (`.txt` or `.json`)
- You didn’t move or rename the file after saving

### ⚠️ Invalid File Format

If you see “Invalid file format” check that:
- Each line in `map.txt` follows the pattern `CityA-CityB,Distance`

✅ `Kyiv-Lviv,540`  
❌ `kyiv , lviv  540`

- Your `traveler.json` should look like this:

```json
{
  "name": "Alice",
  "currentLocation": "Kyiv",
  "route": ["Lviv", "Warsaw"]
}
```
### 🧭 No Route
If the app says “No Route”, it means there’s no valid path between the traveler’s location and the destination city in your map. Try adding missing connections in `map.txt`.

---

👥 The project **Mission Control** was developed by *Milshyna Albina* KN-224k.e and *Kashuashvili Mariia* KN-224k.e.
