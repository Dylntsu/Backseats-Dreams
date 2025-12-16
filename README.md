# 🚗 Backseats Dreams - Endless Runner 2D

> An endless runner game developed in Unity with a focus on optimization and scalability.

[![Itch.io](https://img.shields.io/badge/Itch.io-Play_Now-fa5c5c?style=for-the-badge&logo=itch.io)](https://dylntsu.itch.io/backseat-dream/devlog/1127706/dylntsu343-my-first-game)
![Unity](https://img.shields.io/badge/Unity-2025%2B-black?style=for-the-badge&logo=unity)
![C#](https://imgshields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp)
![Status](https://img.shields.io/badge/Status-Finished-success?style=for-the-badge)

---

## 🎮 Description

**Backseats Dreams** is a 2D Endless Runner where the player must dodge obstacles, collect coins, and use strategic power-ups to achieve the highest possible distance. The project stands out for its software architecture optimized for mobile platforms and persistent progression systems.

## ✨ Key Features

* **🏃‍♂️ Movement Mechanics:** Implementation of a **Finite State Machine (FSM)** to manage jumping, sliding, quick fall, and damage states without animation bugs.
* **⚡ Power-Up System:**
    * **Magnet:** Attracts nearby coins.
    * **Shield:** Protects from impacts.
    * **Double Coins:** Multiplies the value of collected coins.
* **🛒 Shop and Progression:** Economy system based on **JSON files** to save player progress, accumulated coins, and power-up upgrade levels.
* **🚀 Optimization (Object Pooling):** Implementation of an object recycling system for obstacles and coins, eliminating constant `Instantiate/Destroy` calls to improve performance (Garbage Collection).
* **📉 Progressive Difficulty:** Game speed gradually increases over time.

---

## 🛠️ Technologies and Patterns

* **Engine:** Unity (2D).
* **Language:** C#.
* **Design Patterns:**
    * **Singleton:** For `GameManager`, `UIManager`, and `ShopManager`.
    * **Object Pooling:** For procedural generation of terrain and obstacles.
    * **State Machine (FSM):** For `PlayerController` management.
* **Data Persistence:** Save and load system using **JSON serialization**, allowing for scalable and secure data management for HighScore and inventory.

---

## 🏗️ Development Philosophy and Architecture

The development of this project focused on establishing a **solid software architecture** from the start, prioritizing **scalability** and **long-term maintainability** over immediate functionality.

* **Modularity:** Each script has a unique responsibility, separating player control from the interface control (UI) and the save system.
* **Scalability without Chaos:** Thanks to the implementation of **State Machines (FSM)** and **Object Pooling**, adding new mechanics (such as a new Power-Up or a new type of obstacle) can be done without generating rigid dependencies or bugs in the game's core.
* **Strategic Pattern Usage:** The application of patterns like FSM and Object Pooling was the backbone that ensures the code is robust and future updates will be efficient.

---

## 📂 Project Structure

```bash
├── Scripts/
│   ├── Managers/
│   │   ├── GameManager.cs      # Central control of game flow
│   │   ├── SpawnManager.cs     # Object Pooling and generation logic
│   │   └── UIManager.cs        # HUD and visual feedback control
│   ├── Player/
│   │   └── PlayerController.cs # Player physics, inputs, and FSM
│   └── Objects/
│       ├── PowerUp.cs          # Modular power-up logic
│       └── MoveLeft.cs         # Environment scrolling behavior
