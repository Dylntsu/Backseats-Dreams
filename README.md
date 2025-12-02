# 🚗 Backseats Dreams - Endless Runner 2D

> Un juego de carrera infinita desarrollado en Unity con enfoque en optimización y escalabilidad.

[![Itch.io](https://img.shields.io/badge/Itch.io-Jugar_Ahora-fa5c5c?style=for-the-badge&logo=itch.io)](https://dylntsu.itch.io/backseat-dream/devlog/1127706/dylntsu343-my-first-game)
![Unity](https://img.shields.io/badge/Unity-2022%2B-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp)
![Status](https://img.shields.io/badge/Status-Terminado-success?style=for-the-badge)

---

## 🎮 Descripción

**Backseats Dreams** es un Endless Runner 2D donde el jugador debe esquivar obstáculos, recolectar monedas y utilizar potenciadores estratégicos para alcanzar la mayor distancia posible. El proyecto destaca por su arquitectura de software optimizada para móviles y sistemas de progresión persistente.

## ✨ Características Principales

* **🏃‍♂️ Mecánicas de Movimiento:** Implementación de un **Máquina de Estados Finita (FSM)** para gestionar saltos, deslizamientos, caída rápida y estados de daño sin *bugs* de animación.
* **⚡ Sistema de Potenciadores:**
    * **Imán:** Atrae monedas cercanas.
    * **Escudo:** Protege de un impacto.
    * **Doble Monedas:** Multiplica el valor de recolección.
* **🛒 Tienda y Progresión:** Sistema de economía robusto basado en **archivos JSON** para guardar el progreso del jugador, monedas acumuladas y niveles de mejora de los power-ups.
* **🚀 Optimización (Object Pooling):** Implementación de un sistema de reciclaje de objetos para obstáculos y monedas, eliminando el `Instantiate/Destroy` constante para mejorar el rendimiento (Garbage Collection).
* **📉 Dificultad Progresiva:** La velocidad del juego aumenta gradualmente con el tiempo.

---

## 🛠️ Tecnologías y Patrones

* **Engine:** Unity (2D).
* **Lenguaje:** C#.
* **Patrones de Diseño:**
    * **Singleton:** Para `GameManager`, `UIManager` y `ShopManager`.
    * **Object Pooling:** Para la generación procedural de terreno y obstáculos.
    * **State Machine (FSM):** Para el control del `PlayerController`.
* **Persistencia de Datos:** Sistema de guardado y carga mediante **serialización JSON**, permitiendo una gestión de datos escalable y segura para el HighScore y el inventario.

---

## 🏗️ Filosofía de Desarrollo y Arquitectura

El desarrollo de este proyecto se centró en establecer una **arquitectura de software sólida** desde el inicio, priorizando la **escalabilidad** y la **mantenibilidad** a largo plazo sobre la funcionalidad inmediata.

* **Modularidad:** Cada script tiene una responsabilidad única, separando el control del jugador del control de la interfaz (UI) y del sistema de guardado.
* **Escalabilidad sin Caos:** Gracias a la implementación de **Máquinas de Estados (FSM)** y **Object Pooling**, la adición de nuevas mecánicas (como un nuevo Power-Up o un nuevo tipo de obstáculo) se realiza sin generar dependencias rígidas o *bugs* en el *core* del juego.
* **Uso Estratégico de Patrones:** La aplicación de patrones como FSM y Object Pooling fue la columna vertebral que asegura que el código es robusto y que las futuras actualizaciones serán eficientes.
---
🕹️ Jugar Demo
Puedes jugar la versión más reciente del juego directamente en Itch.io:

👉 Jugar Backseats Dreams en Itch.io
https://dylntsu.itch.io/backseat-dream/devlog/1127706/dylntsu343-my-first-game
---

## 📂 Estructura del Proyecto

```bash
├── Scripts/
│   ├── Managers/
│   │   ├── GameManager.cs      # Control central del flujo de juego
│   │   ├── SpawnManager.cs     # Lógica de Object Pooling y generación
│   │   └── UIManager.cs        # Control de HUD y feedback visual
│   ├── Player/
│   │   └── PlayerController.cs # Física, inputs y FSM del jugador
│   └── Objects/
│       ├── PowerUp.cs          # Lógica modular de potenciadores
│       └── MoveLeft.cs         # Comportamiento de desplazamiento del entorno
